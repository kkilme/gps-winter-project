using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// DamageText UI 생성하는 Factory
public static class DamageTextFactory
{
    private static GameObject _damageTextPrefab;
    private static Dictionary<DamageTextType, DamageTextDesign> _designCache = new();

    // 게임 시작 시 한번만 할당하여 사용
    public static void Init()
    {
         _damageTextPrefab ??= Managers.ResourceMng.Load<GameObject>("Prefabs/UI/WorldSpaceUI/UI_DamageText");
        _designCache[DamageTextType.NormalDamage] = new NormalDamageTextDesign();
        _designCache[DamageTextType.PhysicalDamage] = new PhysicalDamageTextDesign();
        _designCache[DamageTextType.MagicDamage] = new MagicDamageTextDesign();
        _designCache[DamageTextType.Heal] = new HealTextDesign();
    }

    /// <summary>
    /// DamageText UI를 생성하는 Factory.
    /// </summary>
    public static void CreateDamageText(Creature creature, int amount, DamageTextType type)
    {
        var damageText = Managers.UIMng.MakeWorldSpaceUI<UI_DamageText>();
        damageText.transform.position = creature.transform.position;
        damageText.transform.rotation = Quaternion.identity;

        if (_designCache.TryGetValue(type, out DamageTextDesign design))
        {
            damageText.Show(amount, design);
        }
    }
}
