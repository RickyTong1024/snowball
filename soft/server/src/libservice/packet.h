#ifndef __PACKET_H__
#define __PACKET_H__

#include "typedefs.h"
#include <ostream>
#include <ace/Message_Block.h>
#include <google/protobuf/message.h>

class ACE_Message_Block;

#define HD_PTR(ptr) reinterpret_cast<Packet::Header*>((ptr)->rd_ptr ())
#define BO_PTR(ptr) ((ptr)->rd_ptr () + sizeof(Packet::Header))

class Packet
{
public:
	struct Header 
	{
		Header (void) : opcode (0), guid (0L), size (0), compress (false), hid(0) {}
		Header (uint16_t o, uint64_t g, uint32_t s, int h)
			: opcode (o), guid (g), size (s), compress (false), hid(h) {}

		bool compress;
		uint16_t opcode;   /// 消息操作码
		int32_t hid;
		uint32_t size;     /// 消息正文大小
		uint64_t guid;     /// 需要发往目的端的GUID
	};

	static ACE_Message_Block * Message (uint16_t opcode, int hid, uint64_t guid, const std::string& data);

	static Packet * New(uint16_t opcode, int hid, uint64_t guid, const char *data, int size, bool compress);
	static Packet * New (uint16_t opcode, int hid, uint64_t guid, const std::string& data);
	static Packet * New (uint16_t opcode, int hid, uint64_t guid, const google::protobuf::Message* message);
	static Packet * New(const std::string& data);
	static Packet * New (ACE_Message_Block* message);

	/// 从 message_block 中提取一个 Packet 对象， 注意： 会移动 message_block 的读指针
	static Packet * Extract (ACE_Message_Block& message_block);

	/// 从 message_block 中格式化一个 含有 Packet 的 Message，并且移动 message_block 的读指针。
	static ACE_Message_Block * Format (ACE_Message_Block& message_block);

	static int TestSize (ACE_Message_Block& message_block);

	static std::size_t HSize (void);

	~Packet (void);

	static Packet * clear (Packet *fre);

	Header & hptr (void);

	uint16_t opcode (void);

	uint64_t guid (void);

	bool compress (void);

	/// 返回消息包正文的长度（不包含Header）
	uint32_t size (void);

	int hid (void);

	/// 返回消息包的正文指针
	const char * body (void);

	Packet * clone (void);

	bool do_compress ();

	/// 解析消息
	google::protobuf::Message * parse_protocol (google::protobuf::Message& msg) const;

	/// 返回底层消息的内存容器, 释放了对于容器的拥有权
	ACE_Message_Block * release (void);

	/// 返回下一个消息包
	Packet * next (void);

	/// 添加消息包到消息链尾部
	Packet * add_tail (Packet* pck);

protected:
	bool wrap(uint16_t opcode, int hid, uint64_t guid, const char * data, int len, bool compress);
	bool wrap (uint16_t opcode, int hid, uint64_t guid, const std::string& data);
	bool wrap (uint16_t opcode, int hid, uint64_t guid, const google::protobuf::Message* message);

	Packet (void) : hp (NULL), next_ (NULL), payload (NULL) {}

public:
	Packet *next_;

	Header *hp;
	ACE_Message_Block *payload;
};

#endif
