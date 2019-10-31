#! /usr/bin/env python
#coding=utf-8

from PIL import Image
import sys
import os

def do_alpha(s):
    ss = s + '.png'
    imp = Image.open(ss)
    imp.load()
    r, g, b, a = imp.split()
    sss = s + '_a.png'
    a.save(sss)
    print "success"

if __name__ == '__main__':
    for i in range(1, len(sys.argv)):
        s = sys.argv[i]
        print s
        if len(s) < 4:
            continue
        if s[len(s) - 4:] != '.png':
            continue
        s = s[:len(s) - 4]
        do_alpha(s)
        
    os.system("pause")
