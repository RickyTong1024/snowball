#ifndef __RPC_CLIENT_H__
#define __RPC_CLIENT_H__

#include "typedefs.h"
#include <ace/Task.h>

class RpcClient : public ACE_Task<ACE_NULL_SYNCH>
{
public:
	RpcClient(const std::string &name, const std::string &addr);

	~RpcClient();

	void putq(const std::string &msg);

protected:
	int svc();

private:
	std::string name_;
	std::string addr_;
	std::list<std::string> msg_list_;
	bool stop_;
	ACE_Thread_Mutex chain_;
};

#endif // !__RPC_CLIENT_H__
