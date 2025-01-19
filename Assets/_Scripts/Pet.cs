using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;


public class Pet : MonoBehaviour
{
    public enum PETSTATUS
    {
        P_IDLE, //대기
        P_BATTLE //배틀
    }
    public enum PET_BATTLE_STATUS
    {
        ATTACK, //기본 공격
        SKILL,  //스킬
        DIE     //죽음
    }
    public Transform startPos;

    public int skillMaxPoint => petInfo.skillMaxPoint;   // 최대로 모을 수 있는 스킬 포인트 수
    public int skillPoint;
    public bool isAttack;

    public PET_BATTLE_STATUS petStatus;

    public float moveSpeed;
    public PetInfo petInfo;

    List<StatusBuffInfo> statusBuffList;
    List<BuffInfo> buffList;
    List<DotDamageInfo> dotDamageList;
    List<HealInfo> dotHealList;
    List<BaseSkill> skills;
    public List<BaseSkill> Skills => skills;

    public Animator animator { get; private set; }
    public bool isAttackAnimation; 

    public float maxHp => petInfo.Vit;
    public float atk => petInfo.Atk;
    public float def => petInfo.Def;
    public float dex => petInfo.Dex;
    public float nowHp;

    public bool isProvocation = false;
    public bool isStun = false;
    public bool isReverseElementStat = false;

    public bool IsEnemy => petInfo.isEnemy;
    public EnemyInfo enemyInfo;

    private StatusBar statusBar;

    public Pet target;
    public BaseSkill selectSkill { get; private set; }
    public BaseAttack baseAttack { get; private set; }

    public int prevNowExp { get; private set; }
    public int prevLevel { get; private set; }
    public int receivedExp { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        baseAttack = GetComponent<BaseAttack>();
        statusBuffList = new List<StatusBuffInfo>();
        buffList = new List<BuffInfo>();
        dotDamageList = new List<DotDamageInfo>();
        dotHealList = new List<HealInfo>();
        isAttack = false;
    }

    public void Setup(PetInfo petInfo, EnemyInfo enemyInfo)
    {
        this.petInfo = petInfo;
        this.enemyInfo = enemyInfo;
        this.prevLevel = petInfo.petLevel;
        this.prevNowExp = petInfo.petNowExp;
        this.receivedExp = 0;
        skills = GetComponents<BaseSkill>()?.ToList();

        petStatus = PET_BATTLE_STATUS.ATTACK;
        statusBar = GameUI.Instance.GetStatusBar(BattleCanvas.Instance.Pivot);
        statusBar.SetLevel(petInfo.petLevel);
        statusBar.SetElementStat(petInfo.petDefault.elementStat);
        nowHp = maxHp;

        // 스킬포인트가 변화되면 UI에 실시간 적용
        this.UpdateAsObservable()
            .Select(x => skillPoint)
            .DistinctUntilChanged()
            .Subscribe(x => { statusBar.UpdateSkillPoint(skillPoint); })
            .AddTo(this);
        // Hp가 변화되면 UI에 실시간 적용
        this.UpdateAsObservable()
            .Select(x => nowHp)
            .DistinctUntilChanged()
            .Subscribe(x => { statusBar.SetHp(nowHp, maxHp); })
            .AddTo(this);
        // Level이 변화되면 UI에 실시간 적용
        this.UpdateAsObservable()
            .Select(x => petInfo.petLevel)
            .DistinctUntilChanged()
            .Subscribe(x => { statusBar.SetLevel(petInfo.petLevel); })
            .AddTo(this);
        // elementStat이 변화되면 UI에 실시간 적용
        this.UpdateAsObservable()
            .Select(x => isReverseElementStat)
            .DistinctUntilChanged()
            .Subscribe(x => {
                int[] finalElementStat = FinalElementStat();
                statusBar.SetElementStat(finalElementStat);
            })
            .AddTo(this);
        // 게임속도가 변화되면 애니메이션 속도를 실시간 적용
        this.UpdateAsObservable()
            .Select(x => GameManager.Instance.speedGrade)
            .DistinctUntilChanged()
            .Subscribe(x => { animator.SetFloat("Speed", GameManager.Instance.speedGrade); } )
            .AddTo(this);


        skillPoint = petInfo.startSkillPoint;
    }

    public void Update()
    {
        Vector3 hpBarPosition = transform.position;
        hpBarPosition.y += GetComponent<BoxCollider>().size.y/2 + 1.5f;
        statusBar.SetPosition(hpBarPosition);

        // Damage 애니메이션이 종료되면 Idle 애니메이션으로 이동
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
                animator.SetTrigger("OnEndDamage");
        }
    }

    public void SetPosition(Transform startPos)
    {
        this.startPos = startPos;
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;
    }
    public void SetBattle()
    {
        animator.SetBool("IsBattle", true);
    }
    public bool GetDamaged(float dmg, Pet attacker) //isDie?
    {
        if (nowHp <= 0)
            return false;
        nowHp -= dmg;
        Vector3 dmgPosition = transform.position;
        dmgPosition.y += GetComponent<BoxCollider>().size.y / 2 + 1.5f;
        if (dmg <= 0.000000001f)
            GameUI.Instance.GetMissNumber(dmgPosition);
        else
            GameUI.Instance.GetDamageNumber(DamageTpye.Normal, dmgPosition, (int)dmg);

        Debug.Log($"{name}가 {dmg}의 피해를 받고 {nowHp}가 남았다");
        if (nowHp <= 0)
        {
            OnDie(attacker);
            return true;
        }
        else
        {
            if (dmg > 0)
                animator.SetTrigger("OnDamage");
            return false;
        }
    }
    public bool GetDotDamage(DotDamageInfo dotDamageInfo)
    {
        if (nowHp <= 0)
            return false;
        float dmg = dotDamageInfo.attacker.RealDamage(this) * dotDamageInfo.damagePer;
        nowHp -= dmg;
        Vector3 dmgPosition = transform.position;
        dmgPosition.y += GetComponent<BoxCollider>().size.y / 2 + 1.5f;
        GameUI.Instance.GetDamageNumber(DamageTpye.Dot, dmgPosition, (int)dmg);
        Debug.Log($"{name}가 {dmg}의 피해를 받고 {nowHp}가 남았다");
        if (nowHp <= 0)
        {
            OnDie(dotDamageInfo.attacker);
            return true;
        }
        else
        {
            if (dmg > 0)
                animator.SetTrigger("OnDamage");
            return false;
        }
    }
    public void GetHeal(HealInfo healInfo)
    {
        if (nowHp >= maxHp)
            return;

        float heal = healInfo.healer.maxHp * healInfo.healPer;
        nowHp += heal;
        Vector3 healPosition = transform.position;
        healPosition.y += GetComponent<BoxCollider>().size.y / 2 + 2f;
        GameUI.Instance.GetDamageNumber(DamageTpye.Heal, healPosition, (int)heal);
        if (nowHp >= maxHp)
        {
            nowHp = maxHp;
        }
    }
    public void SetTarget(Pet target)
    {
        if (petStatus == PET_BATTLE_STATUS.DIE)
            return;

        this.target = target;
    }
    public void SetTarget(Pet target, BaseSkill skill)
    {
        if (petStatus == PET_BATTLE_STATUS.DIE)
            return;

        if (skill == null)
            petStatus = PET_BATTLE_STATUS.ATTACK;
        else
            petStatus = PET_BATTLE_STATUS.SKILL;

        this.target = target;
        this.selectSkill = skill;
    }

    public int[] FinalElementStat()
    {

        int[] elementStat = new int[6];
        System.Array.Copy(petInfo.petDefault.elementStat, elementStat, 6);

        if (isReverseElementStat)      // 속성 반전 버프가 걸려 있을 때
        {
            int temp = elementStat[0];
            elementStat[0] = elementStat[2];    // 지 <-> 화
            elementStat[2] = temp;

            temp = elementStat[1];
            elementStat[1] = elementStat[3];    // 수 <-> 풍
            elementStat[3] = temp;

            temp = elementStat[4];
            elementStat[4] = elementStat[5];    // 빛 <-> 암
            elementStat[5] = temp;
        }

        return elementStat;
    }

    public float RealDamage(Pet enemy)
    {
        return RealDamage(enemy, FinalElementStat());
    }

    public float RealDamage(Pet enemy, int[] elementStat)
    {
        float realDamage;
        float normalDmg;
        float criticalDmg;
        float elementDmg = 1;


        if (BattleController.Instance.fieldBuff.IsBuff())   // 공격자 필드버프 적용
        {
            elementStat[BattleController.Instance.fieldBuff.elementNum] *= 2;
        }
        int[] attackerElementStat = elementStat;
        int[] enemyElementStat = enemy.FinalElementStat();

        if (attackerElementStat != null)
        {
            elementDmg = 0;
            // 속성 추가 데미지 계산 [지수화풍 순으로만 일단 계산]
            for (int i = 0; i < 4; i++)
            {
                if (attackerElementStat[i] > 0)                 // 지수화풍 계산
                    if (enemyElementStat[(i + 1) % 4] > 0)
                    {
                        elementDmg += (float)attackerElementStat[i] * (float)enemyElementStat[(i + 1) % 4];
                    }
            }

            elementDmg += (float)attackerElementStat[4] * (float)enemyElementStat[5];   // 빛 > 암 계산
            elementDmg += (float)attackerElementStat[5] * (float)enemyElementStat[4];   // 암 > 빛 계산

            elementDmg = ((elementDmg / 2f) + 100) / 100f;
        }

        // 기본데미지 계산 
        if (enemy.petInfo.RealDef > petInfo.Atk)
        {
            normalDmg = Random.Range(0, 2) * elementDmg;
        }
        else if (enemy.petInfo.RealDef * 8 / 7 > petInfo.Atk)
        {
            normalDmg = Random.Range(1f, petInfo.Atk / 16f) * elementDmg;
        }
        else
        {
            normalDmg = ((petInfo.Atk - enemy.petInfo.RealDef) * 2) + (petInfo.Def / 8) * elementDmg;
        }
        Debug.Log($"{petInfo.Vit},{petInfo.Atk},{petInfo.Def},{petInfo.Dex}");
        Debug.Log($"--------------------------{petInfo.RealDef}");
        // 크리티컬 데미지 계산 
        criticalDmg = normalDmg + (enemy.petInfo.Def * petInfo.petLevel / enemy.petInfo.petLevel * 0.5f);

        // 크리티컬, 회피 확률 계산
        float criticalPer = (Mathf.Pow((petInfo.Dex - enemy.petInfo.Dex) / 0.09f, 0.5f)) / 100f;
        if (criticalPer < 0)
            criticalPer = 0.01f;
        float missPer = (Mathf.Pow((enemy.petInfo.Dex - petInfo.Dex) / 0.02f, 0.5f) * (enemy.petInfo.Dex / petInfo.Dex)) / 100f;
        if (missPer < 0)
            missPer = 0.01f;

        if (missPer >= Random.Range(0f, 1f) && !GameManager.Instance.isMission)
        {
            Debug.Log("회피 발생!");
            return 0;
        }


        if (criticalPer >= Random.Range(0f, 1f) && !GameManager.Instance.isMission)
        {
            Debug.Log("크리티컬 발생!");
            realDamage = criticalDmg;
        }
        else
        {
            realDamage = normalDmg;
        }

        if (realDamage < 1f)
        {
            realDamage = 0;
        }

        return realDamage;
    }

    public IEnumerator OnAttack()
    {
        if (target == null || target.petStatus == PET_BATTLE_STATUS.DIE || isStun)
            yield break;

        isAttack = true;

        switch (petStatus)
        {
            case PET_BATTLE_STATUS.ATTACK:
                yield return StartCoroutine(baseAttack.IEAttack(this, target));
                AttackTurn();
                break;
            case PET_BATTLE_STATUS.SKILL:
                yield return StartCoroutine(selectSkill.UseSkill());
                break;
        }

        isAttack = false;
    }

    public void OnSkill()
    {
        if (selectSkill != null)
            selectSkill.Skill();
        //int idx = skillNum - 1;
        //Debug.Log($"스킬 발동: Skill_{idx}");
        //if (0 <= idx && idx < skills.Count)
        //    if (skills[idx] != null)
        //        skills[idx].Skill();
    }
    public void OnSkills(int attackNum)
    {
        Debug.Log("////////////////////////");
        if (selectSkill != null)
            selectSkill.Skill(attackNum);
    }

    public void KillEnemy(Pet target)
    {
        if (GameManager.Instance.isMission)
            return;
        foreach (Pet alivePet in BattleController.Instance.AlivePetList)
        {
            alivePet.GetExp(target.enemyInfo.exp);
        }
        BackendManager.Instance.GameData.UserData.AddGold(target.enemyInfo.money);
        // 전리품 드랍 함수 추가----------------------------
    }
    public void GetExp(int exp)
    {
        receivedExp += exp;
        if (petInfo.GetExp(exp))
            statusBar.PlayLevelUpAnim();
    }
    public void StartTurn()
    {
        isProvocation= false;
        isStun= false;
        isReverseElementStat= false;
        // 상태이상 적용
        if (statusBuffList.Count > 0)
        {
            foreach(StatusBuffInfo buff in statusBuffList)
            {
                if (buff.isProvocation)
                    isProvocation = true;
                if (buff.isStun)
                    isStun = true;
                if(buff.isReverseElementStat)
                    isReverseElementStat= true;
            }
        }

        // 버프 적용
        petInfo.ResetBuff();
        if (buffList.Count > 0)
        {
            foreach (BuffInfo buff in buffList)
            {
                petInfo.ApplyBuff(buff);
            }
        }


        if(!isStun)
            animator.SetBool("IsStun", false);
    }

    public void AttackTurn()
    {
        skillPoint++;
        if (skillPoint > skillMaxPoint)
            skillPoint = skillMaxPoint;
    }

    public void EndTurn()
    {
        if (petStatus == PET_BATTLE_STATUS.DIE)
            return;

        // 상태이상 턴 수 적용
        if (statusBuffList.Count > 0)
        {
            for (int i = 0; i < statusBuffList.Count; i++)
            {
                if (statusBuffList[i].IsEnd())
                {
                    statusBuffList.RemoveAt(i);
                    i--;
                }
            }
        }


        // 버프 턴 수 적용
        if (buffList.Count > 0)
        {
            for (int i = 0; i<buffList.Count;i++)
            {
                if (buffList[i].IsEnd())
                {
                    buffList.RemoveAt(i);
                    i--;
                }
            }
        }
        // 힐 적용 및 턴수 적용
        if (dotHealList.Count > 0)
        {
            for(int i  = 0;i<dotHealList.Count; i++)
            {
                GetHeal(dotHealList[i]);

                // 턴 수 적용
                if (dotHealList[i].IsEnd())
                {
                    dotHealList.RemoveAt(i);
                    i--;
                }
            }
        }
        // 도트데미지 및 턴 수 적용
        if (dotDamageList.Count > 0)
        {
            for(int i = 0; i < dotDamageList.Count; i++)
            {
                // 도트데미지 적용
                GetDotDamage(dotDamageList[i]);

                // 턴 수 적용
                if (dotDamageList[i].IsEnd())
                {
                    dotDamageList.RemoveAt(i);
                    i--;
                }
            }
        }

        selectSkill = null;
    }
    public void OnDie(Pet attacker)
    {
        petStatus = PET_BATTLE_STATUS.DIE;
        if (!attacker.IsEnemy)
        {
            attacker.KillEnemy(this);
        }
        Reset();
        animator.SetBool("IsDie", true);
    }

    public void Reset()
    {
        skillPoint = 0;
        isProvocation = false;
        isStun = false;
        buffList.Clear();
        petInfo.ResetBuff();
        selectSkill = null;
        target = null;
    }

    public void AddStatusBuff(StatusBuffInfo statusBuff)
    {
        statusBuffList.Add(statusBuff);
    }
    public void AddBuff(BuffInfo buff)
    {
        buffList.Add(buff);
    }
    public void AddDotDamage(DotDamageInfo dotDamage)
    {
        dotDamageList.Add(dotDamage);
    }
    public void AddHeal(HealInfo healInfo)
    {
        dotHealList.Add(healInfo);
    }

    public void ViewPetInfo()
    {
        UIStatus.Instance.SetUp(petInfo);
        Debug.Log($"레벨: {petInfo.petLevel}");
        Debug.Log($"체: {petInfo.ViewVit} / 공: {petInfo.ViewAtk} / 방: {petInfo.ViewDef} / 순: {petInfo.ViewDex}");
        Debug.Log($"체초기: {petInfo.ViewBaseVit} / 공초기: {petInfo.ViewBaseAtk} / 방초기: {petInfo.ViewBaseDef} / 순초기: {petInfo.ViewBaseDex}");
        Debug.Log($"체초기S: {petInfo.petSrankVit} / 공초기S: {petInfo.petSrankAtk} / 방초기S: {petInfo.petSrankDef} / 순초기S: {petInfo.petSrankDex}");
        Debug.Log($"체성장: {petInfo.ViewVitGrow} / 공성장: {petInfo.ViewAtkGrow} / 방성장: {petInfo.ViewDefGrow} / 순성장: {petInfo.ViewDexGrow}");
        Debug.Log($"체성장S: {petInfo.petSrankVitCoe} / 공성장S: {petInfo.petSrankAtkCoe} / 방성장S: {petInfo.petSrankDefCoe} / 순성장S: {petInfo.petSrankDexCoe}");
    }
    private void OnDestroy()
    {
        if (statusBar != null)
            Destroy(statusBar.gameObject);
    }
}

