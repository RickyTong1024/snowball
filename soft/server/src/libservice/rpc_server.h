#ifndef __RPC_SERVER_H__
#define __RPC_SERVER_H__

#include "typedefs.h"
#include <ace/Task.h>

class RpcServer : public ACE_Task<ACE_NULL_SYNCH>
{
public:
	RpcServer(const std::string &name, const std::string &addr);

	~RpcServer();

	void gutq(std::list<std::string> &msgs);

protected:
	int svc();

private:
	std::string name_;
	std::string addr_;
	std::list<std::string> msg_list_;
	bool stop_;
	ACE_Thread_Mutex chain_;
};

#endif // !__RPC_SERVER_H__
