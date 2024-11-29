using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyInfo : UI_Base
{
    enum GameObjects
    {
        HpBar
    }

    enum Texts
    {
        Name,
        HP
    }

    private Creature _creature;
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void BindCreature(Creature creature)
    {
        _creature = creature;
        GetText(Texts.Name).text = creature.CreatureData.Name;
    }

    private void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y);
        transform.rotation = Camera.main.transform.rotation;
        if (_creature != null)
        {
            SetHpRatio(_creature.CreatureStat.Hp / (float)_creature.CreatureStat.MaxHp);
            GetText(Texts.HP).text = _creature.CreatureStat.Hp.ToString();
        }
    }

    public void SetHpRatio(float ratio)
    {
        GetGameObject(GameObjects.HpBar).GetComponent<Slider>().value = ratio;
    }
}
