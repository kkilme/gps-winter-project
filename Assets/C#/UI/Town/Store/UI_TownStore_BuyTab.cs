using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// TownStore 구매 패널에서 하나의 탭을 나타냄.
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class UI_TownStore_BuyTab : UI_Base
{
    public override void Init(){}

    public void ShowInstantly(ItemType itemType)
    {
        base.ShowInstantly();
        Clear();

        // 상점 물품 채워넣기
        foreach (var entry in Managers.DataMng.StoreDataDict[itemType].StoreEntries)
        {
            UI_StoreEntry storeEntry = Managers.UIMng.MakeSubItemUI<UI_StoreEntry>(transform, "Town/" + nameof(UI_StoreEntry));
            storeEntry.LateInit(entry);
        }
    }

    private void Clear()
    {
        for (int i = gameObject.transform.childCount-1; i>=0; i--)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }
    }
}
