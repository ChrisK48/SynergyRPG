using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyUIParent : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Slider HpSlider;
    public Slider MpSlider;
    private PlayerCharBattle targetPC;

    public virtual void SetUpUI(PlayerCharBattle pc)
    {
        nameText.text = pc.charName;
        HpSlider.maxValue = pc.maxHp;
        HpSlider.value = pc.Hp;
        MpSlider.maxValue = pc.maxMp;
        MpSlider.value = pc.Mp;

        this.targetPC = pc;
        pc.OnStatsChanged += UpdateVisuals;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        nameText.text = targetPC.charName;
        HpSlider.maxValue = targetPC.maxHp;
        HpSlider.value = targetPC.Hp;
        MpSlider.maxValue = targetPC.maxMp;
        MpSlider.value = targetPC.Mp;
    }
}
