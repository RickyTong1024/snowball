#!/usr/bin/env python  
#coding=utf-8

import sys
sys.path.append('../common')
import pymysql
import read_config

def main():
    battle_conf = read_config.db["snowball_battle"][0]
    battle_db = pymysql.connect(user=battle_conf["username"], passwd=battle_conf["password"], db=battle_conf["db"], host=battle_conf["host"], charset='utf8')
    battle_db.autocommit(1)
    battle_cur = battle_db.cursor()
    sql = "delete from battle_result_t where dt < date_sub(curdate(),interval 7 day)"
    battle_cur.execute(sql)
    battle_db.close()
    
if __name__ == '__main__':
    main()
