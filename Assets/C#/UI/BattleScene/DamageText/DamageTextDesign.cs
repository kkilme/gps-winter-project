using TMPro;
using UnityEngine;

public abstract class DamageTextDesign
{
    protected static TMP_ColorGradient _colorGradient; // 텍스트의 ColorGradient. static으로 캐싱하여 한 번 로드 후 재사용
    protected abstract string GetText(int amount); // 실제 작성될 텍스트
    protected abstract Color GetTextColor(); // 텍스트의 색상

    public virtual void ApplyDesign(int amount, UI_DamageText damageText)
    {
        damageText.Text.text = GetText(amount);
        damageText.Text.color = GetTextColor();
        damageText.Text.colorGradientPreset = _colorGradient;
    }

    // public void PlayAnimation(UI_DamageText damageText); // DamageText별로 다른 애니메이션 효과 재생하게 할 수도 있을듯
}

public class PhysicalDamageTextDesign : DamageTextDesign
{
    public PhysicalDamageTextDesign()
    {
        if( _colorGradient == null )
            _colorGradient = Managers.ResourceMng.Load<TMP_ColorGradient>("Textures/TMP_ColorGradients/ColorGradient_PhysicalDamage");
    }

    protected override string GetText(int amount) => amount == 0 ? "Blocked" : $"-{amount}";

    protected override Color GetTextColor() => GlobalValues.PHYSICAL_UI_ELEMENT_BASE_COLOR;
}

public class MagicDamageTextDesign : DamageTextDesign
{
    public MagicDamageTextDesign()
    {
        if (_colorGradient == null)
            _colorGradient = Managers.ResourceMng.Load<TMP_ColorGradient>("Textures/TMP_ColorGradients/ColorGradient_MagicDamage");
    }

    protected override string GetText(int amount) => amount == 0 ? "Blocked" : $"-{amount}";

    protected override Color GetTextColor() => GlobalValues.MAGIC_UI_ELEMENT_BASE_COLOR;
}

public class HealTextDesign : DamageTextDesign
{
    public HealTextDesign()
    {
        if (_colorGradient == null)
            _colorGradient = Managers.ResourceMng.Load<TMP_ColorGradient>("Textures/TMP_ColorGradients/ColorGradient_Heal");
    }

    protected override string GetText(int amount) => $"+{amount}";

    protected override Color GetTextColor() => GlobalValues.HEAL_ELEMENT_BASE_COLOR;
}