#ifndef __SERVICE_INC_H__
#define __SERVICE_INC_H__

#include "service_interface.h"
#include "packet.h"
#include "service.h"
#include "singleton.h"

#define MAKE_GUID(T, I) (((uint64_t(T) << 56) & 0xFF00000000000000) | ((uint64_t(boost::lexical_cast<uint64_t>(service::server_env()->get_game_value("qid"))) << 44) & 0x00FFF00000000000) | (uint64_t(I) & 0x00000FFFFFFFFFFF))
#define MAKE_GUID_EX(T, Q, I) (((uint64_t(T) << 56) & 0xFF00000000000000) | ((uint64_t(Q) << 44) & 0x00FFF00000000000) | (uint64_t(I) & 0x00000FFFFFFFFFFF))
#define TRANS_GUID(T, G) (((uint64_t(T) << 56) & 0xFF00000000000000) | (uint64_t(G) & 0x00FFFFFFFFFFFFFF))
#define GUID_COUNTER(guid) (uint64_t)(guid & 0x00000FFFFFFFFFFF)

enum etypes
{
	et_null = 0x00,				/// ������
	et_player = 0x01,			/// �������
	et_acc = 0x02,				/// �˺�����
	et_gtool = 0x03,			/// guid����������
	et_role = 0x04,				/// ��ɫ����
	et_battle_his = 0x05,		/// ��ʷս��
	et_battle_result = 0x06,	/// ս����¼
	et_post = 0x07,				/// �ʼ�
	et_post_new = 0x08,			/// �ʼ�new
	et_share = 0x09,			/// ����
	et_reharge = 0x0a,			/// ��ֵ
	et_team = 0x0b,				/// ����
	et_name = 0x0c,				/// ���ַ���
	et_social = 0x0d,			/// ���ѷ���
	et_social_list = 0x0e,		/// �����б�
	et_rank = 0x0f,				/// ���а�
	et_all
};

inline etypes type_of_guid(uint64_t guid)
{
	uint8_t et = (uint8_t)((uint64_t(guid) >> 56) & 0x00000000000000FF);
	return (et > et_null && et < et_all) ? etypes(et) : et_null;
}

#define POOL_GET(guid, type) dynamic_cast<type *>(service::pool()->get(guid))

inline std::string make_ertext(const char *s, int d, const char *ss)
{
	char c[256];
	sprintf(c, "%s(%d)-<%s>: ", s, d, ss);
	return std::string(c);
}

#endif
