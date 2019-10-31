noinst_PROGRAMS = room_center
room_center_SOURCES =                \
    room_center.cpp                  \
    room_center_dhc.cpp              \
    room_center_manager.cpp          \
    room_center_message.cpp          \
    room_center_operation.cpp        \
    sqlquery.cpp                     \
    mode_config.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include -I/usr/local/include/mysql++ -I/usr/include/mysql

room_center_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

room_center_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

