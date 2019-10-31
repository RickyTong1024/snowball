#!/usr/bin/env python  
#coding=utf-8

import sys

ofile_path = '..\..\server\src\libprotocol\opcodes.h'

def main(python_path):
    ofile = open(ofile_path, "r")
    opy = open(python_path, "w")
    opy.write('opcodes = {\n')
    l = 0
    for line in ofile:
        index = line.find(',')
        if index != -1:
            index2 = line.find('//')
            if index2 != -1:
                line = line[:index2]
            index1 = line.find('=')
            if index1 != -1:
                ss = line.split('=')
                line = ss[0]
                s = ss[1]
                s = s.replace(',', '')
                s = s.replace(' ', '')
                s = s.replace('\t', '')
                s = s.replace('\n', '')
                s = s.replace('\r', '')
                l = int(s)
            else:
                l = l + 1
            line = line.replace(',', '')
            line = line.replace(' ', '')
            line = line.replace('\t', '')
            line = line.replace('\n', '')
            line = line.replace('\r', '')
            if line == "":
                continue
            print(line, l)
            opy.write('\t"%s" : %d,\n' % (line, l))
    opy.write('}\n')
    opy.write('\n')
    opy.write('def op(s):\n')
    opy.write('    return opcodes[s]\n')
    opy.write('\n')
    opy.write('def op_url(s):\n')
    opy.write('    return "/" + str(op(s))\n')
    ofile.close()
    opy.close()
    

if __name__ == '__main__':
    if len(sys.argv) >= 2:
        for i in range(len(sys.argv) - 1):
            python_path = sys.argv[i + 1]
            main(python_path)
