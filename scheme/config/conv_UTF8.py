#! /usr/bin/env python
#coding=utf-8

import os
from shutil import copy

fdirs = [
    "../../soft/client/Assets/Resources/config/",
    "../../soft/server/config/"
    ]
mission_fdirs = [
    "../../soft/client/Assets/Resources/config/",
    ]

def conv_UTF8():
    for i in range(len(fdirs)):
        if not os.path.exists(fdirs[i]):
            os.makedirs(fdirs[i])

    files = os.listdir ('./')
    for f in files:
        p = os.path.splitext (f)
        if len(p) > 1 and p[1] == '.txt':
            print(f)
            ifs = open(f,'rb')
            try:
                content = ifs.read ().decode('gbk').encode('utf8')
                ifs.close()

                if len (content) > 0:
                    for i in range(len(fdirs)):       
                        try:
                            ofs = open(fdirs[i] + f, 'wb')
                            ofs.write(content)
                        finally:
                            ofs.close()
            finally:
                ifs.close ()
        elif len(p) > 1 and p[1] == '.xml':
            print(f)
            copy(f, fdir + f)
            copy(f, fdira + f)
            copy(f, fdir1 + f)

    for i in range(len(mission_fdirs)):
        if not os.path.exists(mission_fdirs[i] + "mission/"):
            os.makedirs(mission_fdirs[i])
    files = os.listdir ('./mission')
    for f in files:
        p = os.path.splitext (f)
        if len(p) > 1 and p[1] == '.txt':
            f = 'mission/' + f
            print(f)
            ifs = open(f,'rb')
            try:
                content = ifs.read ().decode('gbk').encode('utf8')
                ifs.close()

                for i in range(len(mission_fdirs)):       
                    try:
                        ofs = open(mission_fdirs[i] + f, 'wb')
                        ofs.write(content)
                    finally:
                        ofs.close()
            finally:
                ifs.close ()

if __name__ == '__main__':
    conv_UTF8 ()

