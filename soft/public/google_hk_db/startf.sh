#!/bin/bash

#start

cd server/out
nohup ./room_master room_master1 ../conf snowball > ../../log/room_master.out 2>&1 &
