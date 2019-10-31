noinst_PROGRAMS = gate
gate_SOURCES =                \
    gate.cpp                  \
    gate_manager.cpp          \
    gate_message.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include

gate_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

gate_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

