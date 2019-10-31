#include "rpc_service.h"
#include "rpc_client.h"
#include "rpc_server.h"
#include <google/protobuf/message.h>
#include <boost/bind.hpp>
#include "rpc.pb.h"
#include "service.h"
#include "packet.h"

RpcService::RpcService()
	: request_id_(0)
	, rpc_server_(0)
	, ds_(0)
{

}

RpcService::~RpcService()
{
	
}

int RpcService::init(const std::string &name, mmg::DispathService *ds)
{
	name_ = name;
	ds_ = ds;
	std::string host = service::server_env()->get_server_value(name, "host");
	std::string port = service::server_env()->get_server_value(name, "port");
	if (host == "" || port == "")
	{
		return -1;
	}
	std::string addr = "tcp://" + host + ":" + port;
	rpc_server_ = new RpcServer(name, addr);
	timer_ = service::timer()->schedule(boost::bind(&RpcService::dispacth, this, _1), 30, "rpc_service");

	return 0;
}

int RpcService::fini()
{
	if (timer_ != -1)
	{
		service::timer()->cancel(timer_);
		timer_ = -1;
	}
	if (rpc_server_)
	{
		delete rpc_server_;
	}
	for (std::map<std::string, RpcClient *>::iterator it = rpc_client_map_.begin(); it != rpc_client_map_.end(); ++it)
	{
		RpcClient *rpc_client_ = (*it).second;
		delete rpc_client_;
	}
	rpc_client_map_.clear();

	return 0;
}


void RpcService::request(const std::string &name, Packet *packet, ResponseFunc func)
{
	rpcproto::rpc rpc;
	rpc.set_type(rpcproto::REQUESST);
	rpcproto::request *req = rpc.mutable_req();
	req->set_name(name_);
	req->set_id(request_id_);
	if (packet)
	{
		req->set_msg(packet->payload->rd_ptr(), packet->payload->length());
		delete packet;
	}

	send(name, &rpc);

	response_func_map_[request_id_] = func;
	request_id_++;
}

void RpcService::push(const std::string &name, Packet *packet)
{
	rpcproto::rpc rpc;
	rpc.set_type(rpcproto::PUSH);
	rpcproto::push *ph = rpc.mutable_ph();
	ph->set_name(name_);
	if (packet)
	{
		ph->set_msg(packet->payload->rd_ptr(), packet->payload->length());
		delete packet;
	}

	send(name, &rpc);
}

void RpcService::response(const std::string &name, int id, Packet *packet, int error_code, const std::string &error_text)
{
	rpcproto::rpc rpc;
	rpc.set_type(rpcproto::RESPONSE);
	rpcproto::response *rep = rpc.mutable_rep();
	rep->set_name(name_);
	rep->set_id(id);
	rep->set_error_code(error_code);
	if (packet)
	{
		rep->set_msg(packet->payload->rd_ptr(), packet->payload->length());
		delete packet;
	}

	send(name, &rpc);
}

void RpcService::send(const std::string &name, rpcproto::rpc *msg)
{
	RpcClient *rc = 0;
	std::map<std::string, RpcClient *>::iterator it = rpc_client_map_.find(name);
	if (it == rpc_client_map_.end())
	{
		std::string host = service::server_env()->get_server_value(name, "host");
		std::string port = service::server_env()->get_server_value(name, "port");
		if (host == "" || port == "")
		{
			return;
		}
		std::string addr = "tcp://" + host + ":" + port;
		rc = new RpcClient(name, addr);
		rpc_client_map_[name] = rc;
	}
	else
	{
		rc = (*it).second;
	}
	std::string s;
	msg->SerializeToString(&s);
	rc->putq(s);
}

int RpcService::dispacth(const ACE_Time_Value &tv)
{
	std::list<std::string> msgs;
	rpc_server_->gutq(msgs);

	if (!ds_)
	{
		return 0;
	}

	for (std::list<std::string>::iterator it = msgs.begin(); it != msgs.end(); ++it)
	{
		std::string data = *it;
		rpcproto::rpc rpc;
		rpc.ParseFromString(data);
		if (rpc.type() == rpcproto::REQUESST)
		{
			const rpcproto::request req = rpc.req();
			Packet *packet = Packet::New(req.msg());
			if (packet)
			{
				service::log()->debug("requset name = <%s> id = <%d> opcode = <%hu>", req.name().c_str(), req.id(), packet->opcode());
				ds_->dispath_req_handle(packet, req.name(), req.id());
				delete packet;
			}
		}
		else if (rpc.type() == rpcproto::PUSH)
		{
			const rpcproto::push ph = rpc.ph();
			Packet *packet = Packet::New(ph.msg());
			if (packet)
			{
				service::log()->debug("push name = <%s> opcode = <%hu>", ph.name().c_str(), packet->opcode());
				ds_->dispath_push_handle(packet, ph.name());
				delete packet;
			}
		}
		else if (rpc.type() == rpcproto::RESPONSE)
		{
			const rpcproto::response rep = rpc.rep();
			std::map<int, ResponseFunc>::iterator it = response_func_map_.find(rep.id());
			if (it != response_func_map_.end())
			{
				Packet *packet = Packet::New(rep.msg());
				if (packet)
				{
					service::log()->debug("response name = <%s> id = <%d> opcode = <%hu>", rep.name().c_str(), rep.id(), packet->opcode());
					ResponseFunc f = (*it).second;
					f(packet, rep.error_code());
					response_func_map_.erase(it);
					delete packet;
				}
			}
		}
	}
	return 0;
}
