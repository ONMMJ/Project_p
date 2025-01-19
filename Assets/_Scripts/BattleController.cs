using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ATTACKER_TYPE
{
    PLAYER,
    ENEMY
}
public enum ATTACK_TYPE
{
    ATTACK,
    SKILL,
    BUFF,
    DEBUFF,
    ALL,
    SELF,
}
public enum ELEMENT_TYPE
{
    EARTH = 0,
    WATER,
    FIRE,
    WIND,
    LIGHT,
    DARK
}
public enum TARGET_GROUP
{
    TEAM_ALL,
    TEAM_OTHER,
    ENEMY_ALL,
    ENEMY_OTHER,
    FIELD_ALL,
    FIELD_OTHER,
}

public struct EnemyGroup
{
    public PetInfo enemy;
    public EnemyInfo enemyInfo;

    public EnemyGroup(PetInfo enemy, EnemyInfo enemyInfo)
    {
        this.enemy = enemy;
        this.enemyInfo = enemyInfo;
    }
}

public class FieldBuff
{
    public int elementNum { get; private set; }     // 0부터 지 수 화 풍 빛 암
    public int count { get; private set; }

    public FieldBuff()
    {
        elementNum = -1;        // -1: 필드 버프 없음
    }

    public void Setup(ELEMENT_TYPE elementType, int count)
    {
        elementNum = (int)elementType;

        this.count = count;
    }

    public bool IsBuff()
    {
        return elementNum >= 0 && count > 0;
    }

    public void EndTurn()
    {
        if (count <= 0)
            return;

        count--;
        if (count <= 0)
        {
            Reset();
        }
    }

    public void Reset()
    {
        elementNum = -1;
        count = 0;
    }
}

public class BattleController : SingletonReset<BattleController>
{
    string stageId;
    bool isBattle;
    bool isClear;
    [HideInInspector] public bool isAuto;
    [HideInInspector] public bool isJointAttack;
    [HideInInspector] public FieldBuff fieldBuff;

    int attackNum = 0;

    List<Pet> pets;
    List<Pet> enemies;
    List<EnemyGroup> stageEnemies;
    List<EnemyGroup> groundEnemies;


    [Header("Position")]
    [SerializeField] List<Transform> petSpawns;
    [SerializeField] List<Transform> enemySpawns;
    [SerializeField] Transform centerPos;

    List<Pet> dexOrderPets;

    [HideInInspector] public BaseSkill skill;
    public Pet target;
    public IEnumerator ieStartPhase { get; private set; }
    public ATTACKER_TYPE attackerType { get; private set; }

    public List<Pet> AlivePetList => pets.Where(x => x.petStatus != Pet.PET_BATTLE_STATUS.DIE).ToList();
    public List<Pet> AliveEnemyList => enemies.Where(x => x.petStatus != Pet.PET_BATTLE_STATUS.DIE).ToList();

    public List<Pet> Pets => pets;
    public List<Pet> attackerList { get; private set; }
    public Transform CenterPos => centerPos;

    public ATTACK_TYPE attackType
    {
        get
        {
            if (skill == null)
                return ATTACK_TYPE.ATTACK;
            else
                return skill.AttackType;
        }
    }
    public List<Pet> targetList
    {
        get
        {
            switch (attackType)
            {
                case ATTACK_TYPE.ATTACK:
                case ATTACK_TYPE.SKILL:
                    if (attackerType == ATTACKER_TYPE.PLAYER)
                    {
                        if (AliveEnemyList.Where(x => x.isProvocation).ToList().Count > 0)
                            return AliveEnemyList.Where(x => x.isProvocation).ToList();
                        else
                            return AliveEnemyList.Where(x => !x.isProvocation).ToList();
                    }
                    else if (attackerType == ATTACKER_TYPE.ENEMY)
                    {
                        if (AlivePetList.Where(x => x.isProvocation).ToList().Count > 0)
                            return AlivePetList.Where(x => x.isProvocation).ToList();
                        else
                            return AlivePetList.Where(x => !x.isProvocation).ToList();
                    }
                    break;
                case ATTACK_TYPE.DEBUFF:
                    if (attackerType == ATTACKER_TYPE.PLAYER)
                        return AliveEnemyList;
                    else if(attackerType == ATTACKER_TYPE.ENEMY)
                        return AlivePetList;
                    break;
                case ATTACK_TYPE.BUFF:
                    if (attackerType == ATTACKER_TYPE.PLAYER)
                        return AlivePetList;
                    else if (attackerType == ATTACKER_TYPE.ENEMY)
                        return AliveEnemyList;
                    break;
                case ATTACK_TYPE.ALL:
                    List<Pet> allPets = new List<Pet>();
                    allPets.AddRange(AlivePetList);
                    allPets.AddRange(AliveEnemyList);
                    return allPets;
                case ATTACK_TYPE.SELF:
                    return null;
            }
            return null;
        }
    }
    public int TargetCount => petSpawns.Count + enemySpawns.Count;
    // 나중에 Start로 변경
    public void Start()
    {
        stageId = GameManager.Instance.stageId;
        isAuto = false;
        isJointAttack = false;
        fieldBuff = new();
        pets = new();
        enemies = new();
        stageEnemies = new();
        groundEnemies = new();
        target = null;
        skill = null;



        if (!GameManager.Instance.isMission)     // 미션이 아닐때
        {
            isClear = BackendManager.Instance.GameData.StageData.IsClearStage(stageId);

            GetEnemyGroup();
            // 대표 캐릭터 소환
            List<PetInfo> petInfos = new List<PetInfo>();
            foreach (string petId in BackendManager.Instance.GameData.UserData.selectPets)
            {
                petInfos.Add(new PetInfo(BackendManager.Instance.GameData.UserPetData.Dictionary[petId]));
            }
            for (int i = 0; i < petInfos.Count; i++)
            {
                Pet pet = PetManager.Instance.InfoToPet(petInfos[i], null);
                pet.SetPosition(petSpawns[i]);
                pet.SetBattle();
                pets.Add(pet);
            }

            if (!isClear)       // 클리어 하지 않았다면 라운드 몬스터 소환 
            {
                for (int i = 0; i < stageEnemies.Count; i++)
                {
                    Pet enemy = PetManager.Instance.InfoToPet(stageEnemies[i].enemy, stageEnemies[i].enemyInfo);
                    enemy.SetPosition(enemySpawns[i]);
                    enemy.SetBattle();
                    enemies.Add(enemy);
                }
            }
            else                // 클리어 했다면 사냥터 몬스터 소환
            {
                for (int i = 0; i < enemySpawns.Count; i++)
                {
                    Pet enemy = PickEnemy();
                    enemy.SetPosition(enemySpawns[i]);
                    enemy.SetBattle();
                    enemies.Add(enemy);
                }
            }
        }   
        else            // 미션일 때
        {
            isClear = false;

            // 아군
            List<PetInfo> petInfos = new List<PetInfo>();
            foreach (MissionPet missionPet in GameManager.Instance.missionData.MissionPetList)
            {
                petInfos.Add(PetManager.Instance.GetMissionPet(missionPet.petName, missionPet.petLevel, missionPet.startSkillPoint, false));
            }
            for (int i = 0; i < petInfos.Count; i++)
            {
                Pet pet = PetManager.Instance.InfoToPet(petInfos[i], null);
                pet.SetPosition(petSpawns[i]);
                pet.SetBattle();
                pets.Add(pet);
            }

            // 적
            List<PetInfo> enemyInfos = new List<PetInfo>();
            foreach (MissionPet missionPet in GameManager.Instance.missionData.MissionEnemyList)
            {
                enemyInfos.Add(PetManager.Instance.GetMissionPet(missionPet.petName, missionPet.petLevel, missionPet.startSkillPoint, true));
            }
            for (int i = 0; i < enemyInfos.Count; i++)
            {
                Pet enemy = PetManager.Instance.InfoToPet(enemyInfos[i], null);
                enemy.SetPosition(enemySpawns[i]);
                enemy.SetBattle();
                enemies.Add(enemy);
            }
        }

        isBattle = true;
        StartStage();
    }

    public void GetEnemyGroup()
    {
        // Stage
        List<EnemyInfo> sEnemeyInfoList = BackendManager.Instance.Chart.EnemyChart.StageEnemyDictionary[stageId];
        foreach (EnemyInfo enemyInfo in sEnemeyInfoList)
        {
            PetInfo petInfo = PetManager.Instance.GetEnemy(enemyInfo.petName, enemyInfo.petLevel);
            stageEnemies.Add(new EnemyGroup(petInfo, enemyInfo));
        }


        // Ground
        List<EnemyInfo> gEnemeyInfoList = BackendManager.Instance.Chart.EnemyChart.GroundEnemyDictionary[stageId];
        foreach(EnemyInfo enemyInfo in gEnemeyInfoList)
        {
            PetInfo petInfo = PetManager.Instance.GetEnemy(enemyInfo.petName, enemyInfo.petLevel);
            groundEnemies.Add(new EnemyGroup(petInfo, enemyInfo));
        }
    }

    public Pet PickEnemy()
    {
        float per = 0;
        float rand = Random.Range(0f, 1f);
        foreach(EnemyGroup enemy in groundEnemies)
        {
            per += enemy.enemyInfo.spawnPercent/100f;
            if(rand <= per)
            {
                return PetManager.Instance.InfoToPet(enemy.enemy, enemy.enemyInfo);
            }
        }
        return null;
    }

    public void SetBattle()
    {
        dexOrderPets = new List<Pet>();
        dexOrderPets.AddRange(pets);
        dexOrderPets.AddRange(enemies);
        dexOrderPets.Sort((x, y) =>
        {
            if (x.dex > y.dex)
                return -1;
            // 순발력이 같으면 랜덤으로 선공
            else if (x.dex == y.dex)
            {
                if (Random.Range(0, 2) == 0)
                    return -1;
                else
                    return 1;
            }
            else
                return 1;
        });
        StartPhase();
    }
    private void OrderByPets()
    {
        dexOrderPets = new List<Pet>();
        dexOrderPets.AddRange(AlivePetList);
        dexOrderPets.AddRange(AliveEnemyList);
        dexOrderPets.Sort((x, y) =>
        {
            if (x.dex > y.dex)
                return -1;
            // 순발력이 같으면 랜덤으로 선공
            else if (x.dex == y.dex)
            {
                if (Random.Range(0, 2) == 0)
                    return -1;
                else
                    return 1;
            }
            else
                return 1;
        });
    }

    public void StartPhase()
    {
        if (isBattle)
        {
            ieStartPhase = IEStartPhase();
            StartCoroutine(ieStartPhase);
        }
    }
    public void StartStage()
    {
        isBattle = true;
        StartCoroutine(IEStartStage());
    }

    public void StopStage()
    {
        isBattle = false;
    }

    IEnumerator IEStartStage()
    {
        while (isBattle)
        {
            ieStartPhase = IEStartPhase();
            yield return StartCoroutine(ieStartPhase);
        }
    }

    IEnumerator IEStartPhase()
    {
        OrderByPets();
        yield return StartCoroutine(IEStartTurn());

        yield return StartCoroutine(IEGetTarget());
        yield return StartCoroutine(IEStartBattle());

        yield return StartCoroutine(IEEndTurn());
    }
    IEnumerator IEGetTarget()
    {
        Pet prevPet = null;
        for (int i = 0; i < dexOrderPets.Count; i++)
        {
            target = null;
            skill = null;

            // 공격자의 타입
            if (dexOrderPets[i].IsEnemy)
                attackerType = ATTACKER_TYPE.ENEMY;
            else
                attackerType = ATTACKER_TYPE.PLAYER;
            // 공격자 스킬 세팅
            BattleCanvas.Instance.UpdateSkill(dexOrderPets[i].Skills);

            if (dexOrderPets[i].isStun)
            {
                dexOrderPets[i].target = null;
            }
            else if (isAuto || attackerType == ATTACKER_TYPE.ENEMY)
            {
                SearchRandomTarget(dexOrderPets[i], ref prevPet, false);
            }
            else
            {
                BattleCanvas.Instance.SetTargetArrow(dexOrderPets[i].transform.position + Vector3.up * (dexOrderPets[i].GetComponent<BoxCollider>().size.y / 2 + 2f));
                
                while (target == null && !isAuto)
                {
                    SelectTargetUI(i);
                    //if (Input.GetMouseButtonDown(0))
                    //{
                    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    //    RaycastHit hit;
                    //    if (Physics.Raycast(ray, out hit))
                    //    {
                    //        Debug.Log(hit.transform.name);
                    //        Pet hitTarget = hit.transform.gameObject.GetComponent<Pet>();
                    //        if (hitTarget != null)
                    //        {
                    //            if (targetList == null)
                    //            {
                    //                if (hitTarget == dexOrderPets[i])
                    //                {
                    //                    target = hitTarget;
                    //                }
                    //            }
                    //            else 
                    //            {
                    //                if (targetList.Contains(hitTarget))
                    //                    target = hitTarget;
                    //            }
                    //            prevPet = target;
                    //            dexOrderPets[i].SetTarget(target, skill);
                    //        }
                    //    }
                    //}
                    yield return null;
                }
                prevPet = target;
                dexOrderPets[i].SetTarget(target, skill);

                if (isAuto)
                {
                    i--;
                    continue;
                }
            }
            BattleCanvas.Instance.HideTargetArrow();
            BattleCanvas.Instance.HideAttackTargetImage();
        }
        yield return null;
    }
    private void SearchRandomTarget(Pet attacker, ref Pet prevPet, bool isStartBattle)
    {
        // 공격자의 타입
        if (attacker.IsEnemy)
            attackerType = ATTACKER_TYPE.ENEMY;
        else
            attackerType = ATTACKER_TYPE.PLAYER;

        System.Random rand = new System.Random();
        if (isStartBattle)
        {
            skill = attacker.selectSkill;
        }
        else
        {
            if (attacker.selectSkill == null)
                skill = attacker.Skills.Where(x => x.IsReady).OrderBy(x => rand.Next()).FirstOrDefault();
        }
        if (targetList == null)
            target = attacker;
        else
        {
            target = FindPetTarget(targetList);
            // 오토 플레이 시 아군이고 합동공격이 활성화 되어 있다면 자동으로 합동공격
            if (attackerType == ATTACKER_TYPE.PLAYER && isJointAttack)
            {
                if (prevPet == null)
                {
                    prevPet = target;
                }
                else
                {
                    target = prevPet;
                }
            }
        }
        if (!isStartBattle)
            attacker.SetTarget(target, skill);
        else
            attacker.SetTarget(target);
    }
    private void SelectTargetUI(int self)
    {
        List<Pet> pets = new List<Pet>();
        switch (attackType)
        {
            case ATTACK_TYPE.ATTACK:
            case ATTACK_TYPE.SKILL:
                if (AliveEnemyList.Where(x => x.isProvocation).ToList().Count > 0)
                {
                    foreach (Pet enemy in AliveEnemyList.Where(x => x.isProvocation).ToList())
                    {
                        pets.Add(enemy);
                    }
                }
                else
                {
                    foreach (Pet enemy in AliveEnemyList)
                    {
                        pets.Add(enemy);
                    }
                }
                break;
            case ATTACK_TYPE.DEBUFF:
                foreach (Pet enemy in AliveEnemyList)
                {
                    pets.Add(enemy);
                }
                break;
            case ATTACK_TYPE.BUFF:
                foreach (Pet pet in AlivePetList)
                {
                    pets.Add(pet);
                }
                break;
            case ATTACK_TYPE.SELF:
                pets.Add(dexOrderPets[self]);
                break;
            case ATTACK_TYPE.ALL:
                foreach (Pet pet in AlivePetList)
                {
                    pets.Add(pet);
                }
                foreach (Pet enemy in AliveEnemyList)
                {
                    pets.Add(enemy);
                }
                break;
        }
        BattleCanvas.Instance.SetAttackTargetImage(pets);
    }
    IEnumerator IEStartBattle()
    {
        while (attackNum < dexOrderPets.Count)
        {
            // 공격하려는 펫이 죽었거나 스턴상이면 (다음으로)
            if (dexOrderPets[attackNum].petStatus == Pet.PET_BATTLE_STATUS.DIE || dexOrderPets[attackNum].isStun)
            {
                attackNum++;
                continue;
            }

            // 합동 공격
            Queue<Pet> attacker = new Queue<Pet>();
            attacker.Enqueue(dexOrderPets[attackNum]);
            for (int i = attackNum + 1; i < dexOrderPets.Count; i++)
            {
                // AI거나 합동공격을 체크하지 않았으면 합동공격을 하지 않음
                if (dexOrderPets[attackNum].IsEnemy || !isJointAttack)
                    break;

                // 다음 순서의 펫이 죽어있거나 스턴 상태이면 다음으로
                if (dexOrderPets[i].petStatus == Pet.PET_BATTLE_STATUS.DIE || dexOrderPets[i].isStun)
                {
                    attackNum++;
                    continue;
                }

                if (dexOrderPets[attackNum].IsEnemy == dexOrderPets[i].IsEnemy && dexOrderPets[attackNum].target == dexOrderPets[i].target)
                {
                    // 다음 공격 펫이 아군이 타겟이 같다면 합동 공격에 추가 및 공격자 번호 증가
                    attacker.Enqueue(dexOrderPets[i]);
                    attackNum++;
                    // 체력을 초과해서 공격할 시 더 이상 합동 공격을 추가하지 않도록 하려고 했으나 회피, 크리티컬 변수가 많아 제거
                    //if (targetEnemy.nowHp <= attaker.Sum(x => x.atk))
                    //  break;
                }
                else
                    break;
            }

            while (attacker.Count > 0)
            {
                Pet prevTarget = null;
                attackerList = new List<Pet>();
                if (attacker.Peek().selectSkill != null)
                {
                    Pet pet = attacker.Dequeue();
                    Debug.Log(pet.petInfo.petDefault.petName);
                    if (pet.target.petStatus == Pet.PET_BATTLE_STATUS.DIE)
                    {
                        SearchRandomTarget(pet, ref prevTarget, true);
                    }
                    attackerList.Add(pet);
                }
                else
                {
                    int Count = attacker.Count;
                    for (int i = 0; i < Count; i++)
                    {
                        Pet pet = attacker.Dequeue();
                        Debug.Log(pet.petInfo.petDefault.petName);
                        if (pet.selectSkill != null)
                        {
                            attacker.Enqueue(pet);
                        }
                        else
                        {
                            if (pet.target.petStatus == Pet.PET_BATTLE_STATUS.DIE)
                            {
                                SearchRandomTarget(pet, ref prevTarget, true);
                            }
                            attackerList.Add(pet);
                        }
                    }
                }
                foreach(Pet pet in attackerList)
                {
                    // 도발중인 펫이 있는지 확인
                    List<Pet> provocationTargets;
                    if (pet.IsEnemy)
                        provocationTargets = AlivePetList.Where(x => x.isProvocation).ToList();
                    else
                        provocationTargets = AliveEnemyList.Where(x => x.isProvocation).ToList();
                    // 도발중인 펫이 있다면 도발 캐릭터 중에서 랜덤 선택 후 타겟을 도발 캐릭터로 변경
                    if (provocationTargets.Count > 0)
                        pet.target = RandomPet(provocationTargets);

                    StartCoroutine(pet.OnAttack());
                }
                while (attackerList.Where(x => x.isAttack).Count() > 0)
                {
                    yield return null;
                }
            }
            attackNum++;
        }
    }

    private IEnumerator IEStartTurn()
    {
        attackNum = 0;
        // 전체 펫 턴 시작 함수
        foreach (Pet pet in dexOrderPets)
        {
            pet.StartTurn();
            yield return null;
        }
    }

    private IEnumerator IEEndTurn()
    {
        // 전체 펫 턴 종료 함수
        foreach (Pet pet in dexOrderPets)
        {
            pet.EndTurn();
            yield return null;
        }

        fieldBuff.EndTurn();

        if (!isClear)
        {
            if (enemies.Where(x => x.petStatus != Pet.PET_BATTLE_STATUS.DIE).Count() <= 0)
            {
                Debug.Log("Win!");
                EndBattle();
                if (!GameManager.Instance.isMission)
                {
                    BackendManager.Instance.GameData.StageData.ClearStage(stageId);
                    UIManager.Instance.EndBattle("GAME CLEAR");
                }
                else
                {
                    UIManager.Instance.EndBattle("MISSION CLEAR");
                }
            }
        }
        else
        {
            var dieEnemies = enemies.Where(x => x.petStatus == Pet.PET_BATTLE_STATUS.DIE).ToList();
            if (dieEnemies.Count > 0)
            {
                foreach (var dieEnemy in dieEnemies)
                {
                    Transform pos = dieEnemy.startPos;
                    enemies.Remove(dieEnemy);
                    Destroy(dieEnemy.gameObject);
                    Pet newEnemy = PickEnemy();
                    newEnemy.SetPosition(pos);
                    newEnemy.SetBattle();
                    enemies.Add(newEnemy);
                    yield return null;
                }
            }
        }
        if (pets.Where(x => x.petStatus != Pet.PET_BATTLE_STATUS.DIE).Count() <= 0)
        {
            Debug.Log("Lose!");

            EndBattle();
            UIManager.Instance.EndBattle("GAME OVER");
        }
        yield return null;
    }

    private bool AutoPlay()
    {

        bool isEnemy = dexOrderPets[attackNum].IsEnemy;
        // 적이 공격할 때(합동 공격 x)
        if (isEnemy)
        {
            if (AutoEnemy())
                return true;
        }

        // 아군이 공격할 때(합동 공격 o)
        else
        {
            if (AutoPlayer())
                return true;
        }
        return false;
    }
    private bool AutoEnemy()
    {
        return false;
    }
    private bool AutoPlayer()
    {
        // 합동 공격
        List<Pet> attaker = new List<Pet> { dexOrderPets[attackNum] };
        for (int i = attackNum + 1; i < dexOrderPets.Count; i++)
        {
            // 다음 순서의 펫이 죽어있으면 다음으로
            if (dexOrderPets[i].petStatus == Pet.PET_BATTLE_STATUS.DIE)
            {
                attackNum++;
                continue;
            }

            if (dexOrderPets[attackNum].IsEnemy == dexOrderPets[i].IsEnemy)
            {
                // 다음 공격 펫이 아군이라면 합동 공격에 추가 및 공격자 번호 증가
                attaker.Add(dexOrderPets[i]);
                attackNum++;
                // 체력을 초과해서 공격할 시 더 이상 합동 공격을 추가하지 않도록 하려고 했으나 회피, 크리티컬 변수가 많아 제거
                //if (targetEnemy.nowHp <= attaker.Sum(x => x.atk))
                //  break;
            }
            else
                break;
        }

        foreach (Pet pet in attaker)
        {
            pet.OnAttack();
        }

        return false;
    }


    public List<Pet> GetTarget(Pet pet, TARGET_GROUP targetGroup)
    {
        List<Pet> petGroup = new();
        List<Pet> teamGroup = pet.IsEnemy ? AliveEnemyList : AlivePetList;
        List<Pet> enemyGroup = pet.IsEnemy ? AlivePetList : AliveEnemyList;
        switch (targetGroup)
        {
            case TARGET_GROUP.FIELD_ALL:
                petGroup.AddRange(teamGroup);
                petGroup.AddRange(enemyGroup);
                break;
            case TARGET_GROUP.FIELD_OTHER:
                petGroup.AddRange(teamGroup);
                petGroup.AddRange(enemyGroup);
                petGroup.Remove(pet);
                break;
            case TARGET_GROUP.TEAM_ALL:
                petGroup.AddRange(teamGroup);
                break;
            case TARGET_GROUP.TEAM_OTHER:
                petGroup.AddRange(teamGroup);
                petGroup.Remove(pet);
                break;
            case TARGET_GROUP.ENEMY_ALL:
                petGroup.AddRange(enemyGroup);
                break;
            case TARGET_GROUP.ENEMY_OTHER:
                petGroup.AddRange(enemyGroup);
                petGroup.Remove(pet.target);
                break;
        }

        return petGroup;
    }
    private Pet FindPetTarget(List<Pet> pets)
    {
        // 대기중인 타겟 찾기(죽은 타겟을 공격하지 않음)
        List<Pet> targets = pets.Where(x => x.petStatus != Pet.PET_BATTLE_STATUS.DIE).ToList();

        if (targets.Count <= 0)
            return null;

        // 도발중인 펫이 있는지 확인
        List<Pet> provocationTargets = targets.Where(x => x.isProvocation).ToList();
        Pet randomTarget;
        // 도발중인 펫이 있다면 도발 캐릭터 중에서 랜덤 공격
        if (provocationTargets.Count > 0)
            randomTarget = RandomPet(provocationTargets);
        // 없다면 죽지 않은 타겟 중에 랜덤 공격
        else
            randomTarget = RandomPet(targets);

        return randomTarget;
    }
    private Pet RandomPet(List<Pet> pets)
    {
        return pets[Random.Range(0, pets.Count)];
    }
    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        StopCoroutines();
    }
#endif

    public void EndBattle()
    {
        isBattle = false;
    }
}