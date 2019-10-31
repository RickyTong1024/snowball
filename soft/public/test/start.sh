#!/bin/bash

#start

cd server/out
nohup ./connect connect ../conf snowball > ../../log/connect.out 2>&1 &
nohup ./login login1 ../conf snowball > ../../log/login.out 2>&1 &
nohup ./center center ../conf snowball > ../../log/center.out 2>&1 &
nohup ./room_center room_center ../conf snowball > ../../log/room_center.out 2>&1 &
nohup ./room_master room_master1 ../conf snowball > ../../log/room_master.out 2>&1 &
nohup ./gate gate1 ../conf snowball > ../../log/gate1.out 2>&1 &
nohup ./hall hall1 ../conf snowball > ../../log/hall1.out 2>&1 &
nohup ./team team ../conf snowball > ../../log/team.out 2>&1 &
nohup ./name name ../conf snowball > ../../log/name.out 2>&1 &
nohup ./social social ../conf snowball > ../../log/social.out 2>&1 &
cd ../../login_pipe
nohup python -u login_pipe.py &
cd ../libao_pipe
nohup python -u libao_pipe.py &
cd ../back_pipe
nohup python -u back_pipe.py &
