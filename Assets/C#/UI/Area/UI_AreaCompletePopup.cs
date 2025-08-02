using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Area 완료 시 나오는 팝업
/// </summary>
public class UI_AreaCompletePopup : UI_Popup
{
    enum GameObjects
    {
        Inventory_Loots,
        Inventory_QuestRewards,
        Indicator_NoLoots,
        Complete,
    }

    enum Buttons
    {
        Button_ReturnToTown
    }

    enum Texts
    {
        Text_LootInventoryTitle,
        Text_QuestRewardInventoryTitle,
        Text_Gold
    }

    public override void Init() { }

    private void LateInit(Loot loot, Quest quest)
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        UI_Inventory lootInventory = GetGameObject(GameObjects.Inventory_Loots).GetOrAddComponent<UI_Inventory>();
        lootInventory.HardClear();
        lootInventory.LateInit();

        List<ItemData> lootItems = loot.Items;
        GetGameObject(GameObjects.Indicator_NoLoots).SetActive(lootItems.Count == 0); // 전리품이 없을 때의 표시

        foreach (var item in lootItems)
        {
            lootInventory.AddItem(item);
        }

        GetText(Texts.Text_Gold).text = Managers.AreaMng.Loots.Gold.ToString("N0");
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetText(Texts.Text_Gold).GetComponentInParent<RectTransform>()); // Preferred Width에 맞추기 위해 Horizontal Layout 강제 업데이트

        UI_Inventory rewardInventory = GetGameObject(GameObjects.Inventory_QuestRewards).GetOrAddComponent<UI_Inventory>();
        rewardInventory.HardClear();
        rewardInventory.LateInit();

        if (!quest.QuestData.IsComplete)
        {
            // 첫 퀘스트 클리어 보상 추가
            foreach (QuestReward reward in quest.QuestData.FirstClearRewards)
            {
                for (int i = 0; i < reward.Quantity; i++)
                {
                    ItemData itemData = Managers.DataMng.ItemDataDict[reward.ItemDataId];
                    rewardInventory.AddItem(itemData);
                }
            }
        }
        else
        {
            rewardInventory.gameObject.SetActive(false);
        }

        GetButton(Buttons.Button_ReturnToTown).onClick.AddListener(() => CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.LoadTownScene()));
    }

    public void Show(Loot loot, Quest quest)
    {
        Panel.gameObject.SetActive(false);
        LateInit(loot, quest);
        GameObject completeText = GetGameObject(GameObjects.Complete);
        RectTransform completeRect = completeText.GetComponent<RectTransform>();

        Sequence seq = DOTween.Sequence();

        seq.Append(completeRect.DOScale(new Vector3(.2f, .2f, 1f), .5f).From().SetEase(Ease.InQuad));
        seq.AppendInterval(1f);
        seq.Append(completeRect.DOScale(new Vector3(1f, 1f, 1f), 1f));

        Vector3 temp = completeRect.transform.position;
        completeRect.anchorMax = new Vector2(0.5f, 1);
        completeRect.anchorMin = new Vector2(0.5f, 1);
        completeRect.transform.position = temp;

        seq.Join(completeRect.DOAnchorPos(new Vector2(0f, -80f), 1f).OnComplete(() => { Panel.gameObject.SetActive(true); }));

        RectTransform panelRect = Panel.gameObject.GetComponent<RectTransform>();
        seq.Append(panelRect.DOScale(new Vector3(.2f, .2f, 1f), .5f).From().SetEase(Ease.InQuad));

        seq.Play();
    }
}
