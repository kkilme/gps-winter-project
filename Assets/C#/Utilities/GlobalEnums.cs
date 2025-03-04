#region Type

public enum CreatureType
{
    None,
    Hero,
    Monster,
}

public enum EquipmentType
{
    None,
    Weapon,
    Armor,
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
    Accessory,
    Body,
    Cloak,
    HeadAccessory,
    Helmet,
}

public enum ItemType
{
    None,
    Attack,
    Buff,
    Debuff,
    Recover,
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
    Destroyed
}

public enum QuestRewardType
{
    Money,
}

public enum BattleResultType
{
    Victory,
    Defeat,
    Flee,
}

public enum RewardActionType
{
    Take,
    Pass,
    Dispose
}

public enum SceneType
{
    UnknownScene,
    AreaScene,
    BattleScene,
    TestGameScene,
    TestTitleScene,
    TitleScene,
    TownScene,
}

public enum SoundType
{
    Bgm,
    Effect,
    MaxCount,
}

public enum ActionDataType
{
    SkillData,
    AttackSkillData,
}

public enum AttackType
{
    Physical,
    Magical,
}

public enum AttackRangeType
{
    Melee,
    Ranged,
}
#endregion

#region Attribute

public enum StatName
{
    None,
    Strength,
    Intelligence,
    Vitality,
    Dexterity,
    Monster,
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
    Starting,
    HeroPlacement,
    Idle,
    ActionTargetSelecting,
    ActionProcessing,
    Ending,
}
public enum CreatureBattleState
{
    Wait,
    PrepareAction,
    ActionProceed,
    Dead
}

public enum AnimState
{
    Attack,
    Defend,
    DefendHit,
    Die,
    Dizzy,
    Hit,
    Idle,
    Move,
    Skill,
    Victory
}

public enum AreaState
{
    Idle,
    Moving,
    Battle,
    Encounter,
    Boss,
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

#region NonContent

public enum Layer
{
    Ground = 6,
    Block = 7,
    Monster = 8,
    Player = 9,
}

public enum CameraMode
{
    QuarterView,
}

#endregion

#region Name

public enum AreaName
{
    Forest,
    Desert,
}
#endregion

