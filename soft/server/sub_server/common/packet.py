#!/usr/bin/env python  
#coding=utf-8

import struct
import zlib

def to_pck_string(opcode, data):
    return struct.pack('?HiIQ%ss' % (len(data)), False, opcode, 0, len(data), 0, data)

def to_pck_data(data):
    comp, opcode, hid, size, guid, s = struct.unpack('?HiIQ%ss' % (len(data) - 24), data)
    if comp:
        s = zlib.decompress(s)
    return opcode, s
    
