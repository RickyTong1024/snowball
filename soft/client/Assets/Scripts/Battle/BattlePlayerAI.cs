using BattleDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class BattlePlayerAI
{
    public static int ComplexTNUM = 0;
    public static bool BossTips = false;
    public static bool IsOnce = true;

    public static int t_attack_action_1;
    public static int t_attack_action_2;
    public static int t_attack_action_3;

    public static void Init()
    {
        BossTips = false;
        IsOnce = true;
        BattlePlayerAI.ComplexTNUM = 6;
        BattlePlayerAI.t_attack_action_1 = BattleOperation.toInt(1.3 * BattlePlayers.TNUM);
        BattlePlayerAI.t_attack_action_2 = BattleOperation.toInt(2.0 * BattlePlayers.TNUM);
        BattlePlayerAI.t_attack_action_3 = BattleOperation.toInt(3.0 * BattlePlayers.TNUM);
    }

    public static void Fini()
    {

    }

    public static void new_player_ai_process(BattleAnimalPlayer pt)
    {
        if (pt.animal.ai_state == 0 && pt.animal.is_move)
        {
            BattlePlayerAI.Stop(pt);
            pt.animal.ai_state = 0;
            return;
        }

        if (pt.animal.ai_state == 1)
        {
            BattlePlayers.Move1(pt, pt.ai_r);
            BattlePlayers.Attackr1(pt, pt.ai_r);
        }
        else if (pt.animal.ai_state == 2)
        {
            var r = BattleOperation.get_r(pt.animal.x, pt.animal.y, BattlePlayers.me.animal.x, BattlePlayers.me.animal.y);
            BattlePlayers.Release1(pt, 100101, BattlePlayers.me.animal.x, BattlePlayers.me.animal.y, r);
        }
    }

    public static void set_ai_attribute(BattleAnimalPlayer pt,int ai_state,int r)
    {
        if (pt == null || pt.animal.is_ai == 0)
            return;

        pt.animal.ai_state = ai_state;
        pt.ai_r = r;
        pt.ori_x = pt.animal.x;
        pt.ori_y = pt.animal.y;
    }

    public static void Stop(BattleAnimal pt)
    {
        if (pt == null)
            return;
        if (pt.is_die)
            return;
        BattlePlayers.Stop1(pt, pt.animal.r);
    }

    public static int[] AvoidSkillPoint(BattleAnimal pt, List<int> tids)
    {
        if (tids.Count <= 0)
            return null;

        List<List<int>> areaList = new List<List<int>>();
        int _index = 0;

        for (int i = 0; i < 12; i++)
            areaList.Add(new List<int>());

        while (_index < tids.Count)
        {
            var effect = BattlePlayers.effects[tids[_index]].effect;
            int x = effect.x;
            int y = effect.y;
            int dx = x - pt.animal.x;
            int dy = y - pt.animal.y;
            var angle = BattleOperation.get_r(x, y, pt.animal.x, pt.animal.y);
            angle = BattleOperation.checkr(angle);
            int order = Mathf.CeilToInt((angle + 1) / 30.0f);
            areaList[order - 1].Add(tids[_index]);
            _index = _index + 1;
        }

        int topAreaCount = 0, bottomAreaCount = 0, leftAreaCount = 0, rightAreaCount = 0;
        int sign = 0;
        for (int i = 0; i < 12; i++)
        {
            if (areaList[i].Count < 7)
                topAreaCount = topAreaCount + 1;
            else
                bottomAreaCount = bottomAreaCount + 1;
        }

        if (topAreaCount > bottomAreaCount)
        {
            sign = 0;
            for (int i = 0; i < 6; i++)
            {
                if (areaList[i].Count > 3)
                    leftAreaCount = leftAreaCount + 1;
                else
                    rightAreaCount = rightAreaCount + 1;
            }
        }
        else
        {
            sign = 6;
            for (int i = 6; i < 12; i++)
            {
                if (areaList[i].Count < 10)
                    leftAreaCount = leftAreaCount + 1;
                else
                    rightAreaCount = rightAreaCount + 1;
            }
        }

        int xx = 1;
        if ((leftAreaCount > rightAreaCount) && sign == 0)
            xx = 4;
        else if ((leftAreaCount <= rightAreaCount) && sign == 0)
            xx = 3;
        else if ((leftAreaCount > rightAreaCount) && sign == 6)
            xx = 1;
        else
            xx = 2;

        if ((pt.animal.x < 20000 || pt.animal.x >= 780000) && (pt.animal.y < 20000 || pt.animal.y >= 780000))
            xx = (xx + 2) % 4;

        int min_x = 0, max_x = 0, min_y = 0, max_y = 0;
        if (xx == 1)
        {
            min_x = pt.animal.x;
            max_x = 800000;
            min_y = pt.animal.y;
            max_y = 800000;
        }
        else if (xx == 2)
        {
            min_x = 0;
            max_x = pt.animal.x;
            min_y = pt.animal.y;
            max_y = 800000;
        }
        else if (xx == 3)
        {
            min_x = 0;
            max_x = pt.animal.x;
            min_y = 0;
            max_y = pt.animal.y;
        }
        else if (xx == 4)
        {
            min_x = pt.animal.x;
            max_x = 800000;
            min_y = 0;
            max_y = pt.animal.y;
        }

        int r_x = BattleOperation.random(min_x, max_x);
        int r_y = BattleOperation.random(min_y, max_y);
        int count = 1;
        while (!BattleGrid.can_move(r_x, r_y))
        {
            r_x = BattleOperation.random(min_x, max_x);

            r_y = BattleOperation.random(min_y, max_y);

            count = count + 1;
            if (count > 20)
                return BattleOperation.get_avilialbe_xy();
        }
        return new int[] { r_x, r_y };
    }

    public static bool IsCd(BattleAnimal pt,t_skill sk)
    {
        if (pt == null)
            return true;

        int rz = -999999;
        for (int i = 0; i < pt.animal.save_re_id.Count; i++)
        {
            if (pt.animal.save_re_id[i] == sk.id)
            {
                rz = pt.animal.save_re_zhen[i];
                break;
            }
        }
        float cd_per = 1;
        if (sk != null)
        {
            if (sk.type == 1)
                cd_per = (BattlePlayers.zhen - rz) * BattlePlayers.TICK * 1.0f / sk.get_cd(pt);
            else
                cd_per = (BattlePlayers.zhen - rz) * BattlePlayers.TICK * 1.0f / sk.get_cd(pt);

            if (cd_per < 0)
                cd_per = 0;
            else if (cd_per > 1)
                cd_per = 1;
        }
        return (cd_per < 1);
    }

    public static bool HasSpareSkill(BattleAnimalPlayer pt)
    {
        if (pt == null)
            return false;

        var normal_skill = Config.get_t_skill(100101);
        t_skill exp_skill = null;
        if (pt.player.skill_id != 0 && pt.player.skill_level != 0)
            exp_skill = Config.get_t_skill(pt.player.skill_id, pt.player.skill_level, pt.get_skill_level_add(pt.player.skill_id));

        bool normal_sk_cd = BattlePlayerAI.IsCd(pt, normal_skill);
        bool exp_sk_cd = true;
        if (exp_skill != null)
            exp_sk_cd = BattlePlayerAI.IsCd(pt, exp_skill);
        else
            exp_sk_cd = true;

        if (!normal_sk_cd || !exp_sk_cd)
            return true;
        else
            return false;
    }

    public static int[] GetMaxAttackDis(BattleAnimalPlayer pt)
    {
        if (pt == null)
            return null;
        var normal_skill = Config.get_t_skill(100101);
        t_skill exp_skill = null;
        if (pt.player.skill_id != 0 && pt.player.skill_level != 0)
            exp_skill = Config.get_t_skill(pt.player.skill_id, pt.player.skill_level, pt.get_skill_level_add(pt.player.skill_id));

        return new int[] { normal_skill.id, normal_skill.get_range(pt) };

        //if (exp_skill == null)
        //    return new int[] { normal_skill.id, normal_skill.get_range(pt) };
        //else
        //{
        //    var nor_dis = normal_skill.get_range(pt);
        //    if (BattlePlayerAI.IsCd(pt, exp_skill))
        //        return new int[] { normal_skill.id, nor_dis };
        //    else
        //    {
        //        var exp_dis = exp_skill.get_range(pt);
        //        if (nor_dis > exp_dis)
        //            return new int[] { normal_skill.id, nor_dis };
        //        else
        //            return new int[] { exp_skill.id, exp_dis };
        //    }
        //}
    }

    public static List<BattleAnimal> FindOtherPlayers(BattleAnimal bp)
    {
        bool isBoss = false;
        string[] guids = null;
        List<BattleAnimal> playerList = new List<BattleAnimal>();

        if (bp is BattleAnimalBoss)
        {
            isBoss = true;
            BattleGrid.get(grid_type.et_player, bp.animal.x, bp.animal.y, out guids, bp.animal.eyeRange);
        }
        else if (bp is BattleAnimalPlayer)
        {
            var tp = bp as BattleAnimalPlayer;
            isBoss = false;
            BattleGrid.get(grid_type.et_player, tp.player.birth_x, tp.player.birth_y, out guids, bp.animal.eyeRange);
        }

        for (int i = 0; i < guids.Length; i++)
        {
            var g = guids[i];
            BattleAnimal bp1 = BattlePlayers.players[g];
            if (bp.animal.guid != bp1.animal.guid && bp.animal.camp != bp1.animal.camp && !bp1.is_die && !bp1.animal.is_xueren && !bp1.attr.is_yinshen())
            {
                if (!isBoss)
                    playerList.Add(bp1);
                else
                {
                    if(bp1.animal.type == unit_type.Player)
                        playerList.Add(bp1);     
                }
            }
        }
        return playerList;
    }

    public static int[] GetAvailablePoint(int x, int y, int r)
    {
        int x_min = 0, x_max = 0, y_min = 0, y_max = 0, xx = 0, yy = 0;
        if ((x - r) >= 0)
            x_min = x - r;
        else
            x_min = 0;

        if ((x + r) <= 800000)
            x_max = x + r;
        else
            x_max = 800000;

        if ((y - r) >= 0)
            y_min = y - r;
        else
            y_min = 0;

        if ((y + r) <= 800000)
            y_max = y + r;
        else
            y_max = 800000;

        xx = BattleOperation.random(x_min, x_max);
        yy = BattleOperation.random(y_min, y_max);
        int count = 0;
        while (!BattleGrid.can_move(xx, yy))
        {
            xx = BattleOperation.random(x_min, x_max);
            yy = BattleOperation.random(y_min, y_max);
            count = count + 1;
            if (count > 20)
                return BattleOperation.get_avilialbe_xy();
        }
        return new int[] { xx, yy };
    }

    public static void auto_add_talent(BattleAnimalPlayer bp)
    {
        if (bp == null || bp.player.talent_point <= 0)
            return;

        List<int> talent_ids = new List<int>();
        var dics = from obj in Config.t_talents orderby obj.Key select obj;
        foreach (KeyValuePair<int, t_talent> item in dics)
            talent_ids.Add(item.Key);
        
        int random_p = BattleOperation.random(0, talent_ids.Count);
        BattlePlayers.Talent1(bp, talent_ids[random_p]);
    }

    public static void AttackCheck(BattleAnimalPlayer bp,int random_num)
    {
        var player_list = BattlePlayerAI.FindOtherPlayers(bp);
        if (player_list.Count > 0)
        {
            List<BattleAnimal> robot_list = new List<BattleAnimal>();
            List<BattleAnimal> hum_list = new List<BattleAnimal>();

            for (int i = 0; i < player_list.Count; i++)
                if (Convert.ToUInt64(player_list[i].animal.guid) >= 100)
                    hum_list.Add(player_list[i]);
                else
                    robot_list.Add(player_list[i]);

            BattleAnimal tplayer;
            var attack_player_num = BattleOperation.random(0, 101);
            var t_ai_attr = Config.get_t_ai_attr(bp.animal.is_ai);
            bool sign = false;

            if (attack_player_num <= t_ai_attr.attack_player_p)
                sign = true;

            if (sign)
            {
                if (hum_list.Count > 0)
                    tplayer = hum_list[0];
                else
                    tplayer = robot_list[0];
            }
            else
            {
                if (robot_list.Count > 0)
                    tplayer = robot_list[0];
                else
                    tplayer = hum_list[0];
            }
            var sk = BattlePlayerAI.GetMaxAttackDis(bp);
            var r = BattleOperation.get_r(bp.animal.x, bp.animal.y, tplayer.animal.x, tplayer.animal.y);
            BattlePlayers.Release1(bp, sk[0], tplayer.animal.x, tplayer.animal.y, r);
            BattlePlayers.PreRelease1(bp, 0);
        }
}

    public static void bubblingHp(List<BattleAnimal> playerList)
    {
        for (int i = 1; i < playerList.Count; i++)
        {
            bool sign = true;
            for (int j = 0; j < playerList.Count - i; j++)
            {
                if (playerList[j].animal.hp > playerList[j + 1].animal.hp)
                {
                    var temp = playerList[j];
                    playerList[j] = playerList[j + 1];
                    playerList[j + 1] = temp;
                    sign = false;
                }
            }
            if (sign)
                break;
        }
    }

    public static void PenguinAI(BattleAnimalMonster bp)
    {
        if (bp.is_die)
            return;
        var t_penguin = Config.get_t_penguin(bp.player.monster_id);
        if (t_penguin.type == 1)
            BattlePlayerAI.SmallPenguinAI(bp);
        else if (t_penguin.type == 2)
            BattlePlayerAI.MidPenguinAI(bp);
        else if (t_penguin.type == 3)//大企鹅
            BattlePlayerAI.BigPenguinAI(bp);
    }

    private static void SmallPenguinAI(BattleAnimalMonster bp)
    {
        if(!String.IsNullOrEmpty(bp.player.attack_guid) && BattlePlayers.players.ContainsKey(bp.player.attack_guid))
        {
            if (bp.player.ai_state != 1)
            {
                bp.player.ai_state = 1;
                bp.player.totalZhen = BattlePlayers.TNUM * 3;
                bp.player.nextPoint_x = 0;
                bp.player.nextPoint_y = 0;
                return;
            }
        }

        var t_penguin = Config.get_t_penguin(bp.player.monster_id);
        if (bp.player.ai_state == 0)  //初始状态
        {
            int p = BattleOperation.random(0, 100);
            if (p < t_penguin.standp)  //静止状态
            {
                bp.player.ai_state = 3;
                bp.player.totalZhen = BattleOperation.random(BattlePlayers.TNUM * t_penguin.mint, BattlePlayers.TNUM * t_penguin.maxt);
                bp.player.nextPoint_x = 0;
                bp.player.nextPoint_y = 0;
            }
            else //移动状态
            {
                int[] ways = BattleOperation.GetPenguinPatrolPoint(bp);
                int numt = (int)BattleOperation.get_distance(bp.player.x, bp.player.y, ways[0], ways[1]) / bp.attr.speed();
                bp.player.ai_state = 2;
                bp.player.totalZhen = numt * BattlePlayers.TNUM;
                bp.player.nextPoint_x = ways[0];
                bp.player.nextPoint_y = ways[1];
            }
        }

        if (bp.player.ai_state == 3) //静止
        {
            if (bp.player.is_move)
                BattlePlayers.Stop1(bp, bp.animal.r);

            if (bp.player.totalZhen > 0)
                bp.player.totalZhen = bp.player.totalZhen - 1;
            else
            {
                bp.player.ai_state = 0;
                bp.player.totalZhen = 0;
                bp.player.nextPoint_x = 0;
                bp.player.nextPoint_y = 0;
            }
        }
        else if (bp.player.ai_state == 2)
        {
            if (bp.player.totalZhen > 0)
            {
                int r = BattleOperation.get_r(bp.player.x, bp.player.y, bp.player.nextPoint_x, bp.player.nextPoint_y);
                r = BattleOperation.checkr(r);
                BattlePlayers.Move1(bp, r);
                bp.player.totalZhen = bp.player.totalZhen - 1;
            }
            else
            {
                if (bp.player.is_move)
                    BattlePlayers.Stop1(bp, bp.animal.r);
                bp.player.ai_state = 0;
                bp.player.totalZhen = 0;
                bp.player.nextPoint_x = 0;
                bp.player.nextPoint_y = 0;
            }
        }


        if (bp.player.ai_state == 1 && bp.player.totalZhen > 0 && bp.player.totalZhen % 2 == 0)
        {
            string[] guids = null;
            BattleGrid.get(grid_type.et_player, bp.player.x, bp.player.y, out guids);
            List<BattleAnimal> gs = new List<BattleAnimal>();
            for (int i = 0; i < guids.Length; i++)
            {
                var tp = BattlePlayers.players[guids[i]];
                if (tp.animal.guid != bp.animal.guid && tp.animal.is_ai <= 100)
                    gs.Add(tp);
            }

            if (gs.Count > 0)
            {
                BattleAnimal tbp = gs[0];
                long dis = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, tbp.animal.x, tbp.animal.y);
                for (int i = 1; i < gs.Count; i++)
                {
                    BattleAnimal tbp2 = gs[i];
                    long dis2 = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, tbp2.animal.x, tbp2.animal.y);
                    if (dis2 < dis)
                    {
                        dis = dis2;
                        tbp = tbp2;
                    }
                }
                var army = tbp;
                long[] wall = GetNearWallDis(bp);
                if (wall != null && BattleOperation.get_distance2(bp.animal.x, bp.animal.y, wall[0], wall[1]) < dis)
                {
                    int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, wall[0], wall[1]);
                    int angle = BattleOperation.random(90, 270);
                    r = BattleOperation.checkr(r + angle);
                    BattlePlayers.Move1(bp, r);
                }
                else
                {
                    int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, army.animal.x, army.animal.y);
                    int angle = BattleOperation.random(90, 270);
                    r = BattleOperation.checkr(r + angle);
                    BattlePlayers.Move1(bp, r);
                }
            }
            bp.animal.totalZhen = bp.animal.totalZhen - 1;
        }
        else if (bp.player.ai_state == 1 && bp.player.totalZhen == 0)
        {
            if (bp.player.is_move)
                BattlePlayers.Stop1(bp, bp.animal.r);
            bp.player.ai_state = 0;
            bp.player.totalZhen = 0;
            bp.player.attack_guid = "";
            bp.player.birth_x = bp.animal.x;
            bp.player.birth_y = bp.animal.y;
        }
        else if (bp.animal.ai_state == 1)
            bp.animal.totalZhen = bp.animal.totalZhen - 1;
    }

    private static void MidPenguinAI(BattleAnimalMonster bp)
    {
        if (!String.IsNullOrEmpty(bp.player.attack_guid) && BattlePlayers.players.ContainsKey(bp.player.attack_guid))
        {
            if (bp.animal.ai_state == 4 || bp.animal.ai_state == 5)
            {
                if (bp.animal.is_move)
                    BattlePlayers.Stop1(bp, bp.animal.r);

                bp.animal.ai_state = 1;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                return;
            }
        }

        var t_penguin = Config.get_t_penguin(bp.player.monster_id);
        if (bp.animal.ai_state == 0)
        {
            int p = BattleOperation.random(0, 100);
            if (p < t_penguin.standp)  //静止状态
            {
                bp.animal.ai_state = 5;
                bp.animal.totalZhen = BattleOperation.random(BattlePlayers.TNUM * t_penguin.mint, BattlePlayers.TNUM * t_penguin.maxt);
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
            }
            else //移动状态
            {
                int[] ways = BattleOperation.GetPenguinPatrolPoint(bp);
                int numt = (int)BattleOperation.get_distance(bp.animal.x, bp.animal.y, ways[0], ways[1]) / bp.attr.speed();
                bp.animal.ai_state = 4;
                bp.animal.totalZhen = numt * BattlePlayers.TNUM;
                bp.animal.nextPoint_x = ways[0];
                bp.animal.nextPoint_y = ways[1];
            }
        }

        if (bp.animal.ai_state == 5) //静止
        {
            if (bp.animal.is_move)
                BattlePlayers.Stop1(bp, bp.animal.r);

            if (bp.animal.totalZhen > 0)
                bp.animal.totalZhen = bp.animal.totalZhen - 1;
            else
            {
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
            }
        }
        else if (bp.animal.ai_state == 4)
        {
            if (bp.animal.totalZhen > 0)
            {
                int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, bp.animal.nextPoint_x, bp.animal.nextPoint_y);
                r = BattleOperation.checkr(r);
                BattlePlayers.Move1(bp, r);
                bp.animal.totalZhen = bp.animal.totalZhen - 1;
            }
            else
            {
                if (bp.animal.is_move)
                    BattlePlayers.Stop1(bp, bp.animal.r);
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
            }
        }

        if (bp.animal.ai_state == 1)
        {
            if (bp.animal.totalZhen > 0)
            {
                bp.animal.totalZhen = bp.animal.totalZhen - 1;
                return;
            }

            BattleAnimal army_bp = null;
            if (!BattlePlayers.players.ContainsKey(bp.player.attack_guid))
            {
                if (bp.player.is_move)
                    BattlePlayers.Stop1(bp, bp.animal.r);
                bp.player.ai_state = 0;
                bp.player.totalZhen = 0;
                bp.player.birth_x = bp.animal.x;
                bp.player.birth_y = bp.animal.y;
                bp.player.nextPoint_x = 0;
                bp.player.nextPoint_y = 0;
                bp.player.attack_guid = "";
            }
            else
            {
                army_bp = BattlePlayers.players[bp.player.attack_guid];
                if (army_bp.is_die || army_bp.animal.is_xueren)
                {
                    if (bp.player.is_move)
                        BattlePlayers.Stop1(bp, bp.animal.r);
                    bp.player.ai_state = 0;
                    bp.player.totalZhen = 0;
                    bp.player.birth_x = bp.animal.x;
                    bp.player.birth_y = bp.animal.y;
                    bp.player.nextPoint_x = 0;
                    bp.player.nextPoint_y = 0;
                    bp.player.attack_guid = "";
                }
                else
                {
                    string[] guids;
                    BattleGrid.get(grid_type.et_player, bp.player.birth_x, bp.player.birth_y, out guids, bp.animal.eyeRange);
                    bool leaveState = true;
                    for (int i = 0; i < guids.Length; i++)
                    {
                        if (guids[i] == bp.player.attack_guid)
                        {
                            leaveState = false;
                            break;
                        }
                    }

                    if (!leaveState)
                    {
                        long dis = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, army_bp.animal.x, army_bp.animal.y);
                        int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, army_bp.animal.x, army_bp.animal.y);
                        r = BattleOperation.checkr(r);
                        if (dis < 100000000)
                        {
                            bp.animal.ai_state = 2;
                            bp.animal.totalZhen = 0;
                            bp.animal.nextPoint_x = 0;
                            bp.animal.nextPoint_y = 0;
                        }
                        else
                        {
                            bp.animal.ai_state = 3;
                            bp.animal.totalZhen = 0;
                            bp.animal.nextPoint_x = 0;
                            bp.animal.nextPoint_y = 0;
                        }
                    }
                    else
                    {
                        if (bp.player.is_move)
                            BattlePlayers.Stop1(bp, bp.animal.r);
                        bp.player.ai_state = 0;
                        bp.player.totalZhen = 0;
                        bp.player.birth_x = bp.animal.x;
                        bp.player.birth_y = bp.animal.y;
                        bp.player.nextPoint_x = 0;
                        bp.player.nextPoint_y = 0;
                        bp.player.attack_guid = "";
                    }
                }
            }
        }
        else if (bp.animal.ai_state == 2)  //攻击
        {
            if (!BattlePlayers.players.ContainsKey(bp.player.attack_guid))
            {
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.player.attack_guid = "";
                return;
            }
            var army = BattlePlayers.players[bp.player.attack_guid];
            int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, army.animal.x, army.animal.y);
            r = BattleOperation.checkr(r);
            if (bp.animal.is_move)
                BattlePlayers.Stop1(bp, bp.animal.r);
            var t_skill = Config.get_t_skill(bp.player.skill_id);
            BattlePlayers.Release1(bp, t_skill.id, bp.animal.x, bp.animal.y, r);
            bp.animal.ai_state = 1;
            bp.animal.totalZhen = 5;
        }
        else if (bp.animal.ai_state == 3)  //移动
        {
            if (!BattlePlayers.players.ContainsKey(bp.player.attack_guid))
            {
                bp.player.ai_state = 0;
                bp.player.totalZhen = 0;
                bp.player.nextPoint_x = 0;
                bp.player.nextPoint_y = 0;
                bp.player.attack_guid = "";
                return;
            }
            var army = BattlePlayers.players[bp.player.attack_guid];
            long dis = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, army.animal.x, army.animal.y);
            int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, army.animal.x, army.animal.y);
            r = BattleOperation.checkr(r);
            BattlePlayers.Move1(bp, r);
            bp.animal.ai_state = 1;
            bp.animal.totalZhen = 0;
        }
    }

    private static void BigPenguinAI(BattleAnimalMonster bp)
    {
        if (!String.IsNullOrEmpty(bp.player.attack_guid) && BattlePlayers.players.ContainsKey(bp.player.attack_guid))
        {
            if (bp.animal.ai_state == 4 || bp.animal.ai_state == 5 || bp.animal.ai_state == 0)
            {
                if (bp.animal.is_move)
                    BattlePlayers.Stop1(bp, bp.animal.r);

                bp.animal.ai_state = 1;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                return;
            }
        }
        else if (String.IsNullOrEmpty(bp.player.attack_guid))
        {
            if (bp.animal.ai_state == 4 || bp.animal.ai_state == 5 || bp.animal.ai_state == 0)
            {
                string[] guids;
                BattleGrid.get(grid_type.et_player, bp.player.birth_x, bp.player.birth_y, out guids, bp.animal.eyeRange);
                List<BattleAnimal> relist = new List<BattleAnimal>();
                for (int i = 0; i < guids.Length; i++)
                {
                    var tp = BattlePlayers.players[guids[i]];
                    if (tp.animal.type == unit_type.Player && !tp.is_die && !tp.attr.is_yinshen())
                        relist.Add(tp);
                }
                if (relist.Count > 0)
                {
                    if (bp.animal.is_move)
                        BattlePlayers.Stop1(bp, bp.animal.r);

                    bp.animal.ai_state = 1;
                    bp.animal.totalZhen = 0;
                    bp.animal.nextPoint_x = 0;
                    bp.animal.nextPoint_y = 0;
                    return;
                }
            }
        }
        

        var t_penguin = Config.get_t_penguin(bp.player.monster_id);
        if (bp.animal.ai_state == 0)
        {
            int p = BattleOperation.random(0, 100);
            if (p < t_penguin.standp)  //静止状态
            {
                bp.animal.ai_state = 5;
                bp.animal.totalZhen = BattleOperation.random(BattlePlayers.TNUM * t_penguin.mint, BattlePlayers.TNUM * t_penguin.maxt);
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
            }
            else //移动状态
            {
                int[] ways = BattleOperation.GetPenguinPatrolPoint(bp);
                int numt = (int)BattleOperation.get_distance(bp.animal.x, bp.animal.y, ways[0], ways[1]) / bp.attr.speed();
                bp.animal.ai_state = 4;
                bp.animal.totalZhen = numt * BattlePlayers.TNUM;
                bp.animal.nextPoint_x = ways[0];
                bp.animal.nextPoint_y = ways[1];
            }
        }

        if (bp.animal.ai_state == 5) //静止
        {
            if (bp.animal.is_move)
                BattlePlayers.Stop1(bp, bp.animal.r);

            if (bp.animal.totalZhen > 0)
                bp.animal.totalZhen = bp.animal.totalZhen - 1;
            else
            {
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
            }
        }
        else if (bp.animal.ai_state == 4)
        {
            if (bp.animal.totalZhen > 0)
            {
                int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, bp.animal.nextPoint_x, bp.animal.nextPoint_y);
                r = BattleOperation.checkr(r);
                BattlePlayers.Move1(bp, r);
                bp.animal.totalZhen = bp.animal.totalZhen - 1;
            }
            else
            {
                if (bp.animal.is_move)
                    BattlePlayers.Stop1(bp, bp.animal.r);
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
            }
        }

        if (bp.animal.ai_state == 1)
        {
            if (bp.animal.totalZhen > 0)
            {
                bp.animal.totalZhen = bp.animal.totalZhen - 1;
                return;
            }

            string[] guids;
            BattleGrid.get(grid_type.et_player, bp.player.birth_x, bp.player.birth_y, out guids, bp.animal.eyeRange);
            List<BattleAnimal> relist = new List<BattleAnimal>();
            for (int i = 0; i < guids.Length; i++)
            {
                var tp = BattlePlayers.players[guids[i]];
                if (!tp.is_die && tp.animal.type == unit_type.Player && !tp.attr.is_yinshen())
                    relist.Add(tp);
            }

            relist.Sort((a, b) =>
            {
                long dis = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, a.animal.x, a.animal.y);
                long dis2 = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, b.animal.x, b.animal.y);
                if (dis > dis2)
                    return 1;
                else if (dis == dis2)
                    return 0;
                else
                    return -1;
            });

            if (relist.Count > 0)
            {
                var army = relist[0];
                var dis = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, army.animal.x, army.animal.y);
                if (dis < 100000000)
                {
                    bp.animal.ai_state = 3;
                    bp.animal.totalZhen = 0;
                    bp.player.attack_guid = army.animal.guid;
                    bp.animal.nextPoint_x = 0;
                    bp.animal.nextPoint_y = 0;
                }
                else
                {
                    bp.animal.ai_state = 2;
                    bp.animal.totalZhen = 0;
                    bp.player.attack_guid = army.animal.guid;
                    bp.animal.nextPoint_x = 0;
                    bp.animal.nextPoint_y = 0;
                }  
            }
            else
            {
                if (bp.animal.is_move)
                    BattlePlayers.Stop1(bp, bp.player.r);
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.player.attack_guid = "";
                return;
            }
        }
        else if (bp.animal.ai_state == 2)
        {
            if (!BattlePlayers.players.ContainsKey(bp.player.attack_guid))
            {
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.player.attack_guid = "";
                return;
            }
            var army = BattlePlayers.players[bp.player.attack_guid];
            long dis = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, army.animal.x, army.animal.y);
            int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, army.animal.x, army.animal.y);
            r = BattleOperation.checkr(r);
            BattlePlayers.Move1(bp, r);
            bp.animal.ai_state = 1;
            bp.animal.totalZhen = 0;
        }
        else if (bp.animal.ai_state == 3)
        {
            if (!BattlePlayers.players.ContainsKey(bp.player.attack_guid) || BattlePlayers.players[bp.player.attack_guid].animal.is_xueren)
            {
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.player.attack_guid = "";
                return;
            }
            
            var army = BattlePlayers.players[bp.player.attack_guid];
            int r = BattleOperation.get_r(bp.animal.x, bp.animal.y, army.animal.x, army.animal.y);
            r = BattleOperation.checkr(r);
            if (bp.animal.is_move)
                BattlePlayers.Stop1(bp, bp.animal.r);
            var t_skill = Config.get_t_skill(bp.player.skill_id);
            BattlePlayers.Release1(bp, t_skill.id, bp.animal.x, bp.animal.y, r);
            bp.animal.ai_state = 1;
            bp.animal.totalZhen = 5;
        }
    }

    //检测周围一个格子是否是障碍物 选取距离最近的障碍物
    private static long[] GetNearWallDis(BattleAnimal bp)
    {
        long dis = long.MaxValue;
        long[] result = null;

        for (int offset_x = -1; offset_x <= 1; offset_x++)
        {
            for (int offset_y = -1; offset_y <= 1; offset_y++)
            {
                if (offset_x != 0 || offset_y != 0)
                {
                    int x = BattleOperation.getMax(0, bp.animal.x + offset_x * 10000);
                    x = BattleOperation.getMin(bp.animal.x + offset_x * 10000,800000);
                    
                    int y = BattleOperation.getMax(0, bp.animal.y + offset_y * 10000);
                    x = BattleOperation.getMin(bp.animal.y + offset_y * 10000, 800000);

                    if (!BattleGrid.can_move(x, y))
                    {
                        long dis2 = BattleOperation.get_distance2(bp.animal.x, bp.animal.y, x, y);
                        if (dis2 < dis)
                        {
                            dis = dis2;
                            result = new long[] { x, y };
                        } 
                    }
                }
            }
        }

        return result;
    }

    public static void NewRobotAI(BattleAnimalPlayer bp)
    {
        if (bp == null)
            return;
        
        if (bp.animal.hp <= 0)
        {
            if (bp.animal.ai_state != 1)
            {
                bp.animal.ai_state = 1;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.animal.totalZhen = 0;
                bp.player.attackZhen = 0;
            }
            return;
        }
        else if (bp.animal.hp > 0 && bp.animal.ai_state == 1)
        {
            bp.animal.ai_state = 0;
            bp.animal.nextPoint_x = 0;
            bp.animal.nextPoint_y = 0;
            bp.animal.totalZhen = 0;
            bp.player.attackZhen = 0;
        }

        var t_ai_attr = Config.get_t_ai_attr(bp.animal.is_ai);
        int random_num = BattleOperation.random(0, 101);

        if (t_ai_attr.talent_p == 1)
            BattlePlayerAI.auto_add_talent(bp);

        if (bp.animal.is_xueren && (bp.animal.ai_state != 2 || bp.animal.ai_state != 3))
        {
            bp.animal.ai_state = 2;
            bp.animal.nextPoint_x = 0;
            bp.animal.nextPoint_y = 0;
            bp.animal.totalZhen = BattleOperation.random(50, 101);
            bp.player.attackZhen = 0;
            return;
        }

        if (!bp.animal.is_xueren && (bp.animal.ai_state == 2 || bp.animal.ai_state == 3))
        {
            bp.animal.ai_state = 0;
            bp.animal.nextPoint_x = 0;
            bp.animal.nextPoint_y = 0;
            bp.animal.totalZhen = 0;
            bp.player.attackZhen = 0;
            return;
        }

        if (bp.animal.ai_state == 2)
        {
            int pert = BattleOperation.toInt(bp.animal.hp * 100.0 / bp.attr.max_hp());
            if (pert >= bp.animal.totalZhen)
            {
                bp.animal.ai_state = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.animal.totalZhen = 0;
                bp.player.attackZhen = 0;
                BattlePlayers.Release1(bp, 300101, 0, 0, 0);
            }
            return;
        }
        else if (bp.animal.ai_state == 3)
        {
            if (bp.animal.totalZhen == 0)
            {
                BattlePlayers.Release1(bp, 300101, 0, 0, 0);
                bp.animal.ai_state = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.animal.totalZhen = 0;
                bp.player.attackZhen = 0;
            }
            else
                bp.animal.totalZhen = bp.animal.totalZhen - 1;
            return;
        }

        if (bp.player.attackZhen == 0)
        {
            if (random_num < t_ai_attr.attack_p)
            {
                if (bp.player.re_pre > 0)
                {
                    BattlePlayerAI.AttackCheck(bp, random_num);
                }
                else
                {
                    if (random_num < t_ai_attr.charge_attack_p)
                    {
                        if (bp.player.re_pre == 0)
                        {
                            BattlePlayers.PreRelease1(bp, 1);
                        }
                    }
                    else
                    {
                        BattlePlayerAI.AttackCheck(bp, random_num);
                    }
                }
            }
            bp.player.attackZhen = bp.player.attackZhen + 1;
        }
        else if (bp.player.attackZhen < 8 && bp.player.attackZhen > 0)
            bp.player.attackZhen = bp.player.attackZhen + 1;
        else
            bp.player.attackZhen = 0;

        if (bp.animal.ai_state > 0)
        {
            if (bp.animal.ai_state == 4)
            {
                var re = BattlePlayerAI.IsCloseTargetPoint(bp);
                if (bp.animal.totalZhen > 0 && !re)
                {
                    bp.animal.totalZhen = bp.animal.totalZhen - 1;
                    return;
                }
                else
                {
                    if (re)
                    {
                        bp.player.birth_x = bp.animal.nextPoint_x;
                        bp.player.birth_y = bp.animal.nextPoint_y;
                    }
                    bp.animal.ai_state = 0;
                    bp.animal.nextPoint_x = 0;
                    bp.animal.nextPoint_y = 0;
                    bp.animal.totalZhen = 0;
                }
            }
            else if (bp.animal.ai_state == 5)
            {
                var re = BattlePlayerAI.IsCloseTargetPoint(bp);
                //Debug.Log(BattlePlayers.zhen + "," + bp.animal.guid + "," + bp.animal.ai_state + "," + bp.animal.totalZhen + "," + bp.animal.attack_state + "," + bp.animal.attackZhen + "," + bp.animal.nextPoint_x + "," + bp.animal.nextPoint_y + "," + bp.animal.x + "," + bp.animal.y+","+re);
                if (bp.animal.totalZhen > 0 && !re)
                {
                    bp.animal.totalZhen = bp.animal.totalZhen - 1;
                    return;
                }
                else
                {
                    bp.animal.ai_state = 0;
                    bp.animal.nextPoint_x = 0;
                    bp.animal.nextPoint_y = 0;
                    bp.animal.totalZhen = 0;
                }
            }
            else if (bp.animal.ai_state == 6)
            {
                //Debug.Log(BattlePlayers.zhen + "," + bp.animal.guid + "," + bp.animal.ai_state + "," + bp.animal.totalZhen + "," + bp.animal.attack_state + "," + bp.animal.attackZhen + "," + bp.animal.nextPoint_x + "," + bp.animal.nextPoint_y + "," + bp.animal.x + "," + bp.animal.y);
                if (bp.animal.totalZhen > 0)
                {
                    bp.animal.totalZhen = bp.animal.totalZhen - 1;
                    return;
                }
                else
                {
                    bp.animal.ai_state = 0;
                    bp.animal.nextPoint_x = 0;
                    bp.animal.nextPoint_y = 0;
                    bp.animal.totalZhen = 0;
                }
            }
        }

        if (BattlePlayerAI.AvoidMeteor(bp) || BattlePlayerAI.ChangeSnowState(bp))
            return;

        if (bp.animal.ai_state == 0)
        {
            if (random_num < t_ai_attr.stand_p)
            {
                //Debug.Log(BattlePlayers.zhen + "," + bp.animal.guid + "," + bp.animal.ai_state + "," + bp.animal.totalZhen + "," + bp.animal.attack_state + "," + bp.animal.attackZhen + "," + bp.animal.nextPoint_x + "," + bp.animal.nextPoint_y + "," + bp.animal.x + "," + bp.animal.y);
                bp.animal.ai_state = 6;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.animal.totalZhen = BattleOperation.random(t_ai_attr.stand_min, t_ai_attr.stand_max) * BattlePlayers.TNUM;
                BattlePlayers.Stop1(bp, bp.animal.r);
                if (bp.player.re_pre > 0)
                    BattlePlayers.PreRelease1(bp, 0);
            }
            else
            {
                var player_list = BattlePlayerAI.FindOtherPlayers(bp);
                if (player_list.Count == 0)
                {
                    var t_point = BattleOperation.GetAccessPatrolPoint(bp);
                    if (t_point != null)
                    {
                        //Debug.Log(bp.animal.guid + "," + bp.animal.ai_state + "," + bp.animal.totalZhen + "," + bp.animal.attack_state + "," + bp.animal.attackZhen + "," + bp.animal.nextPoint_x + "," + bp.animal.nextPoint_y + "," + bp.animal.x + "," + bp.animal.y);
                        bp.animal.ai_state = 4;
                        bp.animal.nextPoint_x = t_point[0];
                        bp.animal.nextPoint_y = t_point[1];
                        bp.animal.totalZhen = BattleOperation.random(20, 27);
                        var r = BattleOperation.get_r(bp.animal.x, bp.animal.y, bp.animal.nextPoint_x, bp.animal.nextPoint_y);
                        r = BattleOperation.checkr(r);
                        BattlePlayers.Move1(bp, r);
                    }
                }
                else
                {
                    List<BattleAnimal> robot_list = new List<BattleAnimal>();
                    List<BattleAnimal> hum_list = new List<BattleAnimal>();
                    for (int i = 0; i < player_list.Count; i++)
                    {
                        if (Convert.ToUInt64(player_list[i].animal.guid) > 100)
                            hum_list.Add(player_list[i]);
                        else
                            robot_list.Add(player_list[i]);
                    }
                    if (hum_list.Count > 0)
                    {
                        BattlePlayerAI.bubblingHp(hum_list);
                        bp.animal.ai_state = 5;
                        bp.animal.nextPoint_x = hum_list[0].animal.x;
                        bp.animal.nextPoint_y = hum_list[0].animal.y;
                        bp.animal.totalZhen = BattleOperation.random(20, 25);
                        //Debug.Log(BattlePlayers.zhen + "," + bp.animal.guid + "," + bp.animal.ai_state + "," + bp.animal.totalZhen + "," + bp.animal.attack_state + "," + bp.animal.attackZhen + "," + bp.animal.nextPoint_x + "," + bp.animal.nextPoint_y + "," + bp.animal.x + "," + bp.animal.y);
                    }
                    else
                    {
                        BattlePlayerAI.bubblingHp(robot_list);
                        bp.animal.ai_state = 5;
                        bp.animal.nextPoint_x = robot_list[0].animal.x;
                        bp.animal.nextPoint_y = robot_list[0].animal.y;
                        bp.animal.totalZhen = BattleOperation.random(20, 25);
                        //Debug.Log(BattlePlayers.zhen + "," + bp.animal.guid + "," + bp.animal.ai_state + "," + bp.animal.totalZhen + "," + bp.animal.attack_state + "," + bp.animal.attackZhen + "," + bp.animal.nextPoint_x + "," + bp.animal.nextPoint_y + "," + bp.animal.x + "," + bp.animal.y);
                    }

                    if (!BattleOperation.check_distance(bp.animal.x, bp.animal.y, bp.animal.nextPoint_x, bp.animal.nextPoint_y, 15000))
                    {
                        var r = BattleOperation.get_r(bp.animal.x, bp.animal.y, bp.animal.nextPoint_x, bp.animal.nextPoint_y);
                        r = (r + 180) % 360;
                        r = BattleOperation.checkr(r);
                        BattlePlayers.Move1(bp, r);
                    }
                    else
                    {
                        var r = BattleOperation.get_r(bp.animal.x, bp.animal.y, bp.animal.nextPoint_x, bp.animal.nextPoint_y);
                        r = BattleOperation.checkr(r);
                        BattlePlayers.Move1(bp, r);
                    }   
                }
            }
        }
    }

    public static string getRandomStr()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < BattleOperation.rands.Count; i++)
            sb.Append(BattleOperation.rands[i] + ",");
        return sb.ToString();
    }

    public static bool IsCloseTargetPoint(BattleAnimal bp, int? dis = null)
    {
        if (dis == null)
        {
            if (Mathf.Abs(bp.animal.x - bp.animal.nextPoint_x) <= 500 && Mathf.Abs(bp.animal.y - bp.animal.nextPoint_y) <= 500)
                return true;
            else
                return false;
        }
        else
        {
            if (Mathf.Abs(bp.animal.x - bp.animal.nextPoint_x) <= dis.GetValueOrDefault() && Mathf.Abs(bp.animal.y - bp.animal.nextPoint_y) <= dis.GetValueOrDefault())
                return true;
            else
                return false;
        }
    }

    public static bool ChangeSnowState(BattleAnimalPlayer bp)
    {
        int bloodPert = BattleOperation.toInt(bp.animal.hp * 100.0 / bp.attr.max_hp());
        int succ_t = BattleOperation.random(50, 100);
        var t_ai_attr = Config.get_t_ai_attr(bp.animal.is_ai);
        if (bloodPert <= 30 && succ_t < t_ai_attr.snow_p && bp.animal.is_ai > 0)
        {
            var cds = BattlePlayerAI.IsCd(bp, Config.get_t_skill(300101));
            if (!cds)
            {
                BattlePlayers.Release1(bp, 300101, 0, 0, 0);
                if (!bp.animal.is_xueren)
                    return false;
                else
                {
                    bp.animal.ai_state = 2;
                    bp.animal.nextPoint_x = 0;
                    bp.animal.nextPoint_y = 0;
                    bp.animal.totalZhen = BattleOperation.random(50, 101);
                    bp.player.attackZhen = 0;
                    BattlePlayers.Stop1(bp, bp.animal.r);
                    return true;
                }
            }
        }
        return false;
    }

    public static bool AvoidMeteor(BattleAnimalPlayer bp)
    {
        List<string> fires = new List<string>();
        string[] tids;
        BattleGrid.get(grid_type.et_effect, bp.animal.x, bp.animal.y, out tids, 5);
        for (int i = 0; i < tids.Length; i++)
        {
            int guid = Convert.ToInt32(tids[i]);
            var be = BattlePlayers.effects[guid];
            if (be.effect.id.ToString().Substring(0, 4) == "2009" && be.effect.re_guid != bp.animal.guid && be.effect.camp != bp.animal.camp)
            {
                var t_skill_effect = Config.get_t_skill_effect(be.effect.id);
                if (Regex.IsMatch(t_skill_effect.id.ToString(), "^2009%d%d01$"))
                    t_skill_effect = Config.get_t_skill_effect(t_skill_effect.link_effect);

                if (!BattleOperation.check_distance(bp.animal.x, bp.animal.y, be.effect.x, be.effect.y, bp.ur() + t_skill_effect.get_range_param(be.re_bp, be.effect.pre_zhen)))
                    fires.Add(tids[i]);
            }
        }

        if (fires.Count > 0)
        {
            bool cds = BattlePlayerAI.IsCd(bp, Config.get_t_skill(300101));
            int succ_t = BattleOperation.random(0, 101);
            var t_ai_attr = Config.get_t_ai_attr(bp.animal.is_ai);
            if (!cds && succ_t < t_ai_attr.fire_p)
            {
                BattlePlayers.Release1(bp, 300101, 0, 0, 0);
                if (!bp.animal.is_xueren)
                    return false;
                else
                {
                    bp.animal.ai_state = 3;
                    bp.animal.nextPoint_x = 0;
                    bp.animal.nextPoint_y = 0;
                    bp.animal.totalZhen = BattleOperation.random(60, 130);
                    bp.player.attackZhen = 0;
                    BattlePlayers.Stop1(bp, bp.animal.r);
                    return true;
                }
            }
        }
        return false;
    }

    public static void BossLifeCycle()
    {
        if (Battle.is_newplayer_guide)
            return;

        var t = (BattlePlayers.zhen - BattlePlayers.Boss.player.boss_birth_time) * BattlePlayers.TICK;
        var t_boss_attr = Config.get_t_boss_attr(BattlePlayers.Boss.animal.camp);
        if (!BattlePlayers.BossIsActive())
        {
            if (t >= t_boss_attr.refresh_t * 1000)
                BattlePlayers.Fuhuo(BattlePlayers.Boss);

            if (!BattlePlayers.jiasu)
            {
                if (BattlePlayers.Boss.is_die && t >= (t_boss_attr.refresh_t - 10) * 1000 && t < (t_boss_attr.refresh_t * 1000))
                {
                    if (BattlePlayerAI.IsOnce)
                    {
                        int num = BattleOperation.toInt((t_boss_attr.refresh_t * 1000 - t) * 1.0 / 1000);
                        Battle.battle_panel.show_boss_tips(Config.get_t_script_str("BattlePanel_029"), num, true);
                        BattlePlayerAI.BossTips = true;
                        BattlePlayerAI.IsOnce = false;
                    }
                    else if (!BattlePlayerAI.IsOnce && !BattlePlayerAI.BossTips)
                    {
                        int num = BattleOperation.toInt((t_boss_attr.refresh_t * 1000 - t) * 1.0 / 1000);
                        Battle.battle_panel.show_boss_tips(Config.get_t_script_str("BattlePanel_029"), num, true);
                        BattlePlayerAI.BossTips = true;
                    }
                }
                else
                {
                    BattlePlayerAI.BossTips = false;
                }
            }
        }
        else
        {
            var final_t = t_boss_attr.life_t * 1000;
            var refresh_t = final_t - 10000;
            if (t == final_t)
            {
                BattlePlayers.Boss.animal.x = 18000000;
                BattlePlayers.Boss.animal.y = 18000000;
                BattlePlayers.Boss.set_hp(0);
                BattlePlayers.Boss.is_die = true;
            }

            if (!BattlePlayers.jiasu)
            {
                if (!BattlePlayers.Boss.is_die && t >= refresh_t && t < final_t)
                {
                    if (BattlePlayerAI.IsOnce)
                    {
                        int num = BattleOperation.toInt((final_t - t) * 1.0 / 1000);
                        Battle.battle_panel.show_boss_tips(Config.get_t_script_str("BattlePanel_030"), num);
                        BattlePlayerAI.BossTips = true;
                        BattlePlayerAI.IsOnce = false;
                    }
                    else if (!BattlePlayerAI.IsOnce && !BattlePlayerAI.BossTips)
                    {
                        int num = BattleOperation.toInt((final_t - t) * 1.0 / 1000);
                        Battle.battle_panel.show_boss_tips(Config.get_t_script_str("BattlePanel_030"), num);
                        BattlePlayerAI.BossTips = true;
                    }
                }
                else
                {
                    BattlePlayerAI.BossTips = false;
                }
            }
        }
    }

    public static void BossAI(BattleAnimalBoss bp)
    {
        if (bp.animal.hp <= 0)
        {
            if (bp.animal.ai_state != 0)
            {
                bp.animal.ai_state = 0;
                bp.animal.totalZhen = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.player.attack_state = 0;
                bp.player.attackZhen = 0;
            }
            return;
        }

        if (bp.animal.ai_state == 0)
        {
            if (bp.player.attack_state != 0)
            {
                bp.player.attack_state = 0;
                bp.player.attackZhen = 0;
            }
            return;
        }

        if (bp.animal.ai_state == 1 && bp.player.attack_state != 0)
        {
            bp.player.attack_state = 0;
            bp.player.attackZhen = 0;
        }

        if (bp.player.attack_state > 0)
        {
            if (bp.animal.ai_state == 2)
            {
                var r = BattleOperation.get_r(bp.animal.x, bp.animal.y, bp.animal.nextPoint_x, bp.animal.nextPoint_y);
                if (bp.player.attack_state != 4 && bp.player.attack_state != 8 && bp.player.attackZhen == BattlePlayerAI.t_attack_action_1)
                {
                    BattlePlayers.Release1(bp, 400101, bp.animal.nextPoint_x, bp.animal.nextPoint_y, r);
                    bp.player.attackZhen = bp.player.attackZhen - 1;
                }
                else if (bp.player.attack_state == 4 && bp.player.attackZhen == BattlePlayerAI.t_attack_action_2)
                {
                    bp.player.skill_id = 400201;
                    bp.player.skill_level = 1;
                    BattlePlayers.Release1(bp, 400201, bp.animal.nextPoint_x, bp.animal.nextPoint_y, r);
                    bp.player.attackZhen = bp.player.attackZhen - 1;
                }
                else if (bp.player.attack_state == 8 && bp.player.attackZhen == BattlePlayerAI.t_attack_action_3)
                {
                    bp.player.skill_id = 400301;
                    bp.player.skill_level = 1;
                    BattlePlayers.Release1(bp, 400301, bp.animal.nextPoint_x, bp.animal.nextPoint_y, r);
                    bp.player.attackZhen = bp.player.attackZhen - 1;
                }
                else if (bp.player.attackZhen > 0)
                    bp.player.attackZhen = bp.player.attackZhen - 1;
                else if (bp.player.attackZhen == 0)
                {
                    bp.player.attack_state = (bp.player.attack_state + 1) % 9;
                    if (bp.player.attack_state == 0)
                        bp.player.attack_state = 1;
                    if (bp.player.attack_state == 4)
                        bp.player.attackZhen = BattlePlayerAI.t_attack_action_2;
                    else if (bp.player.attack_state == 8)
                        bp.player.attackZhen = BattlePlayerAI.t_attack_action_3;
                    else
                        bp.player.attackZhen = BattlePlayerAI.t_attack_action_1;
                }
            }
        }

        if(bp.animal.ai_state == 2)
        {
            if (bp.animal.totalZhen > 0)
                bp.animal.totalZhen = bp.animal.totalZhen - 1;
            else
            {
                bp.animal.ai_state = 1;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.animal.totalZhen = 0;
            }
        }

        if (bp.animal.ai_state == 1)
        {
            var player_list = BattlePlayerAI.FindOtherPlayers(bp);
            if (player_list.Count == 0)
            {
                bp.animal.ai_state = 0;
                bp.animal.nextPoint_x = 0;
                bp.animal.nextPoint_y = 0;
                bp.animal.totalZhen = 0;
                bp.player.attack_state = 0;
                bp.player.attackZhen = 0;
                BattlePlayers.action(bp, "ready02");
                return;
            }
            else
            {
                bp.animal.nextPoint_x = player_list[0].animal.x;

                bp.animal.nextPoint_y = player_list[0].animal.y;

                bp.animal.ai_state = 2;

                if (bp.player.attack_state == 0)
                {
                    bp.player.attack_state = 1;
                    bp.player.attackZhen = BattlePlayerAI.t_attack_action_1;
                }

                if (bp.player.attack_state == 4)
                    bp.animal.totalZhen = BattlePlayerAI.t_attack_action_2;
                else if (bp.player.attack_state == 8)
                    bp.animal.totalZhen = BattlePlayerAI.t_attack_action_3;
                else
                    bp.animal.totalZhen = BattlePlayerAI.t_attack_action_1;
            }
        }
    }
}
