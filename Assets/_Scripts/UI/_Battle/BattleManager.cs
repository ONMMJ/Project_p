using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuff
{
    public int skillTurnCount;       // 스킬이 적용되는 남은 턴 수
    public Sprite icon;
    public bool IsEnd()
    {
        skillTurnCount--;
        if (skillTurnCount <= 0)
            return true;
        else
            return false;
    }
}
public class StatusBuffInfo : BaseBuff         // 상태이상 정보
{
    public bool isProvocation = false;      // 도발중인가?
    public bool isStun = false;    // 스턴중인가?
    public bool isReverseElementStat = false;   // 속성 반전인가?

    public StatusBuffInfo(int count)
    {
        skillTurnCount = count;
    }

}

public class BuffInfo : BaseBuff
{

    public float buffVit;
    public float buffAtk;
    public float buffDef;
    public float buffDex;

    public BuffInfo(int count, float vit, float atk, float def, float dex)
    {
        this.skillTurnCount = count;
        this.buffVit = vit;
        this.buffAtk = atk;
        this.buffDef = def;
        this.buffDex = dex;
    }

    public void SetUp(BuffInfo buffInfo)
    {
        this.skillTurnCount = buffInfo.skillTurnCount;
        this.buffVit = buffInfo.buffVit;
        this.buffAtk = buffInfo.buffAtk;
        this.buffDef = buffInfo.buffDef;
        this.buffDex = buffInfo.buffDex;
    }

}

public class DotDamageInfo : BaseBuff
{
    public Pet attacker;
    public float damagePer;

    public DotDamageInfo(Pet attacker, float damagePer, int skillTurnCount)
    {
        this.attacker = attacker;
        this.skillTurnCount = skillTurnCount;
        this.damagePer = damagePer;
    }

}

public class HealInfo : BaseBuff
{
    public Pet healer;
    public float healPer;

    public HealInfo(Pet healer, float healPer, int skillTurnCount)
    {
        this.healer = healer;
        this.skillTurnCount = skillTurnCount;
        this.healPer = healPer;
    }

}

public class BattleManager : SingletonReset<BattleManager>
{
    // 도트 데미지
    public void GetDotDamage(Pet attacker, Pet target, float damagePer, int count)
    {
        DotDamageInfo dotDamage = new DotDamageInfo(attacker, damagePer, count);
        target.AddDotDamage(dotDamage);
    }
    public void GetDotDamage(Pet attacker, List<Pet> targets, float damagePer, int count)
    {
        foreach (Pet target in targets)
        {
            DotDamageInfo dotDamage = new DotDamageInfo(attacker, damagePer, count);
            target.AddDotDamage(dotDamage);
        }
    }

    // 버프
    public void GetBuff(Pet target, SkillInfo skillInfo)
    {
        BuffInfo buff = new BuffInfo(skillInfo.skillTurnCount, skillInfo.buffVit, skillInfo.buffAtk, skillInfo.buffDef, skillInfo.buffDex);
        target.petInfo.ApplyBuff(buff);
        target.AddBuff(buff);
    }
    public void GetBuff(List<Pet> targets, SkillInfo skillInfo)
    {
        foreach (Pet target in targets)
        {
            BuffInfo buff = new BuffInfo(skillInfo.skillTurnCount, skillInfo.buffVit, skillInfo.buffAtk, skillInfo.buffDef, skillInfo.buffDex);
            target.petInfo.ApplyBuff(buff);
            target.AddBuff(buff);
        }
    }

    // 스턴
    public void GetStun(Pet target, int count)
    {
        StatusBuffInfo buff = new StatusBuffInfo(count);
        target.isStun = true;
        buff.isStun = true;
        target.animator.SetBool("IsStun", true);
        target.AddStatusBuff(buff);
    }
    public void GetStun(List<Pet> targets, int count)
    {
        foreach (Pet target in targets)
        {
            StatusBuffInfo buff = new StatusBuffInfo(count);
            target.isStun = true;
            buff.isStun = true;
            target.animator.SetBool("IsStun", true);
            target.AddStatusBuff(buff);
        }
    }

    // 속성 반전
    public void GetReverseElementStat(Pet target, int count)
    {
        StatusBuffInfo buff = new StatusBuffInfo(count);
        target.isReverseElementStat = true;
        buff.isReverseElementStat = true;
        target.AddStatusBuff(buff);
    }
    public void GetReverseElementStat(List<Pet> targets, int count)
    {
        foreach (Pet target in targets)
        {
            StatusBuffInfo buff = new StatusBuffInfo(count);
            target.isReverseElementStat = true;
            buff.isReverseElementStat = true;
            target.AddStatusBuff(buff);
        }
    }

    // 도발
    public void GetProvocation(Pet target, int count)
    {
        StatusBuffInfo buff = new StatusBuffInfo(count);
        target.isProvocation = true;
        buff.isProvocation = true;
        target.AddStatusBuff(buff);
    }

    // 회복
    public void GetDotHeal(Pet healer, Pet target, float healPer, int count)
    {
        HealInfo healInfo = new HealInfo(healer, healPer, count);
        target.AddHeal(healInfo);
    }
    public void GetDotHeal(Pet healer, List<Pet> targets, float healPer, int count)
    {
        foreach (Pet target in targets)
        {
            HealInfo healInfo = new HealInfo(healer, healPer, count);
            target.AddHeal(healInfo);
        }
    }
}
