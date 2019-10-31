noinst_PROGRAMS = room
room_SOURCES =                \
    room.cpp                  \
    room_config.cpp           \
    room_manager.cpp          \
    room_message.cpp          \
    room_pool.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include

room_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

room_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

