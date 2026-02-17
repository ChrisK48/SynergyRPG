using UnityEngine;
using UnityEngine.UI;

public class ShieldCharUI : PartyUIParent
{
    public Slider shieldSlider;
    public override void SetUpUI(PlayerCharBattle character)
    {
        base.SetUpUI(character);
        shieldSlider.value = 0;
        if (character is ShieldCharBattle shieldChar)
            shieldSlider.maxValue = shieldChar.maxShieldPoints;
    }
}
