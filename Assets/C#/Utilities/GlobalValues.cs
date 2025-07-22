using System.Collections.Generic;
using UnityEngine;

public static class GlobalValues
{
    #region Value
    public const float BATTLEFIELD_POS_X = -1000f;
    public const float BATTLEFIELD_POS_Z = -1000f;

    public const int BATTLEGRID_ROW_COUNT = 2;
    public const int BATTLEGRID_COL_COUNT = 3;

    public const float CREATURE_BATTLE_VELOCITY = 5f; // 전투에서, Creature들의 이동속도.

    public const int MAX_COIN_COUNT = 8; // 게임에서 진행되는 모든 코인 토스 관련 시스템에서, 가능한 최대 코인 수
    public const int MAX_PARTY_SIZE = 4; // 파티 인원 최대 수
    public const int MAX_AREAITEM_COUNT = 8; // Area에 가져갈 수 있는 아이템의 최대 개수. 수정 시 관련 UI(UI_QuestDetailPanel, UI_AreaItemListPopup)프리팹도 수정 필요.

    public const int AREA_REST_PROGRESS_COLLAPSE_COUNT = 3; // Area에서 휴식 시 진행되는 붕괴 진척도

    public static readonly int[,] DIRECTION_4WAY = new int[4, 2] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
    public static readonly int[,] HERO_POS_ON_AREA_TILE_OFFSET = new int[4, 2] { { 0, 1 }, { -1, 0 }, { 1, 0 }, { 0, -1 } };

    // Flat-top Hexagon 타일맵에서는 X좌표의 홀/짝 여부에 따라 방향에 해당하는 좌표값이 다름
    public static readonly int[,] DIRECTION_6WAY_X_ODD = new int[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };
    public static readonly int[,] DIRECTION_6WAY_X_EVEN = new int[6, 2] { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } };

    public static readonly int ANIMATION_PARAM_MOVING = Animator.StringToHash("Moving");
    public static readonly int ANIMATION_PARAM_ONDAMAGED = Animator.StringToHash("OnDamaged");
    public static readonly int ANIMATION_PARAM_DEAD = Animator.StringToHash("Dead");
    public static readonly int ANIMATION_PARAM_TOWNSCENE = Animator.StringToHash("TownScene");
    public static readonly int ANIMATION_PARAM_ATTACK1 = Animator.StringToHash("Attack1");
    public static readonly int ANIMATION_PARAM_ATTACK2 = Animator.StringToHash("Attack2");
    public static readonly int ANIMATION_PARAM_ATTACK3 = Animator.StringToHash("Attack3");
    public static readonly int ANIMATION_PARAM_ATTACK4 = Animator.StringToHash("Attack4");

    #endregion

    #region Strings

    public const string BATTLE_SCENE_NAME = "BattleScene";
    public const string AREA_SCENE_NAME = "AreaScene";
    public const string TOWN_SCENE_NAME = "TownScene";

    public const string HERO_PREFAB_PATH_PREFIX = "Heroes/";
    public const string MONSTER_PREFAB_PATH_PREFIX = "Monsters/";
    public const string BATTLEFIELD_PATH_PREFIX = "Battle/Field/";

    public const string HERO_ANIMATOR_PATH_PREFIX = "Animators/Heroes/";

    public const string ACTIONICON_PATH_PREFIX = "Textures/ActionIcons/";
    public const string CREATURE_IMAGE_PATH_PREFIX = "Textures/ModelSprites/";
    public const string ITEMIMAGE_PATH_PREFIX = "Textures/ItemImages/";
    public const string ITEMICON_PATH_PREFIX = "Textures/ItemIcons/";
    public const string COLORGRADIENT_PATH_PREFIX = "Textures/TMP_ColorGradients/";
    public const string TOWNTEXTURE_PATH_PREFIX = "Textures/Town/";
    public const string AREAENCOUNTER_PATH_PREFIX = "Textures/EncounterImages/";
    #endregion

    #region DataId
    // 주로 테스트용으로 사용
    public const int HERO_KNIGHT_ID = 101000;
    public const int HERO_WIZARD_ID = 101001;

    public const int MONSTER_BAT_ID = 102000;

    public const int HERO_HANDWEAPON_ID = 201003; // 무기 장착 안된 상태의 Hero가 사용하는 무기
    public const int KNIGHT_START_WEAPON_ID = 201000;
    public const int WIZARD_START_WEAPON_ID = 201002;

    public const int ARMOR_SAMPLEBODY1_ID = 202000;
    public const int ARMOR_SAMPLEBODY2_ID = 202001;

    public const int ITEM_HEALPOTION_ID = 301000;

    public const int ACTION_MOVE_ID = 401000;
    public const int ACTION_FLEE_ID = 401001;
    public const int ACTION_BAG_ID = 401002;
    public const int ACTION_STRIKE_ID = 402000;
    public const int ACTION_BITE_ID = 403000;

    public const int MONSTERSQUAD_SQUAD1_ID = 501000;

    public const int AREAENCOUNTER_POND_ID = 701000;

    #endregion

    #region Color

    public static readonly Color HEROGRID_OUTLINE_HIGHLIGHT_COLOR = Color.green;
    public static readonly Color HEROGRID_FILL_HIGHLIGHT_COLOR = new Color(0.4f, 1, 0.4f);
    public static readonly Color ENEMYGRID_OUTLINE_HIGHLIGHT_COLOR = Color.red;
    public static readonly Color ENEMYGRID_FILL_HIGHLIGHT_COLOR = new Color(1, 0.2f, 0.2f);

    public static readonly Color PHYSICAL_UI_ELEMENT_COLOR = new Color(0, 0.5f, 1);
    public static readonly Color MAGIC_UI_ELEMENT_COLOR = new Color(0.65f, 0, 1);
    public static readonly Color HEAL_UI_ELEMENT_COLOR = Color.green;

    #endregion

    #region layermask

    public static readonly LayerMask LAYERMASK_BATTLEGROUND = LayerMask.GetMask("BattleGround");
    public static readonly LayerMask LAYERMASK_BATTLEGRIDCELL = LayerMask.GetMask("BattleGridCell");

    #endregion

    #region Dictionarys

    #endregion
}
