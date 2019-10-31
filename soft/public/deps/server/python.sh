
yum update -y
yum install gcc gcc-c++ bzip2 bzip2-devel bzip2-libs -y
yum install python-devel python-pip -y
yum install mysql mysql-devel libxml* automake cmake -y
yum install unzip zip -y

tar -zxvf Python-3.6.5.tgz
yum install -y zlib*
yum -y install zlib-devel bzip2-devel openssl-devel ncurses-devel sqlite-devel readline-devel tk-devel gdbm-devel db4-devel libpcap-devel xz-devel
chmod -R 777 Python-3.6.5
cd Python-3.6.5/
./configure --prefix=/usr/local/python3 --with-ssl
make & make install
ln -s /usr/local/python3/bin/python3 /usr/bin/python3
ln -s /usr/local/python3/bin/pip3 /usr/bin/pip3
cd ..