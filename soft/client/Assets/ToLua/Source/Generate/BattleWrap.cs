﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class BattleWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Battle), typeof(System.Object));
		L.RegFunction("UpdateSelfData", UpdateSelfData);
		L.RegFunction("Awake", Awake);
		L.RegFunction("Start", Start);
		L.RegFunction("OnDestroy", OnDestroy);
		L.RegFunction("GetTimeStr", GetTimeStr);
		L.RegFunction("send_offline_state", send_offline_state);
		L.RegFunction("RegisterMessage", RegisterMessage);
		L.RegFunction("RemoveRegisterMessage", RemoveRegisterMessage);
		L.RegFunction("send_state", send_state);
		L.RegFunction("send_code", send_code);
		L.RegFunction("get_string", get_string);
		L.RegFunction("send_talent", send_talent);
		L.RegFunction("send_battle_inside_msg", send_battle_inside_msg);
		L.RegFunction("send_release", send_release);
		L.RegFunction("send_prerelease", send_prerelease);
		L.RegFunction("send_attackr", send_attackr);
		L.RegFunction("send_attackr_true", send_attackr_true);
		L.RegFunction("send_move_stop_true", send_move_stop_true);
		L.RegFunction("send_stop", send_stop);
		L.RegFunction("send_stop1", send_stop1);
		L.RegFunction("send_move", send_move);
		L.RegFunction("send_move1", send_move1);
		L.RegFunction("send_reset", send_reset);
		L.RegFunction("send_in", send_in);
		L.RegFunction("LanGan", LanGan);
		L.RegFunction("ShowBattlePanel", ShowBattlePanel);
		L.RegFunction("send_change_skill", send_change_skill);
		L.RegFunction("RandomForeBattleMsg", RandomForeBattleMsg);
		L.RegFunction("SMSG_BATTLE_LINK", SMSG_BATTLE_LINK);
		L.RegFunction("SMSG_BATTLE_OP", SMSG_BATTLE_OP);
		L.RegFunction("SMSG_BATTLE_ZHEN", SMSG_BATTLE_ZHEN);
		L.RegFunction("SMSG_BATTLE_FINISH", SMSG_BATTLE_FINISH);
		L.RegFunction("GetNear", GetNear);
		L.RegFunction("send_result", send_result);
		L.RegFunction("SMSG_GUIDE", SMSG_GUIDE);
		L.RegFunction("ChangeSkill", ChangeSkill);
		L.RegFunction("hreturn", hreturn);
		L.RegFunction("SendBattleNullUdp", SendBattleNullUdp);
		L.RegFunction("New", _CreateBattle);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegConstant("BL", 10000);
		L.RegVar("name", get_name, set_name);
		L.RegVar("is_end", get_is_end, set_is_end);
		L.RegVar("is_online", get_is_online, set_is_online);
		L.RegVar("is_newplayer_guide", get_is_newplayer_guide, set_is_newplayer_guide);
		L.RegVar("isPause", get_isPause, set_isPause);
		L.RegVar("ch_pause", get_ch_pause, set_ch_pause);
		L.RegVar("turnTime", get_turnTime, set_turnTime);
		L.RegVar("exTime", get_exTime, set_exTime);
		L.RegVar("is_new", get_is_new, set_is_new);
		L.RegVar("self_skills", get_self_skills, set_self_skills);
		L.RegVar("is_frist_Skill_Hold", get_is_frist_Skill_Hold, set_is_frist_Skill_Hold);
		L.RegVar("move_hold_r_", get_move_hold_r_, set_move_hold_r_);
		L.RegVar("attack_hold_r_", get_attack_hold_r_, set_attack_hold_r_);
		L.RegVar("is_hold_", get_is_hold_, set_is_hold_);
		L.RegVar("holds_", get_holds_, set_holds_);
		L.RegVar("key_hold_r_", get_key_hold_r_, set_key_hold_r_);
		L.RegVar("battle_panel", get_battle_panel, set_battle_panel);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateBattle(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				Battle obj = new Battle();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: Battle.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateSelfData(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 7);
			dhc.player_t arg0 = (dhc.player_t)ToLua.CheckObject<dhc.player_t>(L, 1);
			string arg1 = ToLua.CheckString(L, 2);
			string arg2 = ToLua.CheckString(L, 3);
			System.Collections.Generic.List<dhc.role_t> arg3 = (System.Collections.Generic.List<dhc.role_t>)ToLua.CheckObject(L, 4, typeof(System.Collections.Generic.List<dhc.role_t>));
			int arg4 = (int)LuaDLL.luaL_checknumber(L, 5);
			string arg5 = ToLua.CheckString(L, 6);
			int arg6 = (int)LuaDLL.luaL_checknumber(L, 7);
			Battle.UpdateSelfData(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Awake(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 2);
				Battle.Awake(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 2);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 3);
				Battle.Awake(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Battle.Awake");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Start(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.Start();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnDestroy(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.OnDestroy();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTimeStr(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = Battle.GetTimeStr();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_offline_state(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Battle.send_offline_state(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterMessage(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.RegisterMessage();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveRegisterMessage(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.RemoveRegisterMessage();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_state(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_state();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_code(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_code();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_string(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			byte[] o = Battle.get_string();
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_talent(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Battle.send_talent(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_battle_inside_msg(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Battle.send_battle_inside_msg(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_release(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 5);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
			float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
			float arg3 = (float)LuaDLL.luaL_checknumber(L, 4);
			int arg4 = (int)LuaDLL.luaL_checknumber(L, 5);
			Battle.send_release(arg0, arg1, arg2, arg3, arg4);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_prerelease(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Battle.send_prerelease(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_attackr(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Battle.send_attackr(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_attackr_true(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_attackr_true();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_move_stop_true(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_move_stop_true();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_stop(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Battle.send_stop(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_stop1(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_stop1();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_move(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Battle.send_move(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_move1(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_move1();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_reset(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_reset();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_in(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_in();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LanGan(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.LanGan();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowBattlePanel(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.ShowBattlePanel();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_change_skill(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_change_skill();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RandomForeBattleMsg(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			System.Collections.Generic.List<int> o = Battle.RandomForeBattleMsg();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SMSG_BATTLE_LINK(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			s_net_message arg0 = (s_net_message)ToLua.CheckObject<s_net_message>(L, 1);
			Battle.SMSG_BATTLE_LINK(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SMSG_BATTLE_OP(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			s_net_message arg0 = (s_net_message)ToLua.CheckObject<s_net_message>(L, 1);
			Battle.SMSG_BATTLE_OP(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SMSG_BATTLE_ZHEN(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			s_net_message arg0 = (s_net_message)ToLua.CheckObject<s_net_message>(L, 1);
			Battle.SMSG_BATTLE_ZHEN(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SMSG_BATTLE_FINISH(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			s_net_message arg0 = (s_net_message)ToLua.CheckObject<s_net_message>(L, 1);
			Battle.SMSG_BATTLE_FINISH(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetNear(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			long arg0 = LuaDLL.tolua_checkint64(L, 1);
			find_player o = Battle.GetNear(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int send_result(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.send_result();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SMSG_GUIDE(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			s_net_message arg0 = (s_net_message)ToLua.CheckObject<s_net_message>(L, 1);
			Battle.SMSG_GUIDE(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeSkill(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Battle.ChangeSkill(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int hreturn(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Battle.hreturn();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendBattleNullUdp(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			opclient_t arg0 = (opclient_t)ToLua.CheckObject(L, 1, typeof(opclient_t));
			Battle.SendBattleNullUdp(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_name(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, Battle.name);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_is_end(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, Battle.is_end);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_is_online(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, Battle.is_online);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_is_newplayer_guide(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, Battle.is_newplayer_guide);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isPause(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, Battle.isPause);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ch_pause(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, Battle.ch_pause);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_turnTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushnumber(L, Battle.turnTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_exTime(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Battle.exTime);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_is_new(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Battle.is_new);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_self_skills(IntPtr L)
	{
		try
		{
			ToLua.PushSealed(L, Battle.self_skills);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_is_frist_Skill_Hold(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, Battle.is_frist_Skill_Hold);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_move_hold_r_(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Battle.move_hold_r_);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_attack_hold_r_(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Battle.attack_hold_r_);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_is_hold_(IntPtr L)
	{
		try
		{
			ToLua.PushSealed(L, Battle.is_hold_);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_holds_(IntPtr L)
	{
		try
		{
			ToLua.PushSealed(L, Battle.holds_);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_key_hold_r_(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, Battle.key_hold_r_);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_battle_panel(IntPtr L)
	{
		try
		{
			ToLua.Push(L, Battle.battle_panel);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_name(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			Battle.name = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_is_end(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			Battle.is_end = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_is_online(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			Battle.is_online = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_is_newplayer_guide(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			Battle.is_newplayer_guide = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_isPause(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			Battle.isPause = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ch_pause(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			Battle.ch_pause = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_turnTime(IntPtr L)
	{
		try
		{
			double arg0 = (double)LuaDLL.luaL_checknumber(L, 2);
			Battle.turnTime = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_exTime(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			Battle.exTime = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_is_new(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			Battle.is_new = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_self_skills(IntPtr L)
	{
		try
		{
			System.Collections.Generic.List<int> arg0 = (System.Collections.Generic.List<int>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<int>));
			Battle.self_skills = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_is_frist_Skill_Hold(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			Battle.is_frist_Skill_Hold = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_move_hold_r_(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			Battle.move_hold_r_ = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_attack_hold_r_(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			Battle.attack_hold_r_ = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_is_hold_(IntPtr L)
	{
		try
		{
			System.Collections.Generic.Dictionary<string,bool> arg0 = (System.Collections.Generic.Dictionary<string,bool>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.Dictionary<string,bool>));
			Battle.is_hold_ = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_holds_(IntPtr L)
	{
		try
		{
			System.Collections.Generic.List<string> arg0 = (System.Collections.Generic.List<string>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<string>));
			Battle.holds_ = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_key_hold_r_(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			Battle.key_hold_r_ = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_battle_panel(IntPtr L)
	{
		try
		{
			BattlePanel arg0 = (BattlePanel)ToLua.CheckObject<BattlePanel>(L, 2);
			Battle.battle_panel = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

