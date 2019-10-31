#!/bin/bash

#stop

kill $(ps -ef | grep './connect connect'|awk '{print $2}')
kill $(ps -ef | grep './login login'|awk '{print $2}')
kill $(ps -ef | grep './center center'|awk '{print $2}')
kill $(ps -ef | grep './room_center room_center'|awk '{print $2}')
kill $(ps -ef | grep './room_master room_master'|awk '{print $2}')
kill $(ps -ef | grep './gate gate'|awk '{print $2}')
kill $(ps -ef | grep './hall hall'|awk '{print $2}')
kill $(ps -ef | grep './room room'|awk '{print $2}')
kill $(ps -ef | grep './team team'|awk '{print $2}')
kill $(ps -ef | grep './name name'|awk '{print $2}')
kill $(ps -ef | grep './social social'|awk '{print $2}')
kill $(ps -ef | grep './rank rank'|awk '{print $2}')
kill $(ps -ef | grep 'python -u login_pipe'|awk '{print $2}')
kill $(ps -ef | grep 'python -u back_pipe'|awk '{print $2}')
kill $(ps -ef | grep 'python -u libao_pipe'|awk '{print $2}')
kill $(ps -ef | grep 'python -u recharge_pipe'|awk '{print $2}')
