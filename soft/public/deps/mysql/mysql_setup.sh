#!/bin/bash

#sys
yum update -y
yum install gcc gcc-c++ bzip2 bzip2-devel bzip2-libs -y
yum install python-devel python-pip -y
yum install mysql mysql-devel libxml* -y

#mysql
rpm -ivh mysql57-community-release-el7-11.noarch.rpm
yum install mysql-community-server
yum install mysql-community-devel
cp my.cnf /etc
mkdir /home/app/mysql
mkdir /home/app/mysqldata
chmod -R 777 /home
systemctl enable mysqld
systemctl start mysqld
mysql -uroot -proot << EOF
grant all privileges on *.* to 'root'@'127.0.0.1' identified by 'root';
grant all privileges on *.* to 'root'@'localhost' identified by 'root';
grant all privileges on *.* to 'root'@'%' identified by '1qaz2wsx@39299911';
flush privileges;
create database snowball_gtool default charset utf8 collate utf8_general_ci;
create database snowball_account default charset utf8 collate utf8_general_ci;
create database snowball_battle default charset utf8 collate utf8_general_ci;
create database snowball_social default charset utf8 collate utf8_general_ci;
create database snowball_player1 default charset utf8 collate utf8_general_ci;
create database snowball_player2 default charset utf8 collate utf8_general_ci;
create database snowball_player3 default charset utf8 collate utf8_general_ci;
create database snowball_player4 default charset utf8 collate utf8_general_ci;
create database snowball_player5 default charset utf8 collate utf8_general_ci;
create database snowball_player6 default charset utf8 collate utf8_general_ci;
create database snowball_player7 default charset utf8 collate utf8_general_ci;
create database snowball_player8 default charset utf8 collate utf8_general_ci;
create database snowball_player9 default charset utf8 collate utf8_general_ci;
create database snowball_player10 default charset utf8 collate utf8_general_ci;
create database snowball_player11 default charset utf8 collate utf8_general_ci;
create database snowball_player12 default charset utf8 collate utf8_general_ci;
create database snowball_player13 default charset utf8 collate utf8_general_ci;
create database snowball_player14 default charset utf8 collate utf8_general_ci;
create database snowball_player15 default charset utf8 collate utf8_general_ci;
create database snowball_player16 default charset utf8 collate utf8_general_ci;
create database snowball_player17 default charset utf8 collate utf8_general_ci;
create database snowball_player18 default charset utf8 collate utf8_general_ci;
create database snowball_player19 default charset utf8 collate utf8_general_ci;
create database snowball_player20 default charset utf8 collate utf8_general_ci;
EOF

cp -r mysqlbak /home/app
chmod 777 -R /home/app/mysqlbak

cp crontab /etc/crontab
/etc/rc.d/init.d/crond restart

#reboot
reboot
