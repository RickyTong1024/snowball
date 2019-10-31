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
	et_null = 0x00,				/// 空类型
	et_player = 0x01,			/// 玩家类型
	et_acc = 0x02,				/// 账号类型
	et_gtool = 0x03,			/// guid产生器类型
	et_role = 0x04,				/// 角色类型
	et_battle_his = 0x05,		/// 历史战绩
	et_battle_result = 0x06,	/// 战斗记录
	et_post = 0x07,				/// 邮件
	et_post_new = 0x08,			/// 邮件new
	et_share = 0x09,			/// 分享
	et_reharge = 0x0a,			/// 充值
	et_team = 0x0b,				/// 队伍
	et_name = 0x0c,				/// 名字服务
	et_social = 0x0d,			/// 好友服务
	et_social_list = 0x0e,		/// 好友列表
	et_rank = 0x0f,				/// 排行榜
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
