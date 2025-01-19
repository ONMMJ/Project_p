using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;

enum DB_UPDATE_TYPE
{
    ResetNormal,
    ExpUp,
}
public class PetInfo
{
    public PetInfo()
    {

        this.petLevel = 1;
        this.resetCount = 0;
    }
    // 펫 불러오기 (DB에서 불러오기)
    public PetInfo(UserPetData petData) : this()
    {
        this.petDefault = BackendManager.Instance.Chart.PetChart.Dictionary[petData.petName];

        this.petNo = petData.petNo;
        this.petId = petData.petId;
        this.petRank = petData.petRank;
        this.isEnemy = petData.isEnemy;
        this.resetCount = petData.resetCount;

        this.petLevel = petData.petLevel;
        this.petNowExp = petData.petNowExp;
        this.petNextExp = petData.petNextExp;

        this.addVit = petData.addVit;
        this.addAtk = petData.addAtk;
        this.addDef = petData.addDef;
        this.addDex = petData.addDex;

        this.petVit = petData.petVit;
        this.petAtk = petData.petAtk;
        this.petDef = petData.petDef;
        this.petDex = petData.petDex;

        this.petBaseVit = petData.petBaseVit;
        this.petBaseAtk = petData.petBaseAtk;
        this.petBaseDef = petData.petBaseDef;
        this.petBaseDex = petData.petBaseDex;

        this.skillsLevel = petData.skillsLevel;

        SetUp();
    }

    //// 펫 생성 (처음 생성)
    //public PetInfo(string name, bool isEnemy) : this()
    //{
    //    PetInfo petDB;
    //    petDB = PetManager.Instance.GetLv1Pet(name);

    //    this.petDefault = petDB.petDefault;
    //    this.isEnemy = isEnemy;
    //    this.resetCount = 0;

    //    this.petLevel = petDB.petLevel;
    //    this.petRank = petDB.petRank;

    //    this.addVit = petDB.addVit;
    //    this.addAtk = petDB.addAtk;
    //    this.addDef = petDB.addDef;
    //    this.addDex = petDB.addDex;

    //    this.petBaseVit = petDB.petBaseVit;
    //    this.petBaseAtk = petDB.petBaseAtk;
    //    this.petBaseDef = petDB.petBaseDef;
    //    this.petBaseDex = petDB.petBaseDex;

    //    this.petVit = this.petBaseVit;
    //    this.petAtk = this.petBaseAtk;
    //    this.petDef = this.petBaseDef;
    //    this.petDex = this.petBaseDex;

    //    SetUp();
    //}

    public PetDefault petDefault;
    public int[] skillsLevel;

    public bool isEnemy;        // Pet으로 이동
    public int skillMaxPoint;
    public int petGrade;
    public int resetCount;

    public float petSrankVit;
    public float petSrankAtk;
    public float petSrankDef;
    public float petSrankDex;

    public float petSrankTotalCoe;
    public float petSrankVitCoe;
    public float petSrankAtkCoe;
    public float petSrankDefCoe;
    public float petSrankDexCoe;

    public int petNo;
    public string petId;
    public int petRank;

    public int petLevel;
    public int petNowExp;
    public int petNextExp;

    public float petBaseDex;
    public float petBaseVit;
    public float petBaseAtk;
    public float petBaseDef;

    public float petVitCoe;
    public float petAtkCoe;
    public float petDefCoe;
    public float petDexCoe;

    public STAT_COE_RANK addVit;
    public STAT_COE_RANK addAtk;
    public STAT_COE_RANK addDef;
    public STAT_COE_RANK addDex;

    public float petVit;
    public float petAtk;
    public float petDef;
    public float petDex;

    public float buffVit;
    public float buffAtk;
    public float buffDef;
    public float buffDex;

    public int startSkillPoint = 0;

    // 계산에 사용되는 실제 스탯
    public float FinalVit => ((petVit * 4f) + petAtk + petDef + petDex);
    public float FinalAtk => ((petVit * 0.1f) + petAtk + (petDef * 0.1f) + (petDex * 0.05f));
    public float FinalDef => ((petVit * 0.1f) + (petAtk * 0.1f) + petDef + (petDex * 0.05f));
    public float FinalDex => petDex;

    // 스탯창에서 보여지는 스탯(스텟에서 소수점 제외)
    public int ViewVit => (int)FinalVit;
    public int ViewAtk => (int)FinalAtk;
    public int ViewDef => (int)FinalDef;
    public int ViewDex => (int)FinalDex;

    public int ViewVitDiff => ViewVit - (int)(petSrankVit + (petLevel - 1) * petSrankVitCoe);
    public int ViewAtkDiff => ViewAtk - (int)(petSrankAtk + (petLevel - 1) * petSrankAtkCoe);
    public int ViewDefDiff => ViewDef - (int)(petSrankDef + (petLevel - 1) * petSrankDefCoe);
    public int ViewDexDiff => ViewDex - (int)(petSrankDex + (petLevel - 1) * petSrankDexCoe);

    // 스탯창에서 보여지는 초기스탯
    public int ViewBaseVit => (int)((petBaseVit * 4f) + petBaseAtk + petBaseDef + petBaseDex);
    public int ViewBaseAtk => (int)((petBaseVit * 0.1f) + petBaseAtk + (petBaseDef * 0.1f) + (petBaseDex * 0.05f));
    public int ViewBaseDef => (int)((petBaseVit * 0.1f) + (petBaseAtk * 0.1f) + petBaseDef + (petBaseDex * 0.05f));
    public int ViewBaseDex => (int)petBaseDex;

    // 초기스탯 S랭크와 차이 값
    public int ViewBaseVitDiff => ViewBaseVit - (int)petSrankVit;
    public int ViewBaseAtkDiff => ViewBaseAtk - (int)petSrankAtk;
    public int ViewBaseDefDiff => ViewBaseDef - (int)petSrankDef;
    public int ViewBaseDexDiff => ViewBaseDex - (int)petSrankDex;

    // 실제 성장수치
    public float petVitGrow => petLevel == 1 ? 0 : (float)(ViewVit - ViewBaseVit) / (float)(petLevel - 1);
    public float petAtkGrow => petLevel == 1 ? 0 : (float)(ViewAtk - ViewBaseAtk) / (float)(petLevel - 1);
    public float petDefGrow => petLevel == 1 ? 0 : (float)(ViewDef - ViewBaseDef) / (float)(petLevel - 1);
    public float petDexGrow => petLevel == 1 ? 0 : (float)(ViewDex - ViewBaseDex) / (float)(petLevel - 1);

    // 스탯창에서 보여지는 성장수치
    public float ViewVitGrow => (float)System.Math.Round(petVitGrow, 2);
    public float ViewAtkGrow => (float)System.Math.Round(petAtkGrow, 2);
    public float ViewDefGrow => (float)System.Math.Round(petDefGrow, 2);
    public float ViewDexGrow => (float)System.Math.Round(petDexGrow, 2);

    // 스탱창에서 보여지는 성장 등급
    public string ViewGradeVit => GetGrade(STAT_TYPE.VIT);
    public string ViewGradeAtk => GetGrade(STAT_TYPE.ATK);
    public string ViewGradeDef => GetGrade(STAT_TYPE.DEF);
    public string ViewGradeDex => GetGrade(STAT_TYPE.DEX);

    // 버프가 적용된 실제 스탯
    public float Vit => FinalVit * buffVit;
    public float Atk => FinalAtk * buffAtk;
    public float Def => FinalDef * buffDef;
    public float Dex => FinalDex * buffDex;

    public float RealDef => Def / 2f + petVit / 10f + petDex / 5f;

    public int testAllSCount = 0;

    public Sprite petIcon
    {
        get
        {
            if (BackendManager.Instance.Chart.PetChart.PetIcon.ContainsKey(petDefault.petAssetName))
                return BackendManager.Instance.Chart.PetChart.PetIcon[petDefault.petAssetName];
            else
                return Resources.Load<Sprite>("PetIcons/Null");
        }
    }

    public void SetUp()
    {
        this.petSrankVit = PetManager.Instance.GetSValue(STAT_TYPE.VIT, petDefault.petBaseCoe, petDefault.petVitCoe, petDefault.petAtkCoe, petDefault.petDefCoe, petDefault.petDexCoe);
        this.petSrankAtk = PetManager.Instance.GetSValue(STAT_TYPE.ATK, petDefault.petBaseCoe, petDefault.petVitCoe, petDefault.petAtkCoe, petDefault.petDefCoe, petDefault.petDexCoe);
        this.petSrankDef = PetManager.Instance.GetSValue(STAT_TYPE.DEF, petDefault.petBaseCoe, petDefault.petVitCoe, petDefault.petAtkCoe, petDefault.petDefCoe, petDefault.petDexCoe);
        this.petSrankDex = PetManager.Instance.GetSValue(STAT_TYPE.DEX, petDefault.petBaseCoe, petDefault.petVitCoe, petDefault.petAtkCoe, petDefault.petDefCoe, petDefault.petDexCoe);

        this.petSrankVitCoe = PetManager.Instance.GetSCoeValue(STAT_TYPE.VIT, petDefault.petVitCoe, petDefault.petAtkCoe, petDefault.petDefCoe, petDefault.petDexCoe);
        this.petSrankAtkCoe = PetManager.Instance.GetSCoeValue(STAT_TYPE.ATK, petDefault.petVitCoe, petDefault.petAtkCoe, petDefault.petDefCoe, petDefault.petDexCoe);
        this.petSrankDefCoe = PetManager.Instance.GetSCoeValue(STAT_TYPE.DEF, petDefault.petVitCoe, petDefault.petAtkCoe, petDefault.petDefCoe, petDefault.petDexCoe);
        this.petSrankDexCoe = PetManager.Instance.GetSCoeValue(STAT_TYPE.DEX, petDefault.petVitCoe, petDefault.petAtkCoe, petDefault.petDefCoe, petDefault.petDexCoe);

        //this.petBaseVit = petDefault.petBaseCoe * (petDefault.petVitCoe + 2f + 2.5f) / 100f;
        //this.petBaseAtk = petDefault.petBaseCoe * (petDefault.petAtkCoe + 2f + 2.5f) / 100f;
        //this.petBaseDef = petDefault.petBaseCoe * (petDefault.petDefCoe + 2f + 2.5f) / 100f;
        //this.petBaseDex = petDefault.petBaseCoe * (petDefault.petDexCoe + 2f + 2.5f) / 100f;

        this.petVitCoe = petDefault.petVitCoe + PetManager.Instance.RankToStat(addVit);
        this.petAtkCoe = petDefault.petAtkCoe + PetManager.Instance.RankToStat(addAtk);
        this.petDefCoe = petDefault.petDefCoe + PetManager.Instance.RankToStat(addDef);
        this.petDexCoe = petDefault.petDexCoe + PetManager.Instance.RankToStat(addDex);

        this.skillMaxPoint = petDefault.skillMaxPoint;
        this.petGrade = petDefault.petGrade;

        this.buffVit = 1f;
        this.buffAtk = 1f;
        this.buffDef = 1f;
        this.buffDex = 1f;
    }

    public void LevelUp()
    {
        float levelupBaseVit = petVitCoe;
        float levelupBaseAtk = petAtkCoe;
        float levelupBaseDef = petDefCoe;
        float levelupBaseDex = petDexCoe;

        for (int i = 0; i < 10; i++)
        {
            int work = Random.Range(0, 4);
            if (work == 0)
            {
                levelupBaseVit++;
            }
            if (work == 1)
            {
                levelupBaseAtk++;
            }
            if (work == 2)
            {
                levelupBaseDef++;
            }
            if (work == 3)
            {
                levelupBaseDex++;
            }
        }


        //???
        int growUpValue = Random.Range(430 + (petRank * 20), 480 + (petRank * 20));

        float hpUpValue = levelupBaseVit * growUpValue / 10000f;
        float atkUpValue = levelupBaseAtk * growUpValue / 10000f;
        float defUpValue = levelupBaseDef * growUpValue / 10000f;
        float dexUpValue = levelupBaseDex * growUpValue / 10000f;

        petVit += hpUpValue;
        petAtk += atkUpValue;
        petDef += defUpValue;
        petDex += dexUpValue;

        petLevel++;
    }
    public void EnemyLevelUp()
    {
        int growUpValue = 455 + (petRank * 20);

        float hpUpValue = petVitCoe + 2.5f * growUpValue / 10000f;
        float atkUpValue = petAtkCoe + 2.5f * growUpValue / 10000f;
        float defUpValue = petDefCoe + 2.5f * growUpValue / 10000f;
        float dexUpValue = petDexCoe + 2.5f * growUpValue / 10000f;

        petVit += hpUpValue;
        petAtk += atkUpValue;
        petDef += defUpValue;
        petDex += dexUpValue;

        petLevel++;
    }
    public void SetLevel(int level)
    {
        ResetLevel();
        int count = level - 1;
        for (int i = 0; i < count; i++)
        {
            LevelUp();
        }
    }
    public void SetEnemyLevel(int level)
    {
        ResetLevel();
        int count = level - 1;
        for (int i = 0; i < count; i++)
        {
            EnemyLevelUp();
        }
    }

    public void ResetLevel()
    {
        petVit = petBaseVit;
        petAtk = petBaseAtk;
        petDef = petBaseDef;
        petDex = petBaseDex;

        petLevel = 1;

        this.petNextExp = GetNextExp(petLevel);
        this.petNowExp = 0;
    }
    public void ApplyBuff(BuffInfo buff)
    {
        this.buffVit *= buff.buffVit;
        this.buffAtk *= buff.buffAtk;
        this.buffDef *= buff.buffDef;
        this.buffDex *= buff.buffDex;
    }
    public void ResetBuff()
    {
        buffVit = 1f;
        buffAtk = 1f;
        buffDef = 1f;
        buffDex = 1f;
    }

    public string ExpToString()
    {
        return $"{petNowExp} / {petNextExp}";
    }
    public float ExpPersent()
    {
        return petNowExp / (float)petNextExp;
    }
    public bool GetExp(int exp)
    {
        bool isLevelUp = false;
        petNowExp += exp;
        //        UIManager.Instance.ExpUpdate(this);

        while (petNowExp >= petNextExp)
        {
            petNowExp -= petNextExp; //경험치 빼준다.
            LevelUp();
            petNextExp = GetNextExp(petLevel);
            isLevelUp = true;
            //UIManager.Instance.SetBattlePetInfomation(this);
        }

        UpdateBackendData(DB_UPDATE_TYPE.ExpUp);
        return isLevelUp;
    }
    public int GetNextExp(int level)
    {
        return level * (level * 5);
    }

    public string GetGrade(STAT_TYPE stat)
    {
        switch (stat)
        {
            case STAT_TYPE.VIT:
                return CalcuGradeVit(petVitGrow, petSrankVitCoe);
            case STAT_TYPE.ATK:
                return CalcuGrade(petAtkGrow, petSrankAtkCoe);
            case STAT_TYPE.DEF:
                return CalcuGrade(petDefGrow, petSrankDefCoe);
            case STAT_TYPE.DEX:
                return CalcuGrade(petDexGrow, petSrankDexCoe);
        }
        return "0";
    }
    public string CalcuGrade(float Cur, float Grade)
    {
        float Diff = Cur - Grade;
        if (Diff >= 0.006f)
            return "S+";
        if (Diff >= -0.006f)
            return "S";
        if (Diff >= -0.033f)
            return "A+";
        if (Diff >= -0.048f)
            return "A";
        if (Diff >= -0.066f)
            return "B+";
        if (Diff >= -0.081f)
            return "B";
        if (Diff >= -0.098f)
            return "C+";
        if (Diff >= -0.109f)
            return "C";
        if (Diff >= -0.131f)
            return "D+";
        if (Diff >= -0.147f)
        {
            return "D";
        }
        return "0";
    }
    public string CalcuGradeVit(float Cur, float Grade)
    {
        float Diff = Cur - Grade;
        if (Diff >= 0.02f)
            return "S+";
        if (Diff >= -0.02f)
            return "S";
        if (Diff >= -0.05f)
            return "A+";
        if (Diff >= -0.1f)
            return "A";
        if (Diff >= -0.15f)
            return "B+";
        if (Diff >= -0.2f)
            return "B";
        if (Diff >= -0.25f)
            return "C+";
        if (Diff >= -0.3f)
            return "C";
        if (Diff >= -0.35f)
            return "D+";
        if (Diff >= -0.4f)
        {
            return "D";
        }
        return "0";
    }

    public void ResetNoraml()
    {
        /* Prev Ver
        List<STAT_COE_RANK> ranks = PetManager.Instance.RandomCoeRank();
        addVit = ranks[0];
        addAtk = ranks[1];
        addDef = ranks[2];
        addDex = ranks[3];

        petVitCoe = petDefault.petVitCoe + PetManager.Instance.RankToStat(addVit);
        petAtkCoe = petDefault.petAtkCoe + PetManager.Instance.RankToStat(addAtk);
        petDefCoe = petDefault.petDefCoe + PetManager.Instance.RankToStat(addDef);
        petDexCoe = petDefault.petDexCoe + PetManager.Instance.RankToStat(addDex);

        testAllSCount = 0;

        ResetLevel();
        */
        if (resetCount >= PetManager.Instance.maxResetCount)
            return;

        resetCount++;

        List<STAT_COE_RANK> ranks = PetManager.Instance.RandomCoeRank();
        addVit = ranks[0];
        addAtk = ranks[1];
        addDef = ranks[2];
        addDex = ranks[3];

        petVitCoe = petDefault.petVitCoe + PetManager.Instance.RankToStat(addVit);
        petAtkCoe = petDefault.petAtkCoe + PetManager.Instance.RankToStat(addAtk);
        petDefCoe = petDefault.petDefCoe + PetManager.Instance.RankToStat(addDef);
        petDexCoe = petDefault.petDexCoe + PetManager.Instance.RankToStat(addDex);

        float levelUpVitCoe = PetManager.Instance.RankToStat(addVit);
        float levelUpAtkCoe = PetManager.Instance.RankToStat(addAtk);
        float levelUpDefCoe = PetManager.Instance.RankToStat(addDef);
        float levelUpDexCoe = PetManager.Instance.RankToStat(addDex);

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
        petBaseVit = petDefault.petBaseCoe * (petDefault.petVitCoe + levelUpVitCoe) / 100f;
        petBaseAtk = petDefault.petBaseCoe * (petDefault.petAtkCoe + levelUpAtkCoe) / 100f;
        petBaseDef = petDefault.petBaseCoe * (petDefault.petDefCoe + levelUpDefCoe) / 100f;
        petBaseDex = petDefault.petBaseCoe * (petDefault.petDexCoe + levelUpDexCoe) / 100f;

        ResetLevel();

        UpdateBackendData(DB_UPDATE_TYPE.ResetNormal);
    }
    public bool ResetRevive()
    {
        addVit = PetManager.Instance.ReviveAddStat(addVit);
        addAtk = PetManager.Instance.ReviveAddStat(addAtk);
        addDef = PetManager.Instance.ReviveAddStat(addDef);
        addDex = PetManager.Instance.ReviveAddStat(addDex);

        //addVit = STAT_COE_RANK.SP;
        //addAtk = STAT_COE_RANK.SP;
        //addDef = STAT_COE_RANK.SP;
        //addDex = STAT_COE_RANK.SP;

        petVitCoe = petDefault.petVitCoe + PetManager.Instance.RankToStat(addVit);
        petAtkCoe = petDefault.petAtkCoe + PetManager.Instance.RankToStat(addAtk);
        petDefCoe = petDefault.petDefCoe + PetManager.Instance.RankToStat(addDef);
        petDexCoe = petDefault.petDexCoe + PetManager.Instance.RankToStat(addDex);

        float testSum = 0;

        testSum += PetManager.Instance.RankToStat(addVit);
        testSum += PetManager.Instance.RankToStat(addAtk);
        testSum += PetManager.Instance.RankToStat(addDef);
        testSum += PetManager.Instance.RankToStat(addDex);

        testAllSCount++;
        ResetLevel();

        return testSum > 7.999f;
    }

    private void UpdateBackendData(DB_UPDATE_TYPE type)
    {
        BackendManager.Instance.GameData.UserPetData.UpdatePetData(ExportData());
        switch (type)
        {
            case DB_UPDATE_TYPE.ResetNormal:
                BackendManager.Instance.GameData.UserPetData.Update((callback) =>
                {
                    if (callback.IsSuccess())
                    {
                        UIManager.Instance.InvenUI.SetContent();
                        BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
                        Debug.Log($"업데이트 성공: {callback}");
                    }
                    else
                    {
                        Debug.LogError($"업데이트 실패: {callback})");
                    }
                });
                break;
            case DB_UPDATE_TYPE.ExpUp:
                BackendManager.Instance.GameData.UserPetData.Update((callback) =>
                {
                    if (callback.IsSuccess())
                    {
                        BackendManager.Instance.GameData.UserPetData.IsChangedData = false;
                        Debug.Log($"업데이트 성공: {callback}");
                    }
                    else
                    {
                        Debug.LogError($"업데이트 실패: {callback})");
                    }
                });
                break;

        }
    }

    public UserPetData ExportData()
    {
        UserPetData userPetData = new UserPetData();

        userPetData.petName = this.petDefault.petName;

        userPetData.petNo = this.petNo;
        userPetData.petId = this.petId;
        userPetData.petRank = this.petRank;
        userPetData.isEnemy = this.isEnemy;
        userPetData.resetCount = this.resetCount;

        userPetData.petLevel = this.petLevel;
        userPetData.petNowExp = this.petNowExp;
        userPetData.petNextExp = this.petNextExp;

        userPetData.addVit = this.addVit;
        userPetData.addAtk = this.addAtk;
        userPetData.addDef = this.addDef;
        userPetData.addDex = this.addDex;

        userPetData.petVit = this.petVit;
        userPetData.petAtk = this.petAtk;
        userPetData.petDef = this.petDef;
        userPetData.petDex = this.petDex;

        userPetData.petBaseVit = this.petBaseVit;
        userPetData.petBaseAtk = this.petBaseAtk;
        userPetData.petBaseDef = this.petBaseDef;
        userPetData.petBaseDex = this.petBaseDex;

        userPetData.skillsLevel = this.skillsLevel;

        return userPetData;
    }
    //public PetInfo Copy()
    //{
    //    PetInfo copy = new PetInfo();
    //    copy.petPrefab = this.petPrefab;
    //    copy.skills = this.skills;
    //    copy.petDefault = this.petDefault;

    //    copy.petNo = this.petNo;
    //    copy.petId = this.petId;
    //    copy.petRank = this.petRank;
    //    copy.isEnemy = this.isEnemy;
    //    copy.resetCount = this.resetCount;

    //    copy.petLevel = this.petLevel;
    //    copy.petNowExp = this.petNowExp;
    //    copy.petNextExp = this.petNextExp;

    //    copy.addVit = this.addVit;
    //    copy.addAtk = this.addAtk;
    //    copy.addDef = this.addDef;
    //    copy.addDex = this.addDex;

    //    copy.petVit = this.petVit;
    //    copy.petAtk = this.petAtk;
    //    copy.petDef = this.petDef;
    //    copy.petDex = this.petDex;

    //    copy.petPrefab = this.petPrefab;

    //    copy.petSrankVit = this.petSrankVit;
    //    copy.petSrankAtk = this.petSrankAtk;
    //    copy.petSrankDef = this.petSrankDef;
    //    copy.petSrankDex = this.petSrankDex;

    //    copy.petSrankVitCoe = this.petSrankVitCoe;
    //    copy.petSrankAtkCoe = this.petSrankAtkCoe;
    //    copy.petSrankDefCoe = this.petSrankDefCoe;
    //    copy.petSrankDexCoe = this.petSrankDexCoe;

    //    copy.petBaseVit = this.petBaseVit;
    //    copy.petBaseAtk = this.petBaseAtk;
    //    copy.petBaseDef = this.petBaseDef;
    //    copy.petBaseDex = this.petBaseDex;

    //    copy.petVitCoe = this.petVitCoe;
    //    copy.petAtkCoe = this.petAtkCoe;
    //    copy.petDefCoe = this.petDefCoe;
    //    copy.petDexCoe = this.petDexCoe;

    //    copy.skillMaxPoint = this.skillMaxPoint;

    //    copy.buffVit = this.buffVit;
    //    copy.buffAtk = this.buffAtk;
    //    copy.buffDef = this.buffDef;
    //    copy.buffDex = this.buffDex;

    //    return copy;
    //}

   public void SetGame()
    {
        ResetBuff();
    }
}