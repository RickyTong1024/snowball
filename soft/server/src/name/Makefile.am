noinst_PROGRAMS = name
name_SOURCES =                \
    name.cpp                  \
    name_dhc.cpp              \
    name_manager.cpp          \
    name_message.cpp          \
	sqlquery2.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include -I/usr/local/include/mysql++ -I/usr/include/mysql

name_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

name_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

