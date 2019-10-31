#!/usr/bin/env python  
#coding=utf-8

import sys
sys.path.append('../common')
import pymysql
import read_config

def main():
    name_conf = read_config.db["snowball_social"][0]
    name_db = pymysql.connect(user=name_conf["username"], passwd=name_conf["password"], db=name_conf["db"], host=name_conf["host"], charset='utf8')
    name_db.autocommit(1)
    name_cur = name_db.cursor()
    sql = "delete from name_t"
    name_cur.execute(sql)
    sql1 = "insert into name_t (guid, name) values (%s, %s)"

    for index in range(len(read_config.db["snowball_player"])):
        player_conf = read_config.db["snowball_player"][index]
        player_db = pymysql.connect(user=player_conf["username"], passwd=player_conf["password"], db=player_conf["db"], host=player_conf["host"], charset='utf8')
        player_db.autocommit(1)
        player_cur = player_db.cursor()
        start = 0
        while 1:
            sql = "select guid, name from player_t limit %s, 1000"
            param = (start,)
            player_cur.execute(sql, param)
            res = player_cur.fetchall()
            params = []
            for i in range(len(res)):
                params.append((res[i][0], res[i][1],))
            name_cur.executemany(sql1, params)
            start = start + 1000
            if len(res) != 1000:
                break
        player_db.close()
    name_db.close() 
    
if __name__ == '__main__':
    main()
