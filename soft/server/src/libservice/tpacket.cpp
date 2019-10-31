#include "tpacket.h"
#include <google/protobuf/io/zero_copy_stream_impl.h>
#include <google/protobuf/io/gzip_stream.h>

/// 最大包长度: 64KB
#define MAX_PACKET_SIZE (64 * 1024)
#define COMPRESS_SIZE 1024

TPacket * TPacket::New(uint16_t opcode, const char *data, int size, bool compress)
{
	TPacket *result = new TPacket;

	if (!result->wrap(opcode, data, size, compress))
	{
		delete result;
		return 0;
	}

	return result;
}

TPacket * TPacket::New(uint16_t opcode, const std::string& data)
{
	TPacket *result = new TPacket;

	if (! result->wrap(opcode, data))
	{
		delete result;
		return 0;
	}

	return result;
}

TPacket * TPacket::New(uint16_t opcode, const google::protobuf::Message* message)
{
	TPacket *result = new TPacket;

	if (! result->wrap (opcode, message))
	{
		delete result;
		return 0;
	}

	return result;
}

TPacket * TPacket::Extract(ACE_Message_Block& message_block)
{
	ACE_Message_Block *segment = TPacket::Format(message_block);

	if (segment == NULL) return NULL;

	TPacket *pck = new TPacket;

	pck->payload = segment;
	pck->hp = THD_PTR(pck->payload);

	return pck;
}

ACE_Message_Block * TPacket::Format(ACE_Message_Block& message_block)
{
	/// 需要保证有一个足够的头部信息用于判断 
	if (message_block.length() < TPacket::HSize())
		return NULL;

	/// 一定是以协议头部开始
	TPacket::Header *hp = THD_PTR(&message_block);

	/// 判断是否能组成一个完整的协议包
	ssize_t pck_whole_size = hp->size + TPacket::HSize();
	if (pck_whole_size > message_block.length ())
		return NULL;

	ACE_Message_Block *segment = new ACE_Message_Block (pck_whole_size);

	segment->copy (message_block.rd_ptr (), pck_whole_size);
	message_block.rd_ptr (pck_whole_size);

	return segment;
}

int TPacket::TestSize(ACE_Message_Block& message_block)
{
	if (message_block.length() < TPacket::HSize())
		return 0;

	TPacket::Header *hp = THD_PTR(&message_block);

	return hp->size;
}

uint16_t TPacket::TestOpcode(ACE_Message_Block& message_block)
{
	if (message_block.length() < TPacket::HSize())
		return 0;

	TPacket::Header *hp = THD_PTR(&message_block);

	return hp->opcode;
}

std::size_t TPacket::HSize(void)
{
	return sizeof(TPacket::Header);
}

TPacket::~TPacket(void)
{
	if (payload)
	{
		payload->release();
		payload = 0;
	}
}

TPacket * TPacket::clear(TPacket *fre)
{
	delete fre;
	return 0;
}

TPacket::Header & TPacket::hptr(void)
{
	if (!hp)
	{
		hp = THD_PTR(payload);
	}

	return *hp;
}

uint16_t TPacket::opcode(void)
{
	return hptr().opcode;
}

bool TPacket::compress(void)
{
	return hptr().compress;
}

uint32_t TPacket::size(void)
{
	return hptr().size;
}

const char * TPacket::body(void)
{
	return TBO_PTR(payload);
}

bool TPacket::do_compress()
{
	TPacket::Header *hp = THD_PTR(payload);

	std::vector<unsigned char> imm_buf (MAX_PACKET_SIZE);
	unsigned long buf_size = imm_buf.size ();

	::compress (&imm_buf[0], &buf_size, (unsigned char *)TBO_PTR(payload), hp->size);
	
	if (buf_size > hp->size) return false;  /// compression failure

	payload->reset ();
	payload->wr_ptr(TPacket::HSize());
	
	payload->copy ((char*)&imm_buf[0], buf_size);
	hp->size = buf_size;
	hp->compress = true;  /// set compress flag

	return true;
}

ACE_Message_Block * TPacket::release(void)
{
	ACE_Message_Block *mb = payload;
	payload = 0;
	return mb;
}

bool TPacket::wrap(uint16_t opcode, const char * data, int len, bool compress)
{
	Header header;

	header.opcode = opcode;
	header.compress = compress;
	header.size = len;

	payload = new ACE_Message_Block(len + TPacket::HSize());

	payload->copy(reinterpret_cast<char*>(&header), TPacket::HSize());
	payload->copy(data, len);

	return true;
}

bool TPacket::wrap(uint16_t opcode, const std::string& data)
{
	Header header;

	header.opcode = opcode;
	header.compress = false;
	header.size = data.size();

	payload = new ACE_Message_Block(data.size() + TPacket::HSize());

	payload->copy(reinterpret_cast<char*>(&header), TPacket::HSize());
	payload->copy (data.data (), data.size ());

	hp = THD_PTR(payload);

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

bool TPacket::wrap(uint16_t opcode, const google::protobuf::Message* message)
{
	Header header;

	header.opcode = opcode;
	header.compress = false;
	header.size = 0;

	int bytes = message ? message->ByteSize() : 0;

	header.size = bytes;

	payload = new ACE_Message_Block(bytes + TPacket::HSize());

	payload->copy(reinterpret_cast<char*>(&header), TPacket::HSize());

	if (message && message->SerializeToArray (payload->wr_ptr (), bytes))
		payload->wr_ptr (bytes);

	hp = THD_PTR(payload);

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
