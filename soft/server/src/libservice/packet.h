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
		uint16_t opcode;   /// ��Ϣ������
		int32_t hid;
		uint32_t size;     /// ��Ϣ���Ĵ�С
		uint64_t guid;     /// ��Ҫ����Ŀ�Ķ˵�GUID
	};

	static ACE_Message_Block * Message (uint16_t opcode, int hid, uint64_t guid, const std::string& data);

	static Packet * New(uint16_t opcode, int hid, uint64_t guid, const char *data, int size, bool compress);
	static Packet * New (uint16_t opcode, int hid, uint64_t guid, const std::string& data);
	static Packet * New (uint16_t opcode, int hid, uint64_t guid, const google::protobuf::Message* message);
	static Packet * New(const std::string& data);
	static Packet * New (ACE_Message_Block* message);

	/// �� message_block ����ȡһ�� Packet ���� ע�⣺ ���ƶ� message_block �Ķ�ָ��
	static Packet * Extract (ACE_Message_Block& message_block);

	/// �� message_block �и�ʽ��һ�� ���� Packet �� Message�������ƶ� message_block �Ķ�ָ�롣
	static ACE_Message_Block * Format (ACE_Message_Block& message_block);

	static int TestSize (ACE_Message_Block& message_block);

	static std::size_t HSize (void);

	~Packet (void);

	static Packet * clear (Packet *fre);

	Header & hptr (void);

	uint16_t opcode (void);

	uint64_t guid (void);

	bool compress (void);

	/// ������Ϣ�����ĵĳ��ȣ�������Header��
	uint32_t size (void);

	int hid (void);

	/// ������Ϣ��������ָ��
	const char * body (void);

	Packet * clone (void);

	bool do_compress ();

	/// ������Ϣ
	google::protobuf::Message * parse_protocol (google::protobuf::Message& msg) const;

	/// ���صײ���Ϣ���ڴ�����, �ͷ��˶���������ӵ��Ȩ
	ACE_Message_Block * release (void);

	/// ������һ����Ϣ��
	Packet * next (void);

	/// �����Ϣ������Ϣ��β��
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
