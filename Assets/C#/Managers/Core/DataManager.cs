using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

// 시작하면 바로 데이터를 Load하여 Dict로 관리
public class DataManager
{
    public Dictionary<int, HeroData> HeroDataDict { get; private set; }
    public Dictionary<int, MonsterData> MonsterDataDict { get; private set; }
    public Dictionary<int, MonsterSquadData> MonsterSquadDataDict { get; private set; }

    public Dictionary<int, ItemData> ItemDataDict { get; private set; }
    public Dictionary<int, ConsumableItemData> ConsumableItemDataDict { get; private set; }
    public Dictionary<int, WeaponData> WeaponDataDict { get; private set; }
    public Dictionary<int, ArmorData> ArmorDataDict { get; private set; }

    public Dictionary<int, SkillData> SkillDataDict { get; private set; }

    public Dictionary<AreaName, AreaData> AreaDataDict { get; private set; }
    public Dictionary<int, QuestData> QuestDataDict { get; private set; }

    public void Init()
    {
        HeroDataDict = LoadJson<HeroDataLoader, int, HeroData>("HeroData").MakeDict();
        MonsterDataDict = LoadJson<MonsterDataLoader, int, MonsterData>("MonsterData").MakeDict();
        MonsterSquadDataDict = LoadJson<MonsterSquadDataLoader, int, MonsterSquadData>("MonsterSquadData").MakeDict();
        ConsumableItemDataDict = LoadJson<ConsumableItemDataLoader, int, ConsumableItemData>("ConsumableItemData").MakeDict();
        SkillDataDict = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
        WeaponDataDict = LoadJson<WeaponDataLoader, int, WeaponData>("WeaponData").MakeDict();
        ArmorDataDict = LoadJson<ArmorDataLoader, int, ArmorData>("ArmorData").MakeDict();
        AreaDataDict = LoadJson<AreaDataSet, AreaName, AreaData>("AreaData").MakeDict();
        QuestDataDict = LoadJson<QuestDataLoader, int, QuestData>("QuestData").MakeDict();

        InitItemDataDict();
    }

    private void InitItemDataDict()
    {
        ItemDataDict = new Dictionary<int, ItemData>();

        foreach (var kvp in ConsumableItemDataDict)
        {
            ItemDataDict[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in WeaponDataDict)
        {
            ItemDataDict[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in ArmorDataDict)
        {
            ItemDataDict[kvp.Key] = kvp.Value;
        }
    }

    // path 위치의 Json 파일을 TextAsset 타입으로 로드
    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.ResourceMng.Load<TextAsset>($"Datas/{path}");

        // SkillDataLoader에는 커스텀 컨버터 적용.
        if (typeof(Loader) == typeof(SkillDataLoader))
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new SkillDataConverter() }
            };
            return JsonConvert.DeserializeObject<Loader>(textAsset.text, settings);
        }
        else
        {
            return JsonConvert.DeserializeObject<Loader>(textAsset.text);
        }
    }
}
