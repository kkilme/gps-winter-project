public class SampleBody1 : Armor
{
    public override void SetInfo(int templateId)
    {
        ArmorType = Define.ArmorType.Body;
        
        base.SetInfo(templateId);
    }
}