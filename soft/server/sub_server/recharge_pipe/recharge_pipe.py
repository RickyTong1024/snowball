#!/usr/bin/env python  
#coding=utf-8

import sys
sys.path.append('../common')
import tornado.httpserver
import tornado.ioloop
import tornado.web
import sys
import pull
from msg_pipe_pb2 import *
import packet
import json
import config
import dispatch

class recharge_ali_handler(tornado.web.RequestHandler):

    @tornado.web.asynchronous
    def post(self):
        data = json.loads(self.request.body)
        guid = int(data['guid'])
        rid = int(data['rid'])
        orderno = data['orderno']
        amount = float(data['amount'])

        print("guid", guid, "rid", rid, "orderno", orderno, "amount", amount)
        
        msg = pmsg_recharge_ali()
        msg.guid = guid
        msg.rid = rid
        msg.orderno = orderno
        msg.amount = amount
        data = msg.SerializeToString()
        data = packet.to_pck_string(10009, data)
        pull.ioloop.instance().push_msg("center", data)
        self.finish()

class Application(tornado.web.Application):
    def __init__(self):  
        handlers = [
            (r"/recharge_ali", recharge_ali_handler),
        ]
        tornado.web.Application.__init__(self, handlers)
        
def main():
    pull.ioloop.instance().start(config.myname, dispatch.dispatch)
    http_server = tornado.httpserver.HTTPServer(Application())
    http_server.listen(40002)
    print('Welcome to the machine...')
    tornado.ioloop.IOLoop.instance().start()
            
if __name__ == '__main__':
    main()
