using UnityEngine;

public static class GlobalValues
{
    #region Value
    public const float BATTLEFIELD_POS_X = -1000f;
    public const float BATTLEFIELD_POS_Z = -1000f;

    // 2X3을 유지할 계획이지만, 나중에 변경될 수 있으므로 변수로 선언
    public const int BATTLEGRID_ROW_COUNT = 2;
    public const int BATTLEGRID_COL_COUNT = 3;

    public const float CREATURE_BATTLE_VELOCITY = 5f;

    public const int MAX_COINT_COUNT = 8;

    public static readonly int[,] DIRECTION_4WAY = new int[4, 2] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
    public static readonly int[,] HERO_TILE_POS_OFFSET = new int[4, 2] { { 0, 1 }, { -1, 0 }, { 1, 0 }, { 0, -1 } };

    // Flat-top Hexagon 타일맵에서는 X좌표의 홀/짝 여부에 따라 방향에 해당하는 좌표값이 다름
    public static readonly int[,] DIRECTION_6WAY_X_ODD = new int[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };
    public static readonly int[,] DIRECTION_6WAY_X_EVEN = new int[6, 2] { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } };

    public static readonly int ANIMATION_PARAM_MOVING = Animator.StringToHash("Moving");
    public static readonly int ANIMATION_PARAM_ATTACK = Animator.StringToHash("Attack");

    #endregion

    #region Strings

    public const string BATTLE_SCENE_NAME = "BattleScene";
    public const string AREA_SCENE_NAME = "AreaScene";

    public const string HERO_PREFAB_PATH_ROOT = "Heroes";
    public const string MONSTER_PREFAB_PATH_ROOT = "Monsters";

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

    #region Color

    public static readonly Color HEROGRID_OUTLINE_HIGHLIGHT_COLOR = Color.green;
    public static readonly Color HEROGRID_FILL_HIGHLIGHT_COLOR = new Color(0.4f, 1, 0.4f);
    public static readonly Color ENEMYGRID_OUTLINE_HIGHLIGHT_COLOR = Color.red;
    public static readonly Color ENEMYGRID_FILL_HIGHLIGHT_COLOR = new Color(1, 0.2f, 0.2f);

    #endregion

    #region layermask

    public static readonly LayerMask LAYERMASK_BATTLEGROUND = LayerMask.GetMask("BattleGround");
    public static readonly LayerMask LAYERMASK_BATTLEGRIDCELL = LayerMask.GetMask("BattleGridCell");

    #endregion
}
