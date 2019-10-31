#!/usr/bin/env python
# coding=utf-8

import time
import config
from msg_pipe_pb2 import *
import json
from tornado.httpclient import AsyncHTTPClient, HTTPRequest
import pull
import packet
import urllib.parse
import datetime
import opcodes


def dispatch(tin):
    name = tin[0]
    rid = tin[1]
    opcode = tin[2]
    msg = tin[3]

    if opcode == opcodes.opcodes['REQ_HALL_CENTER_RECHARGE']:
        inmsg = pmsg_req_recharge()
        inmsg.ParseFromString(msg)
        if(inmsg.pt == 'google'):
            recharge_google(name, rid, inmsg.code)
        elif(inmsg.pt == 'ios_yymoon'):
            recharge_apple(name, rid, inmsg.code[0])
        else:
            print('wrong platform')


def recharge_apple(name, rid, code):
    verify_apple(code, False, name, rid)


def verify_apple(code, issandbox, name, rid):
    host = "https://sandbox.itunes.apple.com/verifyReceipt" if issandbox else "https://buy.itunes.apple.com/verifyReceipt"
    httpClient = AsyncHTTPClient()
    headers = {"Content-type": "application/json"}
    body = json.dumps({"receipt-data": code}).encode('utf8')
    req = HTTPRequest(url=host, method="POST", headers=headers, body=body, validate_cert=False)
    httpClient.fetch(req, callback=lambda response: verify_apple_callback(response, code, issandbox, name, rid))


def verify_apple_callback(response, code, issandbox, name, rid):
    res = -1
    orderid = ""
    product_id = ""
    if response.error:
        print(response.error)
        apple_response(-1, "", "", name, rid)
        return
    if response.code == 200:
        print(response.body)
        content = json.loads(response.body)
        res = content['status']
        if res == 0:
            orderid = content["receipt"]["transaction_id"]
            product_id = content["receipt"]["product_id"]
    else:
        apple_response(-1, "", "", name, rid)
        return

    if res != 0 and issandbox == False:
        verify_apple(code, True, name, rid)
        return

    apple_response(res, orderid, product_id, name, rid)


def apple_response(res, orderid, product_id, name, rid):
    outmsg = pmsg_rep_recharge()
    outmsg.res = res
    outmsg.orderid = orderid
    outmsg.product_id = product_id
    s = outmsg.SerializeToString()
    data = packet.to_pck_string(20006, s)
    pull.ioloop.instance().response_msg(name, rid, data)


# google
client_id = '261628343777-1pl8fpuqm1nrramka10a5k5hlk4vqdmi.apps.googleusercontent.com'
client_secret = 'BSFRBfZyHU_Y7n9wX6-cmid7'
access_token = None
refresh_token = '1/mBHzLgNBfbT2WZ9NnNt4IM3vpI-z_CoZkn7wY1YYb_I'
access_token_expire_time = None


def recharge_google(name, rid, code):
    packageName = code[0]
    productId = code[1]
    purchase_token = code[2]

    if not packageName or not productId or not purchase_token:
        google_response(-1, "", "", name, rid)
        return

    if packageName != "com.yymoon.snowball":
        google_response(-1, "", "", name, rid)
        return

    get_token_google(packageName, productId, purchase_token, name, rid)


def get_token_google(packageName, productId, purchase_token, name, rid):
    need_get_access_token = False
    if access_token:
        now = datetime.datetime.now()
        if now >= access_token_expire_time:
            need_get_access_token = True
    else:
        need_get_access_token = True

    if not need_get_access_token:
        verify_google(packageName, productId, purchase_token, name, rid)
        return

    base_url = 'https://accounts.google.com/o/oauth2/token'
    data = dict(
        grant_type='refresh_token',
        client_id=client_id,
        client_secret=client_secret,
        refresh_token=refresh_token,
    )
    httpClient = AsyncHTTPClient()
    headers = {"Content-type": "application/x-www-form-urlencoded"}
    body = urllib.parse.urlencode(data)
    req = HTTPRequest(url=base_url, method="POST", headers=headers, body=body, validate_cert=False)
    httpClient.fetch(req, callback=lambda response: get_token_google_callback(
        response, packageName, productId, purchase_token, name, rid))


def get_token_google_callback(response, packageName, productId, purchase_token, name, rid):
    if response.error:
        print(response.error)
        google_response(-1, "", "", name, rid)
        return
    if response.code == 200:
        print(response.body)
        jdata = json.loads(response.body)
    else:
        google_response(-1, "", "", name, rid)
        return

    if 'access_token' in jdata:
        global access_token
        global access_token_expire_time
        access_token = jdata['access_token']
        access_token_expire_time = datetime.datetime.now() + datetime.timedelta(
            seconds=jdata['expires_in'] * 2 / 3
        )
        verify_google(packageName, productId, purchase_token, name, rid)
    else:
        google_response(-1, "", "", name, rid)
        return


def verify_google(packageName, productId, purchase_token, name, rid):
    url_fmt = 'https://www.googleapis.com/androidpublisher/v2/applications/{packageName}/purchases/products/{productId}/tokens/{token}'
    url = url_fmt.format(packageName=packageName,
                         productId=productId,
                         token=purchase_token)
    params = {"access_token": access_token}
    url += "?" + urllib.urlencode(params)
    httpClient = AsyncHTTPClient()
    httpClient.fetch(url, callback=lambda response: verify_google_callback(response, productId, name, rid))


def verify_google_callback(response, productId, name, rid):
    res = -1
    orderid = ""
    if response.error:
        prin(response.error)
        google_response(-1, "", "", name, rid)
        return
    if response.code == 200:
        print(response.body)
        gdata = json.loads(response.body)
        if not isinstance(gdata, dict):
            google_response(-1, "", "", name, rid)
            return

        if gdata.get("purchaseState", None) != 0:
            google_response(-1, "", "", name, rid)
            return

        if gdata.get("purchaseTimeMillis", None) is None:
            google_response(-1, "", "", name, rid)
            return

        orderid = str(gdata.get("purchaseTimeMillis"))
        google_response(0, orderid, productId, name, rid)
    else:
        google_response(-1, "", "", name, rid)
        return


def google_response(res, orderid, product_id, name, rid):
    outmsg = pmsg_rep_recharge()
    outmsg.res = res
    outmsg.orderid = orderid
    outmsg.product_id = product_id
    s = outmsg.SerializeToString()
    data = packet.to_pck_string(20007, s)
    pull.ioloop.instance().response_msg(name, rid, data)
