using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PartyUIParent : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider mpSlider;
    public TextMeshProUGUI mpText;
    protected PlayerCharBattle targetPC;
    private float fillSpeed = 0.5f;

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
        pc.OnStatsChanged += HandleStatsChanged;
        AnimateBar();
    }

    private void HandleStatsChanged()
    {
        StopAllCoroutines();
        if (gameObject.activeInHierarchy)StartCoroutine(AnimateBar());
    }

    protected virtual IEnumerator AnimateBar()
    {
        float elapsed = 0f;

        int startHp = (int)hpSlider.value;
        int startMp = (int)mpSlider.value;
        
        while (elapsed < fillSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fillSpeed;
            
            // Interpolate the XP number for the text and slider
            int currentDisplayHp = (int)Mathf.Lerp(startHp, targetPC.getHp(), t);
            int currentDisplayMp = (int)Mathf.Lerp(startMp, targetPC.getMp(), t);
            UpdateVisuals(currentDisplayHp, currentDisplayMp);
            
            yield return null;
        }

        // Final snap to ensure it ends exactly on the right number
        UpdateVisuals(targetPC.getHp(), targetPC.getMp());
    }
    
    protected virtual void UpdateVisuals(int hpValue, int mpValue)
    {
        nameText.text = targetPC.CharName;
        hpSlider.maxValue = targetPC.MaxHp;
        hpSlider.value = hpValue;
        hpText.text = $"{hpValue}/{targetPC.MaxHp} HP";
        mpSlider.maxValue = targetPC.MaxMp;
        mpSlider.value = mpValue;
        mpText.text = $"{mpValue}/{targetPC.MaxMp} MP";
        if (hpValue <= 0)
        {
            hpText.text = "DEAD";
        }
    }

    private void OnDestroy()
    {
        if (targetPC != null) targetPC.OnStatsChanged -= HandleStatsChanged;
    }
}
