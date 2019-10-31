#ifndef __SERVEICE_INTERFACE_H__
#define __SERVEICE_INTERFACE_H__

#include "typedefs.h"
#include <boost/function.hpp>
#include <boost/lexical_cast.hpp>
#include <boost/bind.hpp>
#include <boost/scoped_array.hpp>
#include <google/protobuf/message.h>
#include <ace/Time_Value.h>
#include <ace/Event_Handler.h>
#include "packet.h"
#include "tpacket.h"

typedef boost::function<void (Packet *packet, int error_code)> ResponseFunc;
typedef boost::function<int(const ACE_Time_Value & cur_time)> Callback;

#define BEGIN_PACKET_MAP \
	virtual void dispath_packet_handle(Packet *packet) { \
	switch (packet->opcode()) {

#define PACKET_HANDLER(opcode, handler) \
	case opcode: \
		handler(packet); \
		break;

#define PACKET_DEFAULT_HANDLE(handler) \
	default: \
		handler(packet); \
		break;

#define END_PACKET_MAP \
} \
}

#define BEGIN_PUSH_MAP \
	virtual void dispath_push_handle(Packet *packet, const std::string &name) { \
	switch (packet->opcode()) {

#define PUSH_HANDLER(opcode, handler) \
	case opcode: \
		handler(packet, name); \
		break;

#define PUSH_DEFAULT_HANDLE(handler) \
	default: \
		handler(packet, name); \
		break;

#define END_PUSH_MAP \
} \
}

#define BEGIN_REQ_MAP \
	virtual void dispath_req_handle(Packet *packet, const std::string &name, int id) { \
	switch (packet->opcode()) {

#define REQ_HANDLER(opcode, handler) \
	case opcode: \
		handler(packet, name, id); \
		break;

#define END_REQ_MAP \
} \
}


enum opcmd_t {
	opc_insert = 1,   /// 插入实体
	opc_query = 2,   /// 查询实体 
	opc_update = 3,   /// 更新实体 
	opc_remove = 4,   /// 删除实体 
};

class Request
{
public:
	Request(void)
		: op_(opc_insert)
		, guid_(0)
		, data_(0)
		, res_(0)
	{

	}

	~Request(void)
	{
		if (data_)
		{
			delete data_;
			data_ = 0;
		}
	}

	void add(opcmd_t op, uint64_t guid, google::protobuf::Message *data)
	{
		op_ = op;
		guid_ = guid;
		data_ = data;
	}

	void set_result(int res)
	{
		res_ = res;
	}

	opcmd_t op() const { return op_; }

	uint64_t guid() const { return guid_; }

	google::protobuf::Message *data() const { return data_; }

	google::protobuf::Message *release_data() { google::protobuf::Message *data = data_; data_ = 0; return data; }

	int result() const { return res_; }

private:
	opcmd_t op_;
	uint64_t guid_;
	google::protobuf::Message *data_;
	int res_;
};

typedef boost::function<void(Request *)> Upcaller;

class Packet;

namespace mysqlpp
{
	class Connection;
}

class DBCFile;
class OBSFile;

namespace mmg {

	class DispathService
	{
	public:
		virtual void dispath_push_handle(Packet *packet, const std::string &name) = 0;

		virtual void dispath_req_handle(Packet *packet, const std::string &name, int id) = 0;

		virtual void dispath_packet_handle(Packet *packet) = 0;
	};

	class RpcService
	{
	public:
		virtual void request(const std::string &name, Packet *packet, ResponseFunc func) = 0;

		virtual void push(const std::string &name, Packet *packet) = 0;

		virtual void response(const std::string &name, int id, Packet *packet, int error_code = 0, const std::string &error_text = "") = 0;
	};

	class Timer
	{
	public:
		virtual int start() = 0;

		virtual int schedule(Callback task, int expire, const std::string &name) = 0;

		virtual int cancel(int expiry_id) = 0;

		virtual uint64_t now(void) = 0;

		virtual int hour(void) = 0;

		virtual int weekday(void) = 0;

		virtual int day(void) = 0;

		virtual int month(void) = 0;

		virtual bool trigger_time(uint64_t old_time, int hour, int minute) = 0;

		virtual bool trigger_week_time(uint64_t old_time) = 0;

		virtual bool trigger_month_time(uint64_t old_time) = 0;

		virtual int run_day(uint64_t old_time) = 0;
	};

	class ServerEnv
	{
	public:
		virtual std::string get_server_value(const std::string &name, const std::string &key) = 0;

		virtual void get_server_names(const std::string &kind, std::vector<std::string> &names) = 0;

		virtual void get_server_kinds(std::vector<std::string> &kinds) = 0;

		virtual std::string get_db_value(const std::string &name, const std::string &key) = 0;

		virtual void get_db_names(const std::string &kind, std::vector<std::string> &names) = 0;

		virtual void get_db_kinds(std::vector<std::string> &kinds) = 0;

		virtual std::string get_game_value(const std::string &key) = 0;
	};

	class TcpService
	{
	public:
		virtual void send_msg(int hid, TPacket *pck) = 0;

		virtual void destory(int hid) = 0;
	};

	class UdpService
	{
	public:
		virtual void send_msg(int hid, TPacket *pck) = 0;

		virtual void destory(int hid) = 0;
	};

	class LogicService
	{
	public:
		virtual int add_msg(Packet *pck) = 0;
	};

	class DhcRequest
	{
	public:
		virtual int do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data) = 0;

		virtual int do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data) = 0;

		virtual int do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data) = 0;

		virtual int do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data) = 0;
	};

	class Pool
	{
	public:
		enum estatus
		{
			state_none,
			state_new,
			state_update,
		};

		template<typename OBJECT>
		OBJECT * object(uint64_t guid)
		{
			return dynamic_cast<OBJECT*>(get(guid));
		}

		virtual int add(uint64_t guid, google::protobuf::Message *entity, estatus es) = 0;

		virtual int remove(uint64_t guid, uint64_t ref_guid) = 0;

		virtual google::protobuf::Message * release(uint64_t guid) = 0;

		virtual google::protobuf::Message * get(uint64_t guid) = 0;

		virtual estatus get_state(uint64_t guid) = 0;

		virtual void set_state(uint64_t guid, estatus es) = 0;

		virtual void get_entitys(int type, std::vector<uint64_t> &guids) = 0;

		virtual void release_ref(uint64_t ref_guid, std::vector<uint64_t> &entitys) = 0;
	};

	class Log
	{
	public:
		virtual int debug(const char *text, ...) = 0;

		virtual int error(const char *text, ...) = 0;

		virtual void enable_file(const std::string &file) = 0;
	};

	class Scheme
	{
	public:
		virtual int init() = 0;

		virtual int fini() = 0;

		virtual DBCFile * get_dbc(const std::string &name, bool reset = false) = 0;

		virtual int read_file(const std::string &name, std::string &res, bool reset = false) = 0;

		virtual int search_illword(const std::string& text) = 0;

		virtual void change_illword(std::string& text) = 0;

		virtual std::string get_server_str(const char * sstr, ...) = 0;

		virtual std::string get_lang_str(std::string str) = 0;
	};
}

#endif // !__RPC_INTERFACE_H__
