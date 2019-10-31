noinst_PROGRAMS = center
center_SOURCES =                \
    center.cpp                  \
    center_manager.cpp          \
    center_message.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include

center_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

center_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

