using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MonsterProfile : UI_CreatureProfile
{
    public override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// Creature를 UI에 바인딩
    /// </summary>
    public virtual void BindMonster(Monster monster)
    {
        _bindingCreature = monster;
        monster.BindProfileUI(this);

        var stat = monster.CreatureStat;
        stat.OnStatChanged -= UpdateStatProfile;
        stat.OnStatChanged += UpdateStatProfile;

        GetText(Texts.Text_Name).text = monster.CreatureData.Name;
        Get<Image>(Images.Creature_Image).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.CREATURE_IMAGE_PATH_PREFIX + $"{monster.CreatureData.Name}_Front");

        // init
        UpdateStatProfile(stat);
    }

}
