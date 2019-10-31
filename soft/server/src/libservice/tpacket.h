#ifndef __TPACKET_H__
#define __TPACKET_H__

#include "typedefs.h"
#include <ostream>
#include <ace/Message_Block.h>
#include <google/protobuf/message.h>

class ACE_Message_Block;

#define THD_PTR(ptr) reinterpret_cast<TPacket::Header*>((ptr)->rd_ptr ())
#define TBO_PTR(ptr) ((ptr)->rd_ptr () + sizeof(TPacket::Header))

class TPacket
{
public:
	struct Header 
	{
		Header(void) : opcode(0), size(0), compress(false) {}
		Header(uint16_t o, uint32_t s)
			: opcode(o), size(s), compress(false) {}

		bool compress;
		uint16_t opcode;
		uint32_t size;
	};

	static TPacket * New(uint16_t opcode, const char *data, int size, bool compress);
	static TPacket * New(uint16_t opcode, const std::string& data);
	static TPacket * New(uint16_t opcode, const google::protobuf::Message* message);

	/// �� message_block ����ȡһ�� Packet ���� ע�⣺ ���ƶ� message_block �Ķ�ָ��
	static TPacket * Extract(ACE_Message_Block& message_block);

	/// �� message_block �и�ʽ��һ�� ���� Packet �� Message�������ƶ� message_block �Ķ�ָ�롣
	static ACE_Message_Block * Format (ACE_Message_Block& message_block);

	static int TestSize (ACE_Message_Block& message_block);

	static uint16_t TestOpcode(ACE_Message_Block& message_block);

	static std::size_t HSize (void);

	~TPacket(void);

	static TPacket * clear(TPacket *fre);

	Header & hptr (void);

	uint16_t opcode (void);

	bool compress (void);

	/// ������Ϣ�����ĵĳ��ȣ�������Header��
	uint32_t size(void);

	/// ������Ϣ��������ָ��
	const char * body (void);

	bool do_compress ();

	/// ���صײ���Ϣ���ڴ�����, �ͷ��˶���������ӵ��Ȩ
	ACE_Message_Block * release (void);

protected:
	bool wrap(uint16_t opcode, const char * data, int len, bool compress);
	bool wrap(uint16_t opcode, const std::string& data);
	bool wrap (uint16_t opcode, const google::protobuf::Message* message);

	TPacket (void) : hp (NULL), payload (NULL) {}

public:
	Header *hp;
	ACE_Message_Block *payload;
};

#endif
