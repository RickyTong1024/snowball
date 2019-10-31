noinst_PROGRAMS = social
social_SOURCES =                \
    social.cpp                  \
    social_dhc.cpp              \
    social_manager.cpp          \
    social_message.cpp          \
    sqlquery.cpp                \
    sqlquery2.cpp               \
    social_pool.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include -I/usr/local/include/mysql++ -I/usr/include/mysql

social_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

social_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

