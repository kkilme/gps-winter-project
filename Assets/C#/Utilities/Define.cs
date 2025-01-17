public static class Define
{
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

    public enum ActionTargetType
    {
        Single,
        Cross,
        Horizontal,
        Vertical,
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
        ActionData,
        AttackActionData,
    }
    public enum ActionType
    {
        Move,
        Flee,
        Attack,
        Buff,
    }

    public enum AttackType
    {
        Physical,
        Magical,
    }
    #endregion

    #region Attribute

    public enum Stat
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
        EnemySide,
    }
    #endregion

    #region State
    public enum BattleState
    {
        Starting,
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

    #region Path

    public const string HERO_PATH = "Heroes";
    public const string MONSTER_PATH = "Monsters";

    #endregion

    #region Name

    public enum AreaName
    {
        Forest,
        Desert,
    }

    public const string BATTLE_SCENE_NAME = "BattleScene";
    public const string AREA_SCENE_NAME = "AreaScene";

    #endregion

    #region DataId

    public const int HERO_KNIGHT_ID = 101000;
    public const int HERO_WIZARD_ID = 101001;

    public const int MONSTER_BAT_ID = 102000;

    public const int KNIGHT_START_WEAPON_ID = 201000;
    public const int WIZARD_START_WEAPON_ID = 201002;

    public const int ARMOR_SAMPLEBODY1_ID = 202000;
    public const int ARMOR_SAMPLEBODY2_ID = 202001;

    public const int ITEM_HEALPOTION_ID = 301000;

    public const int ACTION_MOVE_ID = 401000;
    public const int ACTION_FLEE_ID = 401001;
    public const int ACTION_STRIKE_ID = 402000;
    public const int ACTION_BITE_ID = 403000;

    public const int MONSTERSQUAD_SQUAD1_ID = 501000;

    #endregion

    #region Value

    public const float MOVE_SPEED = 5f;
    public const float BATTLEFIELD_POS_X = -1000f;
    public const float BATTLEFIELD_POS_Z = -1000f;

    // Animation Name
    public static readonly int ANIMATION_DEFEND = UnityEngine.Animator.StringToHash("Defend");
    public static readonly int ANIMATION_DEFEND_HIT = UnityEngine.Animator.StringToHash("DefendHit");
    public static readonly int ANIMATION_DIZZY = UnityEngine.Animator.StringToHash("Dizzy");
    public static readonly int ANIMATION_IDLE = UnityEngine.Animator.StringToHash("Idle");
    public static readonly int ANIMATION_JUMP = UnityEngine.Animator.StringToHash("Jump");
    public static readonly int ANIMATION_MOVE = UnityEngine.Animator.StringToHash("Move");
    public static readonly int ANIMATION_MOVEAPPROACH = UnityEngine.Animator.StringToHash("MoveApproach");
    public static readonly int ANIMATION_VICTORY = UnityEngine.Animator.StringToHash("Victory");

    // Animation Parameter
    public static readonly int PARAMETER_ATTACK_FINISHED = UnityEngine.Animator.StringToHash("AttackFinished");
    public static readonly int PARAMETER_APPROACH_FINISHED = UnityEngine.Animator.StringToHash("ApproachFinished");
    public static readonly int PARAMETER_NEEDS_JUMP = UnityEngine.Animator.StringToHash("NeedsJump");
    public static readonly int PARAMETER_NEEDS_MOVE = UnityEngine.Animator.StringToHash("NeedsMove");

    #endregion
}
