using System.Collections;
using UnityEngine;


public class HealPotion : ConsumableItem, IUsableInArea, IUsableInBattle
{
    public HealPotion(int dataId) : base(dataId) { }

    public ItemAction GetItemAction()
    {
        return new HealPotionAction();
    }

    public void UseInArea()
    {
        void Use(object hero)
        {
            Hero _hero = hero as Hero;
            if(_hero == null)
            {
                Debug.LogError("[HealPotion] UseInArea: Hero is null or not a valid Hero instance.");
                return;
            }
            _hero.TakeHeal(percent: .2f);
            Managers.AreaMng.Items.Remove(this);
        }
        AreaHeroTargetSelector targetSelector = new AreaHeroTargetSelector();
        targetSelector.StartTargetSelection(Use);
    }
}