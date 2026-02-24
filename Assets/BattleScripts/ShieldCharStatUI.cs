using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShieldCharUI : PartyUIParent
{
    public Slider shieldSlider;
    public TextMeshProUGUI shieldText;
    public override void SetUpUI(PlayerCharBattle pc)
    {
        base.SetUpUI(pc);
        if (pc is ShieldCharBattle shieldChar)
        {
            shieldSlider.maxValue = shieldChar.maxShieldPoints;
            shieldSlider.value = shieldChar.shieldPoints;
            shieldSlider.maxValue = shieldChar.maxShieldPoints;
            shieldSlider.value = shieldChar.shieldPoints;
            shieldText.text = $"{shieldChar.shieldPoints}/{shieldChar.maxShieldPoints} Shield";
        }
    }

    protected override void UpdateVisuals()
    {
        base.UpdateVisuals();
        if (targetPC is ShieldCharBattle shieldChar)
        {
            shieldSlider.maxValue = shieldChar.maxShieldPoints;
            shieldSlider.value = shieldChar.shieldPoints;
            shieldText.text = $"{shieldChar.shieldPoints}/{shieldChar.maxShieldPoints} Shield";
        }
    }
}
