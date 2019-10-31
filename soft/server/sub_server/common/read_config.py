#!/usr/bin/env python  
#coding=utf-8

import json

server_conf = "../../conf/server.json"

f = open(server_conf, "r")
s = f.read()
f.close()
conf = json.loads(s)
servers = {}
for k in conf["server"]:
    t = conf["server"][k]
    for i in range(len(t)):
        servers[t[i]["id"]] = t[i]

db = {}
for k in conf["db"]:
    t = conf["db"][k]
    db[k] = []
    for i in range(len(t)):
        db[k].append(t[i])
