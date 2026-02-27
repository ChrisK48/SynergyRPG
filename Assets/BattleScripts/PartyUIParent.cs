using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        nameText.text = pc.CharName;
        hpSlider.maxValue = pc.MaxHp;
        hpSlider.value = pc.getHp();
        hpText.text = $"{pc.getHp()}/{pc.MaxHp} HP";
        mpSlider.maxValue = pc.MaxMp;
        mpSlider.value = pc.getMp();
        mpText.text = $"{pc.getMp()}/{pc.MaxMp} MP";

        this.targetPC = pc;
        pc.OnStatsChanged += UpdateVisuals;
        UpdateVisuals();
    }
    
    protected virtual void UpdateVisuals()
    {
        nameText.text = targetPC.CharName;
        hpSlider.maxValue = targetPC.MaxHp;
        hpSlider.value = targetPC.getHp();
        hpText.text = $"{targetPC.getHp()}/{targetPC.MaxHp} HP";
        mpSlider.maxValue = targetPC.MaxMp;
        mpSlider.value = targetPC.getMp();
        mpText.text = $"{targetPC.getMp()}/{targetPC.MaxMp} MP";
    }
}
