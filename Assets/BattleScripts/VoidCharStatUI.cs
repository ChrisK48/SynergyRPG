using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class VoidCharUI : PartyUIParent
{
    public Slider voidSlider;
    public TextMeshProUGUI voidText;

    public override void SetUpUI(PlayerCharBattle pc)
    {
        base.SetUpUI(pc);
        if (pc is VoidCharBattle voidChar)
        {
            voidSlider.maxValue = voidChar.maxVoidPoints;
            voidSlider.value = voidChar.voidPoints;
            voidText.text = $"{voidChar.voidPoints}/{voidChar.maxVoidPoints} Void";
        }
    }

    protected override void UpdateVisuals()
    {
        base.UpdateVisuals();
        if (targetPC is VoidCharBattle voidChar)
        {
            voidSlider.maxValue = voidChar.maxVoidPoints;
            voidSlider.value = voidChar.voidPoints;
            voidText.text = $"{voidChar.voidPoints}/{voidChar.maxVoidPoints} Void";
        }
    }
}
