noinst_PROGRAMS = connect
connect_SOURCES =                \
    connect.cpp                  \
    connect_manager.cpp          \
    connect_message.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include

connect_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

connect_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

