using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyUIParent : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider mpSlider;
    public TextMeshProUGUI mpText;
    protected PlayerCharBattle targetPC;

    public virtual void SetUpUI(PlayerCharBattle pc)
    {
        nameText.text = pc.charName;
        hpSlider.maxValue = pc.maxHp;
        hpSlider.value = pc.Hp;
        hpText.text = $"{pc.Hp}/{pc.maxHp} HP";
        mpSlider.maxValue = pc.maxMp;
        mpSlider.value = pc.Mp;
        mpText.text = $"{pc.Mp}/{pc.maxMp} MP";

        this.targetPC = pc;
        pc.OnStatsChanged += UpdateVisuals;
        UpdateVisuals();
    }
    
    protected virtual void UpdateVisuals()
    {
        nameText.text = targetPC.charName;
        hpSlider.maxValue = targetPC.maxHp;
        hpSlider.value = targetPC.Hp;
        hpText.text = $"{targetPC.Hp}/{targetPC.maxHp} HP";
        mpSlider.maxValue = targetPC.maxMp;
        mpSlider.value = targetPC.Mp;
        mpText.text = $"{targetPC.Mp}/{targetPC.maxMp} MP";
    }
}
