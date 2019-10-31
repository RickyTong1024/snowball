#!/bin/bash

#sys
yum update -y
yum install gcc gcc-c++ bzip2 bzip2-devel bzip2-libs -y
yum install python-devel python-pip -y
yum install mysql mysql-devel libxml* automake cmake -y
yum install unzip zip -y

cp -r updatebak /home/app
cp -r work /home/app
chmod 777 -R /home/app/updatebak

#boost
tar xvzf boost_1_55_0.tar.gz
chmod 777 -R boost_1_55_0
cd boost_1_55_0
./bootstrap.sh
./b2
./b2 install --prefix=/usr/local
cd ..

#mysql++
tar xvzf mysql++-3.2.1.tar.gz
chmod 777 -R mysql++-3.2.1
cd mysql++-3.2.1
./configure
make
make install
cd ..

#zmq
tar xvzf zeromq-4.0.4.tar.gz
chmod 777 -R zeromq-4.0.4
cd zeromq-4.0.4
./configure
make
make install
cd ..
cp zmq.hpp /usr/local/include

#ACE
tar xvzf ACE-6.2.7.tar.gz
chmod 777 -R ACE_wrappers
cd ACE_wrappers/ace
cp ../../config-linux.h config-linux.h
ln -s config-linux.h config.h
cd ../include/makeinclude
ln -s platform_linux.GNU platform_macros.GNU
cd ../..
export ACE_ROOT=/home/app/deps/server/ACE_wrappers
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$ACE_ROOT/ace
make
make install INSTALL_PREFIX=/usr/local
cd ..

#protobuf
tar xvzf protobuf-3.6.1.tar.gz
chmod 777 -R protobuf-3.6.1
cd protobuf-3.6.1
./configure
make
make install
cd python
python3 setup.py install
cd ..
cd ..

#raknet
unzip RakNet-master.zip
chmod 777 -R RakNet-master
cd RakNet-master
cmake .
make
make install
cp -R include/* /usr/local/include/
cp Lib/RakNetLibStatic/libRakNetLibStatic.a /usr/local/lib
cd ..

#python
pip3 install --upgrade pip
pip3 install tornado==4.4.3
pip3 install pyzmq
pip3 install pymysql
pip3 install protobuf==3.6.1
#pip install httplib2

#system
cp .bash_profile /home

cp common.conf /etc/ld.so.conf.d
ldconfig

#cp crontab /etc
#/bin/systemctl reload crond.service

reboot
#reboot
