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
        UI_Town_Heroes,
        UI_Town_Store,
        UI_Town_Quest,
        UI_HeroProfileGroup_Horizontal,
    }

    enum Buttons
    {
        Button_Inventory,
        Button_Heroes,
        Button_Store,
        Button_Quest,
        Button_Setting,
    }

    private UI_Base _currentOpenUI;
    public UI_TownInventory InventoryUI { get; protected set; }
    public UI_HeroList HeroesUI { get; protected set; }
    public UI_TownStore StoreUI { get; protected set; }
    public UI_QuestBoard QuestBoardUI { get; protected set; }
    public UI_HeroProfileGroup HeroProfileGroupUI { get; protected set; }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(SubItemUI));
        Bind<Button>(typeof(Buttons));

        InventoryUI = GetGameObject(SubItemUI.UI_Town_Inventory).GetOrAddComponent<UI_TownInventory>();
        HeroesUI = GetGameObject(SubItemUI.UI_Town_Heroes).GetOrAddComponent<UI_HeroList>();
        StoreUI = GetGameObject(SubItemUI.UI_Town_Store).GetOrAddComponent<UI_TownStore>();
        QuestBoardUI = GetGameObject(SubItemUI.UI_Town_Quest).GetOrAddComponent<UI_QuestBoard>();
        HeroProfileGroupUI = GetGameObject(SubItemUI.UI_HeroProfileGroup_Horizontal).GetOrAddComponent<UI_HeroProfileGroup>();
    }
}
