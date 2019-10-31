#!/usr/bin/env python  
#coding=utf-8

import zmq
import time
import threading
from rpc_pb2 import *
import read_config
import tornado.ioloop
import packet

context = None

class push_thread(threading.Thread):
    def __init__(self, name):
        threading.Thread.__init__(self)

        self.outmsgq = []
        self.outlock = threading.Lock()
        self.push_socket = context.socket(zmq.PUSH)
        addr = "tcp://" + read_config.servers[name]["host"] + ":" + str(read_config.servers[name]["port"])
        self.push_socket.connect(addr)
        print("push_thread start")

    def add_msg(self, s):
        self.outlock.acquire()
        self.outmsgq.append(s)
        self.outlock.release()
             
    def run(self):
        global outmsgq
        
        while True:
            flag = True
            self.outlock.acquire()
            tout = list(self.outmsgq)
            self.outmsgq = []
            self.outlock.release()
            l = len(tout)
            if l > 0:
                flag = False
            for i in range(l):
                pck_str = tout.pop()
                self.push_socket.send(pck_str, zmq.NOBLOCK)

            # 队列空，休息0.01秒
            if flag == True:
                time.sleep(0.01)

class pull_thread(threading.Thread):
    def __init__(self, name):
        threading.Thread.__init__(self)

        self.inmsgq = []
        self.inlock = threading.Lock()
        self.pull_socket = context.socket(zmq.PULL)
        addr = "tcp://" + read_config.servers[name]["host"] + ":" + str(read_config.servers[name]["port"])
        self.pull_socket.bind(addr)
        print("pull_thread start")

    def get_msg(self):
        self.inlock.acquire()
        tin = list(self.inmsgq)
        self.inmsgq = []
        self.inlock.release()

        return tin
        
    def run(self):
        while True:
            # 收返回消息
            message = self.pull_socket.recv()
            try:
                # 解析返回包
                inmsg = rpc()
                inmsg.ParseFromString(message)
                opcode, data = packet.to_pck_data(inmsg.req.msg)

                # 加入队列
                inm = [inmsg.req.name, inmsg.req.id, opcode, data]
                self.inlock.acquire()
                self.inmsgq.append(inm)
                self.inlock.release()
            except Exception as e:
                print(e)
                
class ioloop():
    def __init__(self):
        global context
        context = zmq.Context(1)
        self.pull_thread = None
        self.tm = 0
        self.push_threads = {}
        self.dispatch = None
        self.name = ""

    @classmethod
    def instance(cls):
        if not hasattr(cls, "_instance"):
            cls._instance = cls()
        return cls._instance

    def response_msg(self, name, rid, msg):
        rpc_msg = rpc()
        rpc_msg.type = RESPONSE
        rpc_msg.rep.name = self.name
        rpc_msg.rep.id = rid
        if msg != "":
            rpc_msg.rep.msg = msg
        s = rpc_msg.SerializeToString()

        if name in self.push_threads:
            self.push_threads[name].add_msg(s)
        else:
            tmp = push_thread(name)
            self.push_threads[name] = tmp
            tmp.start()
            tmp.add_msg(s)


    def push_msg(self, name, msg):
        rpc_msg = rpc()
        rpc_msg.type = PUSH
        rpc_msg.ph.name = self.name
        if msg != "":
            rpc_msg.ph.msg = msg
        s = rpc_msg.SerializeToString()
        
        if name in self.push_threads:
            self.push_threads[name].add_msg(s)
        else:
            tmp = push_thread(name)
            self.push_threads[name] = tmp
            tmp.start()
            tmp.add_msg(s)

    def run(self):
        tin = self.pull_thread.get_msg()
        for i in range(len(tin)):
            self.dispatch(tin[i])
        tornado.ioloop.IOLoop.instance().add_timeout(time.time() + 0.1, self.run)

    def start(self, name, dispatch):
        self.name = name
        self.dispatch = dispatch
        self.pull_thread = pull_thread(self.name)
        self.pull_thread.start()
        tornado.ioloop.IOLoop.instance().add_timeout(time.time() + 0.1, self.run)
