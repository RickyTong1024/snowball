#!/usr/bin/env python  
#coding=utf-8

import sys
sys.path.append('../common')
import tornado.ioloop
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

create_db()

def dispatch(tin):
    name = tin[0]
    rid = tin[1]
    opcode = tin[2]
    msg = tin[3]

    if opcode == opcodes["REQ_HALL_CENTER_LIBAO"]:
        try:
            libao(name, rid, msg)
        except:
            print("msg error")
     
def libao(name, rid, msg):
    inmsg = pmsg_req_libao()
    inmsg.ParseFromString(msg)
    code = inmsg.code
    use = inmsg.use

    res, reward, pc, cf = self.check(code, use)
    print("res:", res)

    outmsg = pmsg_rep_libao()
    outmsg.res = res
    outmsg.pc = pc
    outmsg.cf = cf
    for i in range(len(reward)):
        outmsg.types.append(reward[i][0])
        outmsg.value1.append(reward[i][1])
        outmsg.value2.append(reward[i][2])
        outmsg.value3.append(reward[i][3])
    s = outmsg.SerializeToString()
    data = packet.to_pck_string(opcodes["REQ_HALL_CENTER_LIBAO"], s)
    pull.ioloop.instance().response_msg(name, rid, data)

def check(self, code, use):
    codeq = code[:2]
    self.ping()
    cur = self.db.cursor()
    sql = "select code, pc, cf, type, value1, value2, value3 from libao_type where code = '%s'"
    param = (codeq,)
    cur.execute(sql, param)
    data = cur.fetchall()
    if len(data) != 1:
        print("没有该礼包码")
        return -1, [], 0, 0
    data = data[0]
    pc = data[1]
    cf = data[2]
    types = data[3]
    l, types = struct.unpack('i%ds' % (len(types) - 4), types)
    type_arr = []
    for i in range(l):
        j, types = struct.unpack('i%ds' % (len(types) - 4), types)
        type_arr.append(j)
    value1 = data[4]
    l, value1 = struct.unpack('i%ds' % (len(value1) - 4), value1)
    value1_arr = []
    for i in range(l):
        j, value1 = struct.unpack('i%ds' % (len(value1) - 4), value1)
        value1_arr.append(j)
    value2 = data[5]
    l, value2 = struct.unpack('i%ds' % (len(value2) - 4), value2)
    value2_arr = []
    for i in range(l):
        j, value2 = struct.unpack('i%ds' % (len(value2) - 4), value2)
        value2_arr.append(j)
    value3 = data[6]
    l, value3 = struct.unpack('i%ds' % (len(value3) - 4), value3)
    value3_arr = []
    for i in range(l):
        j, value3 = struct.unpack('i%ds' % (len(value3) - 4), value3)
        value3_arr.append(j)
    reward = []
    for i in range(len(type_arr)):
        reward.append([type_arr[i], value1_arr[i], value2_arr[i], value3_arr[i]])

    sql = "select code, used from libao where code = '%s'"
    param = (code,)
    cur.execute(sql, param)
    data = cur.fetchall()
    if len(data) != 1:
        print("没有该礼包码")
        return -1, [], 0, 0
    data = data[0]
    if data[1] == 1:
        print("已被使用")
        return -3, [], 0, 0
    if use == '1' and cf == 0:
        sql = "update libao set used = 1 where code = '%s'"
        param = (code,)
        cur.execute(sql, param)
    print(reward)
    return 0, reward, pc, cf

def create_db(self):
    db = pymysql.connect(user='root' ,passwd='root' ,db='snowball_libao', host='127.0.0.1')
    db.autocommit(1)
    
def ping(self):
    try:
        db.ping()
    except Exception as e:
        create_db()

def main():
    pull.ioloop.instance().start(config.myname, dispatch)
    print('Welcome to the machine...')
    tornado.ioloop.IOLoop.instance().start()
            
if __name__ == '__main__':
    main()
