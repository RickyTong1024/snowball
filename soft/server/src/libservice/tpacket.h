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

	/// 从 message_block 中提取一个 Packet 对象， 注意： 会移动 message_block 的读指针
	static TPacket * Extract(ACE_Message_Block& message_block);

	/// 从 message_block 中格式化一个 含有 Packet 的 Message，并且移动 message_block 的读指针。
	static ACE_Message_Block * Format (ACE_Message_Block& message_block);

	static int TestSize (ACE_Message_Block& message_block);

	static uint16_t TestOpcode(ACE_Message_Block& message_block);

	static std::size_t HSize (void);

	~TPacket(void);

	static TPacket * clear(TPacket *fre);

	Header & hptr (void);

	uint16_t opcode (void);

	bool compress (void);

	/// 返回消息包正文的长度（不包含Header）
	uint32_t size(void);

	/// 返回消息包的正文指针
	const char * body (void);

	bool do_compress ();

	/// 返回底层消息的内存容器, 释放了对于容器的拥有权
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
