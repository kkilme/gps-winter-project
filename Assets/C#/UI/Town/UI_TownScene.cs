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
        UI_Town_TopBar,
        UI_Town_Inventory,
        UI_Town_HeroList,
        UI_Town_Store,
        UI_Town_Quest,
        UI_HeroProfileGroup_Horizontal,
    }

    enum Buttons
    {
        Button_Inventory,
        Button_HeroList,
        Button_Store,
        Button_Quest,
        Button_Setting,
    }

    public UI_Base CurrentOpenUI { get; set; }
    public UI_TownInventory InventoryUI { get; protected set; }
    public UI_HeroList HeroListUI { get; protected set; }
    public UI_TownStore StoreUI { get; protected set; }
    public UI_QuestBoard QuestBoardUI { get; protected set; }
    public UI_HeroProfileGroup HeroProfileGroupUI { get; protected set; }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(SubItemUI));
        Bind<Button>(typeof(Buttons));

        InventoryUI = GetGameObject(SubItemUI.UI_Town_Inventory).GetOrAddComponent<UI_TownInventory>();
        HeroListUI = GetGameObject(SubItemUI.UI_Town_HeroList).GetOrAddComponent<UI_HeroList>();
        StoreUI = GetGameObject(SubItemUI.UI_Town_Store).GetOrAddComponent<UI_TownStore>();
        //QuestBoardUI = GetGameObject(SubItemUI.UI_Town_Quest).GetOrAddComponent<UI_QuestBoard>();
        //HeroProfileGroupUI = GetGameObject(SubItemUI.UI_HeroProfileGroup_Horizontal).GetOrAddComponent<UI_HeroProfileGroup>();

        GetButton(Buttons.Button_Inventory).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly(); InventoryUI.Show(); CurrentOpenUI = InventoryUI; });
        GetButton(Buttons.Button_HeroList).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly(); HeroListUI.Show(); CurrentOpenUI = HeroListUI; });
        GetButton(Buttons.Button_Store).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly(); StoreUI.Show(); CurrentOpenUI = StoreUI; });
        //GetButton(Buttons.Button_Quest).onClick.AddListener(() => { CurrentOpenUI?.HideInstantly(); QuestBoardUI.Show(); CurrentOpenUI = QuestBoardUI; });
    }

    public void InitUIs()
    {
        InventoryUI.LateInit();
        HeroListUI.HideInstantly();
        StoreUI.LateInit();
    }
}
