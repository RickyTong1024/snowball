noinst_PROGRAMS = login
login_SOURCES =                \
    login.cpp                  \
    login_dhc.cpp              \
    login_manager.cpp          \
    login_message.cpp          \
    sqlquery2.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include -I/usr/local/include/mysql++ -I/usr/include/mysql

login_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

login_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

