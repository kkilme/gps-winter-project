using UnityEngine;


public class HealPotion : ConsumableItem, IUsableInArea, IUsableInBattle
{
    public HealPotion(int dataId) : base(dataId) { }

    public ItemAction GetItemAction()
    {
        return new HealPotionAction(this);
    }

    public void UseInArea()
    {
        void Use(object hero)
        {
            Hero _hero = hero as Hero;
            if (_hero == null)
            {
                Debug.LogError("[HealPotion] UseInArea: Hero is null or not a valid Hero instance.");
                return;
            }
            _hero.TakeHeal(ratio: .2f);
            Managers.AreaMng.Items.Remove(this);
        }
        AreaSingleHeroTargetSelector targetSelector = new AreaSingleHeroTargetSelector();
        targetSelector.StartTargetSelection(Use);
    }
}