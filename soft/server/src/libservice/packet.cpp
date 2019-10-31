#include "packet.h"
#include <google/protobuf/io/zero_copy_stream_impl.h>
#include <google/protobuf/io/gzip_stream.h>

/// 最大包长度: 64KB
#define MAX_PACKET_SIZE (64 * 1024)
#define COMPRESS_SIZE 1024

ACE_Message_Block * Packet::Message(uint16_t opcode, int hid, uint64_t guid, const std::string& data)
{
	Packet tmp;
	return tmp.wrap(opcode, hid, guid, data) ? tmp.release () : 0;
}

Packet * Packet::New(uint16_t opcode, int hid, uint64_t guid, const char *data, int size, bool compress)
{
	Packet *result = new Packet;

	if (!result->wrap(opcode, hid, guid, data, size, compress))
	{
		delete result;
		return 0;
	}

	return result;
}

Packet * Packet::New (uint16_t opcode, int hid, uint64_t guid, const std::string& data)
{
	Packet *result = new Packet;

	if (! result->wrap(opcode, hid, guid, data))
	{
		delete result;
		return 0;
	}

	return result;
}

Packet * Packet::New (uint16_t opcode, int hid, uint64_t guid, const google::protobuf::Message* message)
{
	Packet *result = new Packet;

	if (! result->wrap (opcode, hid, guid, message))
	{
		delete result;
		return 0;
	}

	return result;
}

Packet * Packet::New(const std::string& data)
{
	ACE_Message_Block *message = new ACE_Message_Block(data.size());
	message->copy(data.data(), data.size());
	return New(message);
}

Packet * Packet::New (ACE_Message_Block* message)
{
	/// 需要保证有一个足够的头部信息用于判断 
	if (message->length () < Packet::HSize ())
	{
		return 0;
	}

	/// 一定是以协议头部开始
	Packet::Header *hp = HD_PTR(message);

	/// 判断是否能组成一个完整的协议包
	ssize_t pck_whole_size = hp->size + Packet::HSize ();
	if (pck_whole_size != message->length ())
	{
		return 0;
	}

	Packet *result = new Packet;

	result->payload = message;
	result->hp = HD_PTR(result->payload);

	return result;
}

Packet * Packet::Extract (ACE_Message_Block& message_block)
{
	ACE_Message_Block *segment = Packet::Format (message_block);

	if (segment == NULL) return NULL;

	Packet *pck = new Packet;

	pck->payload = segment;
	pck->hp = HD_PTR(pck->payload);

	return pck;
}

ACE_Message_Block * Packet::Format (ACE_Message_Block& message_block)
{
	/// 需要保证有一个足够的头部信息用于判断 
	if (message_block.length () < Packet::HSize ())
		return NULL;

	/// 一定是以协议头部开始
	Packet::Header *hp = HD_PTR(&message_block);

	/// 判断是否能组成一个完整的协议包
	ssize_t pck_whole_size = hp->size + Packet::HSize ();
	if (pck_whole_size > message_block.length ())
		return NULL;

	ACE_Message_Block *segment = new ACE_Message_Block (pck_whole_size);

	segment->copy (message_block.rd_ptr (), pck_whole_size);
	message_block.rd_ptr (pck_whole_size);

	return segment;
}

int Packet::TestSize (ACE_Message_Block& message_block)
{
	if (message_block.length () < Packet::HSize ())
		return 0;

	Packet::Header *hp = HD_PTR(&message_block);

	return hp->size;
}

std::size_t Packet::HSize (void)
{
	return sizeof(Packet::Header);
}

Packet::~Packet(void)
{
	if (payload)
	{
		payload->release();
		payload = 0;
	}
}

Packet * Packet::clear(Packet *fre)
{
	Packet *cur = fre;

	for (;;)
	{
		if (! fre) break; 

		cur = cur->next (); delete fre; fre = cur;
	}

	return 0;
}

Packet::Header & Packet::hptr (void)
{
	if (!hp)
	{
		hp = HD_PTR(payload);
	}

	return *hp;
}

uint16_t Packet::opcode (void)
{
	return hptr().opcode;
}

uint64_t Packet::guid (void)
{
	return hptr().guid;
}

bool Packet::compress (void)
{
	return hptr().compress;
}

uint32_t Packet::size (void)
{
	return hptr().size;
}

int Packet::hid (void)
{
	return hptr().hid;
}

const char * Packet::body (void)
{
	return BO_PTR(payload);
}

Packet * Packet::clone (void)
{
	Packet *pck = new Packet;
	pck->payload = payload->clone ();

	return pck;
}

bool Packet::do_compress ()
{
	Packet::Header *hp = HD_PTR(payload);

	std::vector<unsigned char> imm_buf (MAX_PACKET_SIZE);
	unsigned long buf_size = imm_buf.size ();

	::compress (&imm_buf[0], &buf_size, (unsigned char *)BO_PTR(payload), hp->size);
	
	if (buf_size > hp->size) return false;  /// compression failure

	payload->reset ();
	payload->wr_ptr (Packet::HSize ());
	
	payload->copy ((char*)&imm_buf[0], buf_size);
	hp->size = buf_size;
	hp->compress = true;  /// set compress flag

	return true;
}

google::protobuf::Message * Packet::parse_protocol (google::protobuf::Message& msg) const
{
	Header *hp = HD_PTR(payload);
	google::protobuf::io::ArrayInputStream istream (BO_PTR(payload), hp->size);

	if (! hp->compress)
	{
		if (msg.ParseFromZeroCopyStream (&istream))
			return &msg;
		else
			return NULL;
	}
	else
	{
		std::vector<unsigned char> imm_buf (MAX_PACKET_SIZE * 5);
		unsigned long buf_size = imm_buf.size ();

		::uncompress (&imm_buf[0], &buf_size, (unsigned char *)BO_PTR(payload), hp->size);
		if (msg.ParseFromArray (&imm_buf[0], buf_size))
			return &msg;
		else
			return NULL;
	}

	return NULL;
}

ACE_Message_Block * Packet::release (void)
{
	ACE_Message_Block *mb = payload;
	payload = 0;
	return mb;
}

Packet * Packet::next (void)
{
	return next_;
}

Packet * Packet::add_tail (Packet* pck)
{
	Packet *cur = this;

	for (;;)
	{
		if (!cur->next_) 
		{
			cur->next_ = pck;
			return pck;
		} 
		else
		{
			cur = cur->next_;
		}
	}

	return 0;
}

bool Packet::wrap(uint16_t opcode, int hid, uint64_t guid, const char * data, int len, bool compress)
{
	Header header;

	header.opcode = opcode;
	header.guid = guid;
	header.compress = compress;
	header.size = len;
	header.hid = hid;

	payload = new ACE_Message_Block(len + Packet::HSize());

	payload->copy(reinterpret_cast<char*>(&header), Packet::HSize());
	payload->copy(data, len);

	return true;
}

bool Packet::wrap (uint16_t opcode, int hid, uint64_t guid, const std::string& data)
{
	Header header;

	header.opcode   = opcode;
	header.guid     = guid;
	header.compress = false;
	header.size     = data.size ();
	header.hid		= hid;

	payload = new ACE_Message_Block(data.size() + Packet::HSize());

	payload->copy (reinterpret_cast<char*>(&header), Packet::HSize());
	payload->copy (data.data (), data.size ());

	hp = HD_PTR(payload);

	if (hp->size >= COMPRESS_SIZE)
	{
		do_compress();
	}

	if (hp->size > MAX_PACKET_SIZE)
	{
		return false;
	}

	return true;
}

bool Packet::wrap (uint16_t opcode, int hid, uint64_t guid, const google::protobuf::Message* message)
{
	Header header;

	header.opcode   = opcode;
	header.guid     = guid;
	header.compress = false;
	header.size     = 0;
	header.hid		= hid;

	int bytes = message ? message->ByteSize () : 0;

	header.size = bytes;

	payload = new ACE_Message_Block (bytes + Packet::HSize ());

	payload->copy (reinterpret_cast<char*>(&header), Packet::HSize());

	if (message && message->SerializeToArray (payload->wr_ptr (), bytes))
		payload->wr_ptr (bytes);

	hp = HD_PTR(payload);

	if (hp->size >= COMPRESS_SIZE)
	{
		do_compress();
	}

	if (hp->size > MAX_PACKET_SIZE)
	{
		return false;
	}

	return true;
}
