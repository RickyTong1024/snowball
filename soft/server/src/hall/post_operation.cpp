#include "post_operation.h"
#include "hall_dhc.h"
#include "hall_message.h"

void PostOperation::post_create(uint64_t player_guid, const std::string &title, const std::string &text,
	const std::string &sender_name, const s_t_rewards &rewards)
{
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		post_create_offline(player_guid, title, text, sender_name, rewards);
	}
	else
	{
		post_create_online(player, title, text, sender_name, rewards);
	}
}

void PostOperation::post_create_online(dhc::player_t *player, const std::string &title, const std::string &text,
	const std::string &sender_name, const s_t_rewards &rewards)
{
	if (player->post_guids_size() >= 20)
	{
		return;
	}
	uint64_t post_guid = DB_GTOOL->assign(et_post);
	dhc::post_t *post = new dhc::post_t();
	post->set_guid(post_guid);
	post->set_player_guid(player->guid());
	post->set_send_date(service::timer()->now());
	post->set_title(title);
	post->set_text(text);
	post->set_sender_name(sender_name);
	for (int i = 0; i < rewards.rewards.size(); ++i)
	{
		post->add_type(rewards.rewards[i].type);
		post->add_value1(rewards.rewards[i].value1);
		post->add_value2(rewards.rewards[i].value2);
		post->add_value3(rewards.rewards[i].value3);
	}
	post->set_is_read(0);
	if (rewards.rewards.size() == 0)
	{
		post->set_is_pick(1);
	}
	else
	{
		post->set_is_pick(0);
	}
	service::pool()->add(post_guid, post, mmg::Pool::state_new);
	player->add_post_guids(post->guid());
	HallMessage::send_smsg_post_num(player);
}

void PostOperation::post_create_offline(uint64_t player_guid, const std::string &title, const std::string &text,
	const std::string &sender_name, const s_t_rewards &rewards)
{
	dhc::post_new_t *post_new = new dhc::post_new_t();
	post_new->set_player_guid(player_guid);
	post_new->set_send_date(service::timer()->now());
	post_new->set_title(title);
	post_new->set_text(text);
	post_new->set_sender_name(sender_name);
	for (int i = 0; i < rewards.rewards.size(); ++i)
	{
		post_new->add_type(rewards.rewards[i].type);
		post_new->add_value1(rewards.rewards[i].value1);
		post_new->add_value2(rewards.rewards[i].value2);
		post_new->add_value3(rewards.rewards[i].value3);
	}
	Request *req = new Request();
	req->add(opc_insert, MAKE_GUID(et_post_new, 0), post_new);
	DB_PLAYER(player_guid)->upcall(req, 0);
}

void PostOperation::post_delete(dhc::player_t *player, uint64_t post_guid)
{
	for (int i = 0; i < player->post_guids_size(); ++i)
	{
		if (player->post_guids(i) == post_guid)
		{
			player->mutable_post_guids()->SwapElements(i, player->post_guids_size() - 1);
			player->mutable_post_guids()->RemoveLast();
			break;
		}
	}
	service::pool()->remove(post_guid, player->guid());
}

void PostOperation::get_new_post(dhc::player_t *player)
{
	uint64_t post_new_guid = MAKE_GUID(et_post_new, 0);
	Request *req = new Request();
	protocol::game::msg_post_new_list *obj = new protocol::game::msg_post_new_list();
	obj->set_player_guid(player->guid());
	req->add(opc_query, post_new_guid, obj);
	DB_PLAYER(player->guid())->upcall(req, boost::bind(&PostOperation::get_new_post_callback, _1, player->guid()));
}

void PostOperation::get_new_post_callback(Request *req, uint64_t player_guid)
{
	if (req->result() < 0)
	{
		return;
	}
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return;
	}
	protocol::game::msg_post_new_list *list = (protocol::game::msg_post_new_list *)req->data();
	for (int i = 0; i < list->post_news_size(); ++i)
	{
		s_t_rewards rewards;
		for (int j = 0; j < list->post_news(i).type_size(); ++j)
		{
			rewards.add_reward(list->post_news(i).type(j), list->post_news(i).value1(j), list->post_news(i).value2(j), list->post_news(i).value3(j));
		}
		post_create_online(player, list->post_news(i).title(), list->post_news(i).text(), list->post_news(i).sender_name(), rewards);
	}
	uint64_t post_new_guid = MAKE_GUID(et_post_new, 0);
	Request *req1 = new Request();
	protocol::game::msg_post_new_delete *obj = new protocol::game::msg_post_new_delete();
	obj->set_player_guid(player->guid());
	req1->add(opc_remove, post_new_guid, obj);
	DB_PLAYER(player->guid())->upcall(req1, 0);
}

void PostOperation::check_post(dhc::player_t *player)
{
	uint64_t time = service::timer()->now() - 7 * 86400000;
	for (int i = 0; i < player->post_guids_size();)
	{
		uint64_t post_guid = player->post_guids(i);
		dhc::post_t *post = POOL_GET(post_guid, dhc::post_t);
		if (!post || post->send_date() < time)
		{
			player->mutable_post_guids()->SwapElements(i, player->post_guids_size() - 1);
			player->mutable_post_guids()->RemoveLast();
		}
		else
		{
			++i;
		}
	}
}

void PostOperation::get_share(dhc::player_t *player)
{
	uint64_t share_guid = TRANS_GUID(et_share, player->guid());
	Request *req = new Request();
	dhc::share_t *obj = new dhc::share_t;
	req->add(opc_query, share_guid, obj);
	DB_PLAYER(player->guid())->upcall(req, boost::bind(&PostOperation::get_share_callback, _1, player->guid()));
}

void PostOperation::get_share_callback(Request *req, uint64_t player_guid)
{
	if (req->result() < 0)
	{
		return;
	}
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return;
	}
	uint64_t share_guid = TRANS_GUID(et_share, player->guid());
	Request *req1 = new Request();
	req1->add(opc_remove, share_guid, 0);
	DB_PLAYER(player->guid())->upcall(req1, 0);

	dhc::share_t *share = (dhc::share_t *)req->data();
	if (service::timer()->trigger_time(share->ctime(), 5, 0))
	{
		return;
	}
	int num = share->num();
	if (num + player->fenxiang_num() > 100)
	{
		num = 100 - player->fenxiang_num();
	}
	if (num > 0)
	{
		player->set_fenxiang_num(player->fenxiang_num() + num);
		player->set_fenxiang_total_num(player->fenxiang_total_num() + num);
		PlayerOperation::player_add_resource(player, resource::SNOW, num);
		HallMessage::send_smsg_fenxiang_num(player);
	}
}
