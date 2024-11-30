using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_TurnState : UI_Base
{
    [SerializeField] private float _duration = 0.5f;
    private Image[] _iconImages;
    private int _activatedCount;
 
    enum Objects
    {
        IconGroup,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Objects));
        _iconImages = GetGameObject(Objects.IconGroup).GetComponentsInChildren<Image>();
        ActivateCreatureUisByCount(0);
    }
    
    // 전체 크리처 턴만 띄우고 다 하면 다시 재구성 리스트형태로 매개변수 받아오기
    public void ChangeTurnStateUI()
    {
        // 맨 앞에 있는 아이콘 뒤로 이동
        Image firstImage = _iconImages[0];
        firstImage.gameObject.SetActive(false);

        // 앞으로 한칸식 앞당기기
        for (int i = 1; i < _activatedCount; i++)
            _iconImages[i - 1] = _iconImages[i];

        _iconImages[_activatedCount - 1] = firstImage;

        // 첫번째 이미지 크기 늘려주기
        _iconImages[0].rectTransform.DOSizeDelta(firstImage.rectTransform.sizeDelta, _duration)
            .OnComplete(() => { firstImage.transform.SetAsLastSibling(); firstImage.gameObject.SetActive(true); });
        // 마지막 이미지 크기 되돌리기
        firstImage.rectTransform.sizeDelta = _iconImages[1].rectTransform.sizeDelta;
    }

    // TODO - Hero, Monster Image 만들어지면 바꿔주기
    // TODO - UI 활성화 방법 마련되면 자동으로 호출하게 바꿔주기
    public void InitTurnStateUI()
    {
        int creatureCount = Managers.BattleMng.TurnSystem.CreatureCount;
        ActivateCreatureUisByCount(creatureCount);
        
        for (int i = 0; i < creatureCount; i++)
        {
            ulong id = Managers.BattleMng.TurnSystem.Turns[i];
            var creature = Managers.ObjectMng.GetCreatureWithId(id);
            var icon = _iconImages[i];
            switch (creature.CreatureType)
            {
                case Define.CreatureType.Monster:
                    icon.color = Color.red;
                    break;
                case Define.CreatureType.Hero:
                    icon.color = Color.green;
                    break;
            }
        }
    }

    public void ActivateCreatureUisByCount(int count)
    {
        foreach (var icon in _iconImages)
            icon.gameObject.SetActive(false);

        for (int i = 0; i < count; i++)
        {
            _iconImages[i].gameObject.SetActive(true);
        }

        _activatedCount = count;
    }

    // TODO - FOR TEST
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangeTurnStateUI();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            InitTurnStateUI();
        }
    }
}
