#!/usr/bin/env python  
#coding=utf-8

import sys

def encode(file_name):
    ifs = open(file_name, 'r+')
    content = ifs.read()
    
    p = 0
    while 1:
        index = content.find("private int ", p)
        if index == -1:
            break

        pp = index + 12
        while content[pp] != ' ' :
            pp += 1
        s = content[index + 12:pp]
        ys = "private int " + s + " = default(int);"
        hs = "private int " + s + " = default(int) ^ protocall.key1;\n    private int _" + s + " = default(int) ^ protocall.key2;"
        content = content.replace(ys, hs)
        p = index + len(hs)
        
    p = 0
    while 1:
        index = content.find("public int ", p)
        if index == -1:
            break

        pp = index + 11
        while content[pp] != '\n':
            pp += 1
        s = content[index + 11:pp]
        ys = "    public int " + s + "\n    {\n      get { return _" + s + "; }\n      set { _" + s + " = value; }\n    }"
        hs = "    public int " + s + "\n    {\n      get { int tmp = _" + s + " ^ protocall.key1; int tmp1 = __" + s + " ^ protocall.key2; if (tmp != tmp1) sys._instance.quit(); return tmp; }\n      set { _" + s + " = value ^ protocall.key1; __" + s + " = value ^ protocall.key2; }\n    }"
        content = content.replace(ys, hs)
        p = pp

    ifs.seek(0)
    ifs.write(content)
    ifs.close()

if __name__ == '__main__':
    if len(sys.argv) == 2:
        encode(sys.argv[1])
