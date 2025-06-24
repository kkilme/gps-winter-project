using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TownScene : UI_Scene
{
    enum SubItemUI
    {
        UI_TownTopBar,
        UI_Town_Inventory,
        UI_Town_HeroList,
        UI_Town_Store,
        UI_QuestBoard,
        UI_HeroProfileGroup_Horizontal,

        UI_PartyFormation
    }

    enum Buttons
    {
        // TopBar Buttons
        Button_Inventory,
        Button_HeroList,
        Button_Store,
        Button_Quest,
        Button_Setting,

        // SideBar Buttons
        Button_PartyFormation,
    }

    public UI_Base CurrentOpenUI { get; set; } // 각종 마을 씬의 UI중 현재 열린 UI
    public UI_TownInventory InventoryUI { get; protected set; }
    public UI_HeroList HeroListUI { get; protected set; }
    public UI_TownStore StoreUI { get; protected set; }
    public UI_QuestBoard QuestBoardUI { get; protected set; }
    public UI_HeroProfileGroup HeroProfileGroupUI { get; protected set; }

    public UI_PartyFormation PartyFormationUI { get; protected set; }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(SubItemUI));
        Bind<Button>(typeof(Buttons));

        InventoryUI = GetGameObject(SubItemUI.UI_Town_Inventory).GetOrAddComponent<UI_TownInventory>();
        HeroListUI = GetGameObject(SubItemUI.UI_Town_HeroList).GetOrAddComponent<UI_HeroList>();
        StoreUI = GetGameObject(SubItemUI.UI_Town_Store).GetOrAddComponent<UI_TownStore>();
        QuestBoardUI = GetGameObject(SubItemUI.UI_QuestBoard).GetOrAddComponent<UI_QuestBoard>();
        PartyFormationUI = GetGameObject(SubItemUI.UI_PartyFormation).GetOrAddComponent<UI_PartyFormation>();
        HeroProfileGroupUI = GetGameObject(SubItemUI.UI_HeroProfileGroup_Horizontal).GetOrAddComponent<UI_HeroProfileGroup>();

        GetButton(Buttons.Button_Inventory).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly(); InventoryUI.Show(); CurrentOpenUI = InventoryUI; });
        GetButton(Buttons.Button_HeroList).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly(); HeroListUI.Show(); CurrentOpenUI = HeroListUI; });
        GetButton(Buttons.Button_Store).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly(); StoreUI.Show(); CurrentOpenUI = StoreUI; });
        GetButton(Buttons.Button_Quest).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly(); QuestBoardUI.Show(); CurrentOpenUI = QuestBoardUI; });
        GetButton(Buttons.Button_PartyFormation).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly();  PartyFormationUI.Show(); CurrentOpenUI = PartyFormationUI; });
    }

    public void InitUIs()
    {
        // UI_Base의 Init()이 아닌 각 UI 클래스의 LateInit으로 수동 초기화를 진행함.
        // UI_Base의 Init()은 Awake에서 호출되기 때문에 대표적으로 두 가지 문제가 존재.
        // 1. 모든 UI가 동시에 Awake에서 초기화되기 때문에 순서 관계를 명확히 지정할 수가 없음. A 초기화가 완료된 이후 B를 초기화해야만 할 시, 문제가 발생함.
        // 2. Awake는 게임오브젝트가 활성화 상태일 때만 호출됨. 즉, 어떤 UI를 비활성화 상태로 게임을 시작한다면 제대로 초기화가 안되어 문제가 발생할 수 있음.
        // 이를 해결하기 위해 LateInit에서는 ShowInstantly()를 통해 UI를 직접적으로 활성화 시킨 후 초기화를 진행한다.
        // 단, HideInstantly()로 해당 UI를 다시 비활성화해야할 지의 여부는 UI마다 다르다. 
        InventoryUI.LateInit();
        HeroListUI.HideInstantly();
        StoreUI.LateInit();
        QuestBoardUI.LateInit();
    }
}
