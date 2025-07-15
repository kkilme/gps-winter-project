#region Type

public enum EquipmentType
{
    None,
    Weapon,
    Helmet,
    Body,
    Cloak,
}

public enum WeaponType
{
    NoWeapon,
    Bow,
    DoubleSword,
    SingleSword,
    Spear,
    SwordAndShield,
    TwoHandedSword,
    Wand,
}

public enum ArmorType
{
    None,
    Helmet,
    Body,
    Cloak,
}

public enum ItemType
{
    Consumable,
    Armor,
    Weapon,
}

public enum AreaTileType
{
    OutOfField,
    ForceEmpty,
    Obstacle,
    Empty,
    MainTile,
    SubTile,
    Start,
    Normal,
    Battle,
    Encounter,
    Boss,
    Collapsed
}

public enum BattleType
{
    Normal,
    Boss,
}

public enum BattleResultType
{
    Victory,
    Defeat,
    Retreat,
}

public enum SceneType
{
    UnknownScene,
    TownScene,
    AreaScene,
    BattleScene,
}

public enum SoundType
{
    Bgm,
    Effect,
    MaxCount,
}

public enum ActionDataType
{
    ActionData,
    SkillData,
    AttackSkillData,
}

public enum AttackType
{
    Physical,
    Magic,
}

public enum AttackRangeType
{
    Melee,
    Ranged,
}

public enum DamageTextType
{
    NormalDamage,
    PhysicalDamage,
    MagicDamage,
    Heal,
}
#endregion

#region Attribute

public enum StatName
{
    None,
    BaseDamage,
    MaxHp,
    PhysicalDefense,
    MagicDefense,
    Strength,
    Intelligence,
    Vitality,
    Dexterity,
}

public enum GridSide
{
    HeroSide,
    MonsterSide,
}
#endregion

#region State
public enum BattleState
{
    BattleSceneNotLoaded,
    Starting,
    HeroPlacement,
    Idle,
    ActionTargetSelecting,
    ActionProcessing,
    Finishing,
}

public enum AreaState
{
    Idle,
    Busy,
    Moving,
    Battle,
    Encounter,
}

#endregion

#region Event

public enum UIEvent
{
    Click,
    DoubleClick,
    Drag,
    Enter,
    Exit,
    Stay,
}

public enum MouseEvent
{
    Press,
    PointerDown,
    PointerUp,
    Click,
    Hover,
}

#endregion

#region Name

public enum AreaName
{
    Forest,
    Desert,
}
#endregion

