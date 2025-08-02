using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Area에서 지금까지 획득한 전리품을 디스플레이하는 팝업 UI
/// </summary>
public class UI_AreaLootListPopup : UI_Popup
{
    enum GameObjects
    {
        LootInventory,
        Indicator_NoLoots
    }

    enum Buttons
    {
        Button_Close
    }

    enum Texts
    {
        Text_Gold
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        GetButton(Buttons.Button_Close).onClick.AddListener(Close);

        UI_Inventory inventory = GetGameObject(GameObjects.LootInventory).GetOrAddComponent<UI_Inventory>();
        inventory.HardClear();
        inventory.LateInit(maxSize: GlobalValues.MAX_AREAITEM_COUNT);

        List<ItemData> lootItems = Managers.AreaMng.Loots.Items;
        GetGameObject(GameObjects.Indicator_NoLoots).SetActive(lootItems.Count == 0); // 전리품이 없을 때의 표시

        foreach (var item in lootItems)
        {
            inventory.AddItem(item);
        }

        GetText(Texts.Text_Gold).text = Managers.AreaMng.Loots.Gold.ToString("N0");
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetText(Texts.Text_Gold).GetComponentInParent<RectTransform>()); // Preferred Width에 맞추기 위해 Horizontal Layout 강제 업데이트
    }
}
