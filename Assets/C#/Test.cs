using UnityEngine;

// TODO - TEST CODE
public class Test : MonoBehaviour
{
    private void Start()
    {
        Managers.InputMng.KeyAction -= OnKeyboardClick;
        Managers.InputMng.KeyAction += OnKeyboardClick;
    }

    private void OnKeyboardClick()
    {
        // Hero hero = Managers.ObjectMng.Heroes[Managers.ObjectMng.NextHeroId - 1];
        Hero hero = Managers.BattleMng.CurrentTurnCreature as Hero;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BaseItem item = hero.Bag.StoreItem(Define.ITEM_HEALPOTION_ID);
            Debug.Log($"{item.ItemData.Name}: {item.Count}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // int itemIdx = 0;
            // Bag.UseItem(itemIdx, this.Id);
            // if (Bag.Items[itemIdx] != null)
            //     Debug.Log($"{Bag.Items[itemIdx].ItemData.Name}: {Bag.Items[itemIdx].Count}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Weapon weapon = new SampleSingleSword();
            weapon.SetInfo(Define.WEAPON_SAMPLESINGLESWORD_ID);
            hero.EquipWeapon(weapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Armor body = new SampleBody1();
            body.SetInfo(Define.ARMOR_SAMPLEBODY1_ID);
            hero.EquipArmor(body);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Armor body = new SampleBody2();
            body.SetInfo(Define.ARMOR_SAMPLEBODY2_ID);
            hero.EquipArmor(body);
        }
    }
}