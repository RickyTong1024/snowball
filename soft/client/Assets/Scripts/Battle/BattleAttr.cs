using BattleDB;
using protocol.game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CAttr
{
    public int? speed;
}


public class AnimalAttr
{
    protected BattleAnimal bp;
    protected t_role t_role;
    public AnimalAttr(BattleAnimal player)
    {
        bp = player;
        t_role = Config.get_t_role(bp.animal.role_id);
    }
    public virtual int init_max_hp()
    {
        var tmp = t_role.hp;
        return tmp;
    }
    public virtual int max_hp()
    {
        var tmp = t_role.hp;
        tmp = tmp + bp.attr_value[1] + bp.attr_value[47 + t_role.sex];
        tmp = tmp + BattleOperation.toInt(tmp * (bp.attr_value[2] + bp.attr_value[49 + t_role.sex]) / 100.0);
        return tmp;
    }
    public virtual int init_atk()
    {
        var tmp = t_role.atk;
        return tmp;
    }
    public virtual int atk()
    {
        var tmp = t_role.atk;
        tmp = tmp + bp.attr_value[3] + bp.attr_value[51 + t_role.sex];
        if (bp.attr_value[73] != 0)
            tmp = tmp + BattleOperation.toInt((bp.attr.max_hp() - bp.animal.hp) * 5.0 / bp.attr.max_hp()) * bp.attr_value[73];
        var per = 1 + (bp.attr_value[4] + bp.attr_value[53 + t_role.sex]) / 100.0;
        if (bp.attr_value[41] != 0)
            per = per + BattleOperation.toInt((bp.attr.max_hp() - bp.animal.hp) * 5.0 / bp.attr.max_hp()) * bp.attr_value[41] / 100.0;
        tmp = BattleOperation.toInt(tmp * per);
        return tmp;
    }
    public virtual int init_def()
    {
        var tmp = t_role.def;
        return tmp;
    }
    public virtual int def()
    {
        var tmp = t_role.def;
        tmp = tmp + bp.attr_value[5] + bp.attr_value[55 + t_role.sex];
        tmp = tmp + BattleOperation.toInt(tmp * (bp.attr_value[6] + bp.attr_value[57 + t_role.sex]) / 100.0);
        return tmp;
    }
    public virtual double defper()
    {
        var tmp = t_role.def;
        tmp = tmp + bp.attr_value[5] + bp.attr_value[55 + t_role.sex];
        tmp = tmp + BattleOperation.toInt(tmp * (bp.attr_value[6] + bp.attr_value[57 + t_role.sex]) / 100.0);
        return tmp;
    }
    public virtual int init_range()
    {
        return t_role.range;
    }
    public virtual int range()
    {
        var tmp = t_role.range;
        tmp = tmp + bp.attr_value[12] + bp.attr_value[63 + t_role.sex];
        tmp = tmp + BattleOperation.toInt(tmp * (bp.attr_value[13] + bp.attr_value[65 + t_role.sex]) / 100.0);
        if (tmp >= 150000)
            tmp = 150000;
        return tmp;
    }
    public int init_speed()
    {
        return t_role.speed;
    }
    public virtual int speed()
    {
        var tmp = bp.attr.speed1();
        return tmp;
    }
    public virtual int speed1()
    {
        if (bp.cattr.speed != null)
            return bp.cattr.speed.GetValueOrDefault();

        int tmp = t_role.speed;
        tmp = tmp + bp.attr_value[7] + bp.attr_value[59 + t_role.sex];
        tmp = tmp + BattleOperation.toInt(tmp * (bp.attr_value[8] + bp.attr_value[61 + t_role.sex]) / 100.0);
        if (tmp >= 100000)
            tmp = 100000;
        else if (tmp <= 10000)
            tmp = 10000;
        bp.cattr.speed = tmp;
        return tmp;
    }
    public virtual int aspeed()
    {
        return t_role.aspeed;
    }
    public virtual int zs()
    {
        return bp.attr_value[14] + bp.attr_value[67 + t_role.sex];
    }
    public virtual int js()
    {
        return bp.attr_value[15] + bp.attr_value[69 + t_role.sex];
    }

    public virtual bool is_zhimang()
    {
        return bp.attr_value[101] > 0;
    }

    public virtual bool is_stun()
    {
        return bp.attr_value[102] > 0;
    }

    public virtual bool is_hunluan()
    {
        return bp.attr_value[103] > 0;
    }

    public virtual bool is_bing()
    {
        return bp.attr_value[104] > 0;
    }

    public virtual bool is_wudi()
    {
        return bp.attr_value[105] > 0;
    }

    public virtual bool is_wudicd()
    {
        return bp.attr_value[106] > 0;
    }

    public virtual bool is_yinshen()
    {
        return bp.attr_value[107] > 0;
    }
}

public class AnimalBossAttr : AnimalAttr
{
    protected BattleAnimalBoss aba;
    public AnimalBossAttr(BattleAnimalBoss player) : base(player)
    {
        aba = player;
        t_role = Config.get_t_role(aba.animal.role_id);
    }
}

public class AnimalMonsterAttr : AnimalAttr
{
    protected BattleAnimalMonster bam;
    public AnimalMonsterAttr(BattleAnimalMonster player) : base(player)
    {
        bam = player;
        t_role = Config.get_t_role(bam.animal.role_id);
    }
}

public class AnimalPlayerAttr : AnimalAttr
{
    protected BattleAnimalPlayer bap;
    public AnimalPlayerAttr(BattleAnimalPlayer player) : base(player)
    {
        bap = player;
        t_role = Config.get_t_role(bap.animal.role_id);
    }
    public override int init_max_hp()
    {
        var tmp = t_role.hp + (bap.player.role_level - 1) * t_role.hp_add;
        tmp = BattleOperation.toInt(tmp * (0.99 + 0.01 * bap.player.level));
        return tmp;
    }

    public override int max_hp()
    {
        var tmp = t_role.hp + (bap.player.role_level - 1) * t_role.hp_add;
        tmp = BattleOperation.toInt(tmp * (99 + bap.player.level) / 100.0);
        tmp = tmp + bap.attr_value[1] + bap.attr_value[47 + t_role.sex] + (bap.attr_value[76] * bap.player.exp / 10000) + bap.player.lattr_value[0];
        tmp = tmp + BattleOperation.toInt(tmp * (bap.attr_value[2] + bap.attr_value[49 + t_role.sex] + bap.player.lattr_value[1]) / 100.0);
        return tmp;
    }

    public override int init_atk()
    {
        var tmp = t_role.atk + (bap.player.role_level - 1) * t_role.atk_add;
        tmp = BattleOperation.toInt(tmp * (99 + bap.player.level) / 100.0);
        return tmp;
    }

    public override int atk()
    {
        var tmp = t_role.atk + (bap.player.role_level - 1) * t_role.atk_add;
        tmp = BattleOperation.toInt(tmp * (99 + bap.player.level) / 100.0);
        tmp = tmp + bap.attr_value[3] + bap.attr_value[51 + t_role.sex] + (bap.attr_value[75] * bap.player.exp / 10000) + bap.player.lattr_value[2];
        if (bap.attr_value[73] != 0)
            tmp = tmp + BattleOperation.toInt((bap.attr.max_hp() - bap.player.hp) * 5.0 / bap.attr.max_hp()) * bap.attr_value[73];
        var per = 1 + (bap.attr_value[4] + bap.attr_value[53 + t_role.sex] + bap.player.lattr_value[3]) / 100.0;
        if (bap.attr_value[41] != 0)
            per = per + BattleOperation.toInt((bap.attr.max_hp() - bap.player.hp) * 5.0 / bap.attr.max_hp()) * bap.attr_value[41] / 100.0;
        tmp = BattleOperation.toInt(tmp * per);
        return tmp;
    }

    public override int init_def()
    {
        var tmp = t_role.def + (bap.player.role_level - 1) * t_role.def_add;
        tmp = BattleOperation.toInt(tmp * (0.99 + 0.01 * bap.player.level));
        return tmp;
    }

    public override int def()
    {
        var tmp = t_role.def + (bap.player.role_level - 1) * t_role.def_add;
        tmp = BattleOperation.toInt(tmp * (99 + bap.player.level) / 100.0);
        tmp = tmp + bap.attr_value[5] + bap.attr_value[55 + t_role.sex] + bap.player.lattr_value[4];

        tmp = tmp + BattleOperation.toInt(tmp * (bap.attr_value[6] + bap.attr_value[57 + t_role.sex] + bap.player.lattr_value[5]) / 100.0);
        return tmp;
    }

    public override double defper()
    {
        var tmp = bap.attr.def();
        return (tmp / (100 + tmp) * 1000) / 10.0;
    }
    public override int init_range()
    {
        return t_role.range;
    }

    public override int range()
    {
        var tmp = t_role.range;
        tmp = tmp + bap.attr_value[12] + bap.attr_value[63 + t_role.sex];
        tmp = tmp + BattleOperation.toInt(tmp * (bap.attr_value[13] + bap.attr_value[65 + t_role.sex]) / 100.0);
        if (tmp >= 150000)
            tmp = 150000;
        return tmp;
    }

    public override int speed()
    {
        var tmp = bap.attr.speed1();
        if (bap.is_fight())
            tmp = BattleOperation.toInt(tmp * 0.9);
        return tmp;
    }

    public override int speed1()
    {
        if (bap.cattr.speed != null)
            return bap.cattr.speed.GetValueOrDefault();

        int tmp = t_role.speed;
        tmp = tmp + bap.attr_value[7] + bap.attr_value[59 + t_role.sex] + bap.player.lattr_value[6];
        tmp = tmp + BattleOperation.toInt(tmp * (bap.attr_value[8] + bap.attr_value[61 + t_role.sex] + bap.player.lattr_value[7]) / 100.0);
        if (tmp >= 100000)
            tmp = 100000;
        else if (tmp <= 10000)
            tmp = 10000;
        bap.cattr.speed = tmp;
        return tmp;
    }
}

