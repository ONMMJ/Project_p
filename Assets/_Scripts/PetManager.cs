using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum STAT_COE_RANK
{
    SP = 0,
    S,
    A,
    B,
    C,
    D,
    F
}

public enum STAT_TYPE
{
    VIT,
    ATK,
    DEF,
    DEX
}

public class PetManager : Singleton<PetManager>
{
    [SerializeField] public int maxResetCount; // 차트로 변경

    // 환생 시 확률
    #region RevivePer
    float dRankUp = 0.3f;           // 상승 확률 
    float dRankSpecial = 0.2f;      // 대성공 확률

    float cRankUp = 0.3f;           // 상승 확률 
    float cRankDown = 0.2f;         // 하락 확률
    float cRankSpecial = 0.1f;      // 대성공 확률

    float bRankUp = 0.3f;
    float bRankDown = 0.2f;
    float bRankSpecial = 0.05f;

    float aRankUp = 0.2f;
    float aRankDown = 0.1f;

    float sRankDown = 0f;
    #endregion

    // 초기화 시 확률
    #region ResetPer
    //float[] per = new float[5] { 0.1f, 0.15f, 0.20f, 0.25f, 0.30f }; // S, A, B, C, D 랭크 확률
    float[] per = new float[5] { 0.2f, 0.2f, 0.20f, 0.2f, 0.20f }; // S, A, B, C, D 랭크 확률
    float sPlusPer = 0.1f;
    #endregion

    private void Start()
    {
    }

    public PetDefault GetWildPet(string name, int level)
    {
        return null;
    }
    public PetInfo GetMissionPet(string petName, int level, int startSkillPoint, bool isEnemy)
    {
        PetInfo petInfo = new PetInfo();
        PetDefault petDefault = BackendManager.Instance.Chart.PetChart.Dictionary[petName];

        petInfo.addVit = STAT_COE_RANK.D;
        petInfo.addAtk = STAT_COE_RANK.D;
        petInfo.addDef = STAT_COE_RANK.D;
        petInfo.addDex = STAT_COE_RANK.D;

        petInfo.petBaseVit = petDefault.petBaseCoe * (petDefault.petVitCoe + 2.5f) / 100f;
        petInfo.petBaseAtk = petDefault.petBaseCoe * (petDefault.petAtkCoe + 2.5f) / 100f;
        petInfo.petBaseDef = petDefault.petBaseCoe * (petDefault.petDefCoe + 2.5f) / 100f;
        petInfo.petBaseDex = petDefault.petBaseCoe * (petDefault.petDexCoe + 2.5f) / 100f;

        petInfo.petVit = petInfo.petBaseVit;
        petInfo.petAtk = petInfo.petBaseAtk;
        petInfo.petDef = petInfo.petBaseDef;
        petInfo.petDex = petInfo.petBaseDex;

        petInfo.petDefault = petDefault;
        petInfo.petRank = GetRank(petDefault.petVitCoe + petDefault.petAtkCoe + petDefault.petDefCoe + petDefault.petDexCoe);
        petInfo.isEnemy = isEnemy;

        int[] skillsLevel = { 1, 1, 1 };
        petInfo.skillsLevel = skillsLevel;
        petInfo.startSkillPoint = startSkillPoint;

        petInfo.SetUp();
        petInfo.SetEnemyLevel(level);

        return petInfo;
    }
    public PetInfo GetEnemy(string petName, int level)
    {
        PetInfo petInfo = new PetInfo();
        PetDefault petDefault = BackendManager.Instance.Chart.PetChart.Dictionary[petName];

        petInfo.addVit = STAT_COE_RANK.D;
        petInfo.addAtk = STAT_COE_RANK.D;
        petInfo.addDef = STAT_COE_RANK.D;
        petInfo.addDex = STAT_COE_RANK.D;

        petInfo.petBaseVit = petDefault.petBaseCoe * (petDefault.petVitCoe + 2.5f) / 100f;
        petInfo.petBaseAtk = petDefault.petBaseCoe * (petDefault.petAtkCoe + 2.5f) / 100f;
        petInfo.petBaseDef = petDefault.petBaseCoe * (petDefault.petDefCoe + 2.5f) / 100f;
        petInfo.petBaseDex = petDefault.petBaseCoe * (petDefault.petDexCoe + 2.5f) / 100f;

        petInfo.petVit = petInfo.petBaseVit;
        petInfo.petAtk = petInfo.petBaseAtk;
        petInfo.petDef = petInfo.petBaseDef;
        petInfo.petDex = petInfo.petBaseDex;

        petInfo.petDefault = petDefault;
        petInfo.petRank = GetRank(petDefault.petVitCoe + petDefault.petAtkCoe + petDefault.petDefCoe + petDefault.petDexCoe);
        petInfo.isEnemy = true;

        int[] skillsLevel = { 1, 1, 1 };
        petInfo.skillsLevel = skillsLevel;

        petInfo.SetUp();
        petInfo.SetEnemyLevel(level);

        return petInfo;
    }
    public PetInfo GetLv1Pet(string petName)
    {
        PetInfo petInfo = new PetInfo();
        PetDefault petDefault = BackendManager.Instance.Chart.PetChart.Dictionary[petName];

        List<STAT_COE_RANK> ranks = RandomCoeRank();
        petInfo.addVit = ranks[0];
        petInfo.addAtk = ranks[1];
        petInfo.addDef = ranks[2];
        petInfo.addDex = ranks[3];

        float levelUpVitCoe = RankToStat(petInfo.addVit);
        float levelUpAtkCoe = RankToStat(petInfo.addAtk);
        float levelUpDefCoe = RankToStat(petInfo.addDef);
        float levelUpDexCoe = RankToStat(petInfo.addDex);

        for (int i = 0; i < 10; i++)
        {
            int work = Random.Range(0, 4);
            if (work == 0)
            {
                levelUpVitCoe++;
            }
            if (work == 1)
            {
                levelUpAtkCoe++;
            }
            if (work == 2)
            {
                levelUpDefCoe++;
            }
            if (work == 3)
            {
                levelUpDexCoe++;
            }
        }
        petInfo.petBaseVit = petDefault.petBaseCoe * (petDefault.petVitCoe + levelUpVitCoe) / 100f;
        petInfo.petBaseAtk = petDefault.petBaseCoe * (petDefault.petAtkCoe + levelUpAtkCoe) / 100f;
        petInfo.petBaseDef = petDefault.petBaseCoe * (petDefault.petDefCoe + levelUpDefCoe) / 100f;
        petInfo.petBaseDex = petDefault.petBaseCoe * (petDefault.petDexCoe + levelUpDexCoe) / 100f;

        petInfo.petVit = petInfo.petBaseVit;
        petInfo.petAtk = petInfo.petBaseAtk;
        petInfo.petDef = petInfo.petBaseDef;
        petInfo.petDex = petInfo.petBaseDex;

        petInfo.petDefault = petDefault;
        petInfo.petRank = GetRank(petDefault.petVitCoe + petDefault.petAtkCoe + petDefault.petDefCoe + petDefault.petDexCoe);
        petInfo.isEnemy = false;

        petInfo.petLevel = 1;
        petInfo.petNextExp = petInfo.GetNextExp(petInfo.petLevel);

        int[] skillsLevel = { 1, 1, 1 };
        petInfo.skillsLevel = skillsLevel;

        petInfo.SetUp();

        return petInfo;
    }

    public List<STAT_COE_RANK> RandomCoeRank()
    {
        List<STAT_COE_RANK> ranks = new List<STAT_COE_RANK>();

        int count = 4;      // VIT, ATK, DEF, DEX
        int spCount = 0;    // S+ 개수

        for (int i = 0; i < count; i++)
        {
            //STAT_COE_RANK rank = STAT_COE_RANK.S;
            //float randomNum = Random.Range(0f, 1f);
            //float cumulative = 0f;

            //// 확률에 따라 랭크 결정
            //for (int j = 0; j < per.Length; j++)
            //{
            //    cumulative += per[j];
            //    if (randomNum <= cumulative)
            //        break;
            //    rank++;
            //}

            // S랭크일 때 확률로 S+랭크로 변경(최대 2개) - 잠시 막아둠
            //if (spCount < 2)
            //    if (rank == STAT_COE_RANK.S)
            //        if (Random.Range(0f, 1f) <= sPlusPer)
            //        {
            //            rank = STAT_COE_RANK.SP;
            //            spCount++;
            //        }

            STAT_COE_RANK rank = STAT_COE_RANK.S + Random.Range(0, 5);
            ranks.Add(rank);
        }

        // S+이 위에서부터 뜰 확률이 높은 현상을 방지하기 위해 셔플
        System.Random rnd = new System.Random();
        List<STAT_COE_RANK> result = ranks.OrderBy(item => rnd.Next()).ToList();

        return result;
    }

    public STAT_COE_RANK ReviveAddStat(STAT_COE_RANK addStat)
    {
        STAT_COE_RANK reviveAddStat = addStat;

        float randomNum = Random.Range(0f, 1f);
        float randomSpecial = Random.Range(0f, 1f);

        switch (addStat)
        {
            case STAT_COE_RANK.D:
                if (randomNum <= dRankUp)
                {
                    if (randomSpecial <= dRankSpecial)
                        reviveAddStat -= 2;
                    else
                        reviveAddStat--;
                }
                break;
            case STAT_COE_RANK.C:
                if (randomNum <= cRankUp)
                {
                    if (randomSpecial <= cRankSpecial)
                        reviveAddStat -= 2;
                    else
                        reviveAddStat--;
                }
                else if (randomNum <= cRankUp + cRankDown)
                {
                    reviveAddStat++;
                }
                break;
            case STAT_COE_RANK.B:
                if (randomNum <= bRankUp)
                {
                    if (randomSpecial <= bRankSpecial)
                        reviveAddStat -= 2;
                    else
                        reviveAddStat--;
                }
                else if (randomNum <= bRankUp + bRankDown)
                {
                    reviveAddStat++;
                }
                break;
            case STAT_COE_RANK.A:
                if (randomNum <= aRankUp)
                {
                    reviveAddStat--;
                }
                else if (randomNum <= aRankUp + aRankDown)
                {
                    reviveAddStat++;
                }
                break;
            case STAT_COE_RANK.S:
                if (randomNum <= sRankDown)
                {
                    reviveAddStat--;
                }
                break;
        }

        return reviveAddStat;
    }

    public float RankToStat(STAT_COE_RANK addStat)
    {
        float stat = 0f;
        switch (addStat)
        {
            case STAT_COE_RANK.SP:
                stat = 2.5f;
                break;
            case STAT_COE_RANK.S:
                stat = 2f;
                break;
            case STAT_COE_RANK.A:
                stat = 1f;
                break;
            case STAT_COE_RANK.B:
                stat = 0f;
                break;
            case STAT_COE_RANK.C:
                stat = -1f;
                break;
            case STAT_COE_RANK.D:
                stat = -2f;
                break;
        }

        return stat;
    }
    public System.Type GetTypeFromName(string typeName)
    {
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            var assembly = assemblies[i];
            try
            {
                var types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    var type = types[j];
                    if (string.Equals(type.Name, typeName)) return type;
                }
            }
            catch (System.Exception)
            {
                // Ignore exceptions.
            }
        }
        return null;
    }

    public Pet InfoToPet(PetInfo petInfo, EnemyInfo enemyInfo)
    {
        GameObject petPrefab = Instantiate(Resources.Load<GameObject>($"PetPrefabs/{petInfo.petDefault.petAssetName}"));
        petPrefab.AddComponent<Pet>();
        Pet pet = petPrefab.GetComponent<Pet>();
        for (int i = 0; i < petInfo.petDefault.skillsName.Length; i++)
        {
            if (!string.IsNullOrEmpty(petInfo.petDefault.skillsName[i]) && petInfo.petDefault.skillsRange[i] != null)
            {
                Debug.Log(petInfo.petDefault.skillsName[i]);
                BaseSkill skill = petPrefab.AddComponent(GetTypeFromName(petInfo.petDefault.skillsName[i])) as BaseSkill;
                skill.SetPet(pet);
                skill.Setup(petInfo.petDefault.skillsName[i], petInfo.skillsLevel[i], i + 1, petInfo.petDefault.skillsRange[i]);
            }
        }
        petInfo.SetGame();

        pet.Setup(petInfo, enemyInfo);
        return pet;
    }

    public float GetSValue(STAT_TYPE type, float petBaseCoe, float petVitCoe, float petAtkCoe, float petDefCoe, float petDexCoe)
    {
        float returnValue = 0.0f;

        float vitValue = (petVitCoe + 2.0f + 2.5f) * petBaseCoe / 100f;
        float atkValue = (petAtkCoe + 2.0f + 2.5f) * petBaseCoe / 100f;
        float defValue = (petDefCoe + 2.0f + 2.5f) * petBaseCoe / 100f;
        float dexValue = (petDexCoe + 2.0f + 2.5f) * petBaseCoe / 100f;

        switch (type)
        {
            case STAT_TYPE.VIT:
                {
                    returnValue = (vitValue * 4f) + atkValue + defValue + dexValue;
                }
                break;
            case STAT_TYPE.ATK:
                {
                    returnValue = (vitValue * 0.1f) + atkValue + (defValue * 0.1f) + (dexValue * 0.05f);
                }
                break;
            case STAT_TYPE.DEF:
                {
                    returnValue = (vitValue * 0.1f) + (atkValue * 0.1f) + defValue + (dexValue * 0.05f);
                }
                break;
            case STAT_TYPE.DEX:
                {
                    returnValue = dexValue;
                }
                break;
        }

        return returnValue;
    }
    public float GetSCoeValue(STAT_TYPE type, float petVitCoe, float petAtkCoe, float petDefCoe, float petDexCoe)
    {
        float rankCal = petVitCoe + petAtkCoe + petDefCoe + petDexCoe;
        int rank = GetRank(rankCal);

        float returnValue = 0.0f;
        float rankCoe = (455f + (rank * 20f));

        float vitValue = (petVitCoe + 2.0f + 2.5f) * rankCoe / 10000f;
        float atkValue = (petAtkCoe + 2.0f + 2.5f) * rankCoe / 10000f;
        float defValue = (petDefCoe + 2.0f + 2.5f) * rankCoe / 10000f;
        float dexValue = (petDexCoe + 2.0f + 2.5f) * rankCoe / 10000f;

        switch (type)
        {
            case STAT_TYPE.VIT:
                {
                    returnValue = (vitValue * 4f) + atkValue + defValue + dexValue;
                }
                break;
            case STAT_TYPE.ATK:
                {
                    returnValue = (vitValue * 0.1f) + atkValue + (defValue * 0.1f) + (dexValue * 0.05f);
                }
                break;
            case STAT_TYPE.DEF:
                {
                    returnValue = (vitValue * 0.1f) + (atkValue * 0.1f) + defValue + (dexValue * 0.05f);
                }
                break;
            case STAT_TYPE.DEX:
                {
                    returnValue = dexValue;
                }
                break;
        }

        return returnValue;
    }
    public int GetRank(float rankCal)
    {
        int rank = 0;
        if (rankCal >= 100) rank = 1;
        if (95 <= rankCal && rankCal <= 99) rank = 2;
        if (90 <= rankCal && rankCal <= 94) rank = 3;
        if (85 <= rankCal && rankCal <= 89) rank = 4;
        if (80 <= rankCal && rankCal <= 84) rank = 5;
        if (rankCal <= 79) rank = 6;

        return rank;
    }
}