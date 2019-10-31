#!/usr/bin/env python  
#coding=utf-8

import sys
sys.path.append('../common')
import tornado.httpserver
import tornado.ioloop
import tornado.web
import time
import config
from msg_pipe_pb2 import *
import json
import urllib.parse
from tornado.httpclient import AsyncHTTPClient, HTTPRequest
import pull
import packet
from opcodes import *
import pymysql

def dispatch(tin):
    name = tin[0]
    rid = tin[1]
    opcode = tin[2]
    msg = tin[3]

    if opcode == opcodes["REQ_LOGIN"]:
        login(name, rid, msg)

def login(name, rid, msg):
    inmsg = pmsg_req_login()
    inmsg.ParseFromString(msg)
    uid = inmsg.uid
    token = inmsg.token
    pt = inmsg.pt

    def login_callback(response):
        if response.error:
            print(response.error)
            return
        elif response.code != 200:
            res = -1
            print("state", response.code)
            return
        else:
            r = response.body
            print("res", r)
            errno = 0
            if r != b'0': 
                errno = -1

        outmsg = pmsg_rep_login()
        outmsg.errres = errno
        outmsg.errmsg = ""
        s = outmsg.SerializeToString()
        data = packet.to_pck_string(opcodes["REQ_LOGIN"], s)
        pull.ioloop.instance().response_msg(name, rid, data)

    httpClient = AsyncHTTPClient()
    headers = {"Content-type": "application/x-www-form-urlencoded", "Accept": "text/plain"}
    body = urllib.parse.urlencode({'username': uid, 'password': token})
    print(body)
    req = HTTPRequest(url="http://127.0.0.1:40001/login", method="POST", headers=headers, body=body)
    httpClient.fetch(req, callback=login_callback)

class reg_handler(tornado.web.RequestHandler):
    def post(self):
        # 收到包，取出地址信息
        username = self.get_body_argument("username")
        password = self.get_body_argument("password")

        print("username:%s password:%s" % (username, password))

        l = len(password)
        if l < 6 or l > 14:
            # 长度问题
            print("len error -1")
            self.write("-1")
            self.finish()
            return

        self.application.ping()
        cur = self.application.db.cursor()
        sql = "select password from user where username = %s"
        param = (username,)
        cur.execute(sql, param)
        res = cur.fetchall()

        if len(res) != 0:
            # 已存在
            print("dump error -2")
            self.write("-2")
            self.finish()
            return
        
        sql = "insert into user (username, password) values(%s, %s)"
        param = (username, password,)
        cur.execute(sql, param)
        # 注册成功
        print("register suc 0")
        self.write("0")
        self.finish()

class login_handler(tornado.web.RequestHandler):
    def post(self):
        # 收到包，取出地址信息
        username = self.get_body_argument("username")
        password = self.get_body_argument("password")

        print("username:%s password:%s" % (username, password))

        l = len(password)
        if l < 6 or l > 14:
            # 长度问题
            print("len error -1")
            self.write("-1")
            self.finish()
            return

        self.application.ping()
        cur = self.application.db.cursor()
        sql = "select password from user where username = %s"
        param = (username,)
        cur.execute(sql, param)
        res = cur.fetchall()

        if len(res) > 1:
            # 系统错误
            print("sys error -2")
            self.write("-2")
            self.finish()
            return
        elif len(res) == 0:
            # 未注册
            print("unregister error -3")
            self.write("-3")
            self.finish()
            return
        else:
            pw = res[0][0]
            if pw != password:
                # 密码错误
                print("password error -4")
                self.write("-4")
                self.finish()
                return

        # 登陆成功
        print("login suc 0")
        self.write("0")
        self.finish()

class chpwd_handler(tornado.web.RequestHandler):
    def post(self):
        # 收到包，取出地址信息
        username = self.get_body_argument("username")
        old_password = self.get_body_argument("old_password")
        new_password = self.get_body_argument("new_password")

        print("username:%s old_password:%s new_password:%s" % (username, old_password, new_password))

        l = len(old_password)
        if l < 6 or l > 14:
            # 长度问题
            print("len error -1")
            self.write("-1")
            self.finish()
            return

        l = len(new_password)
        if l < 6 or l > 14:
            # 长度问题
            print("len error -1")
            self.write("-1")
            self.finish()
            return

        self.application.ping()
        cur = self.application.db.cursor()
        sql = "select password from user where username = %s"
        param = (username,)
        cur.execute(sql, param)
        res = cur.fetchall()

        if len(res) > 1:
            # 系统错误
            print("unknow error -2")
            self.write("-2")
            self.finish()
            return
        elif len(res) == 0:
            # 未注册
            print("register error -3")
            self.write("-3")
            self.finish()
            return
        else:
            pw = res[0][0]
            if pw != old_password:
                # 密码错误
                print("password error -4")
                self.write("-4")
                self.finish()
                return
            sql = "update user set password = %s where username = %s"
            param = (new_password, username,)
            cur.execute(sql, param)
        # 修改成功
        print("chpwd suc 0")
        self.write("0")
        self.finish()
        
class Application(tornado.web.Application):  
    def __init__(self):  
        handlers = [
            (r"/reg", reg_handler),
            (r"/login", login_handler),
            (r"/chpwd", chpwd_handler),
        ]
        tornado.web.Application.__init__(self, handlers)
        self.create_db()

    def create_db(self):
        self.db = pymysql.connect(user='root' ,passwd='root' ,db='snowball_user', host='127.0.0.1')
        self.db.autocommit(1)
        
    def ping(self):
        try:
            self.db.ping()
        except Exception as e:
            self.create_db()
        
def main():
    pull.ioloop.instance().start(config.myname, dispatch)
    http_server = tornado.httpserver.HTTPServer(Application())
    http_server.listen(40001)
    print('Welcome to the machine...')
    tornado.ioloop.IOLoop.instance().start()
            
if __name__ == '__main__':
    main()

