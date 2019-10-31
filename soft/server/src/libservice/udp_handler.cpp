#include "udp_handler.h"
#include "udp_service.h"
#include "service.h"
#include "packet.h"
#include "RakPeerInterface.h"
#include "MessageIdentifiers.h"
#include "BitStream.h"
#include "service.h"

UdpHandler::UdpHandler(int hid, const std::string &addr, UdpService *us)
: udp_service_(us)
, hid_(hid)
, chunk_(RECV_BUFSIZE)
, addr_(addr)
{
}

UdpHandler::~UdpHandler()
{
}

void UdpHandler::onconnect(const std::string &addr)
{
	service::log()->debug("udp connect from %s", addr.c_str());
	Packet *pck = Packet::New(30003, hid_, 0, "");
	service::logic_service()->add_msg(pck);
}

void UdpHandler::send(ACE_Message_Block *mb)
{
	int pos = 0;
	int len = mb->length();
	while (len > 0)
	{
		int num = 500;
		if (num > len)
		{
			num = len;
		}
		RakNet::BitStream stream;
		stream.Write((RakNet::MessageID)ID_USER_PACKET_ENUM);
		stream.Write(mb->rd_ptr(), num);
		RakNet::SystemAddress rsa(addr_.c_str());
		udp_service_->get_peer()->Send(&stream, HIGH_PRIORITY, RELIABLE_ORDERED, 0, rsa, false);
		pos += num;
		len -= num;
		mb->rd_ptr(num);
	}
}

void UdpHandler::kick(bool is_send)
{
	Packet *pck = Packet::New(30004, hid_, 0, "");
	service::logic_service()->add_msg(pck);
	udp_service_->del_handler(hid_);
	RakNet::SystemAddress rsa(addr_.c_str());
	udp_service_->get_peer()->CloseConnection(rsa, is_send);
	delete this;
}

int UdpHandler::recv(const char *data, int len)
{
	if (len > chunk_.space())
	{
		service::log()->error("kick chunk space");
		this->kick();
		return -1;
	}
	//service::log()->debug("len = %d", len);
	chunk_.copy(data, len);
	int res = feed(chunk_);
	if (res == -1)
	{
		service::log()->error("kick feed -1");
		this->kick();
		return -1;
	}
	chunk_.crunch();
	return 0;
}

//////////////////////////////////////////////////////////////////////////

int UdpHandler::feed(ACE_Message_Block &chunk)
{
	do
	{
		int size = TPacket::TestSize(chunk_);
		if (size > MAX_PCK_SIZE)
		{
			service::log()->error("pck too large");
			return -1;
		}
		TPacket *pck = TPacket::Extract(chunk_);
		if (!pck)
		{
			//uint16_t op = TPacket::TestOpcode(chunk_);
			//service::log()->error("!pck op = %d, size = %d", (int)op, size);
			break;
		}
		Packet *pck1 = Packet::New(pck->opcode(), hid_, 0, pck->body(), pck->size(), pck->compress());
		delete pck;
		service::logic_service()->add_msg(pck1);
	} while (1);

	return 0;
}
