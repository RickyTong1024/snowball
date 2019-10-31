noinst_PROGRAMS = hall
hall_SOURCES =                \
    chat_manager.cpp          \
    chat_pool.cpp             \
    hall.cpp                  \
    hall_dhc.cpp              \
    hall_manager.cpp          \
    hall_message.cpp          \
    hall_pool.cpp             \
    item_config.cpp           \
    item_manager.cpp          \
    item_operation.cpp        \
    player_config.cpp         \
    player_loader.cpp         \
    player_manager.cpp        \
    player_operation.cpp      \
    post_manager.cpp          \
    post_operation.cpp        \
	rank_config.cpp           \
    rank_manager.cpp          \
    rank_operation.cpp        \
    reward.cpp                \
    role_config.cpp           \
    role_manager.cpp          \
    role_operation.cpp        \
    sqlquery.cpp              \
    sqlquery2.cpp             \
    social_pool.cpp           \
    social_manager.cpp        \
    team_manager.cpp

INCLUDES = -I$(top_builddir)/src/libservice -I$(top_builddir)/src/libprotocol -I/usr/local/include -I/usr/local/include/mysql++ -I/usr/include/mysql

hall_LDFLAGS = -lpthread -ldl -lrt -lmysqlpp -lACE -lboost_thread -lboost_date_time -lboost_filesystem -lprotobuf -lzmq -lz

hall_LDADD = $(top_builddir)/src/libservice/libservice.a $(top_builddir)/src/libprotocol/libprotocol.a /usr/local/lib/libRakNetLibStatic.a

