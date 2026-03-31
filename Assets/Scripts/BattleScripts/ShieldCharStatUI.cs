using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
            shieldText.text = $"{shieldChar.shieldPoints}/{shieldChar.maxShieldPoints} Shield";
        }
    }

    protected override IEnumerator AnimateBar()
    {
        float elapsed = 0f;
        float startHp = hpSlider.value;
        float startMp = mpSlider.value;
        float startShield = shieldSlider.value; // Get starting shield

        ShieldCharBattle shieldChar = targetPC as ShieldCharBattle;

        while (elapsed < 1f) // using your fillSpeed variable here
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1f;

            int curHp = (int)Mathf.Lerp(startHp, targetPC.getHp(), t);
            int curMp = (int)Mathf.Lerp(startMp, targetPC.getMp(), t);
            
            // Update the visuals
            UpdateVisuals(curHp, curMp);
            
            // Update shield manually if it's a shield char
            if (shieldChar != null)
            {
                int curShield = (int)Mathf.Lerp(startShield, shieldChar.shieldPoints, t);
                shieldSlider.value = curShield;
                shieldText.text = $"{curShield}/{shieldChar.maxShieldPoints} Shield";
            }

            yield return null;
        }
        
        // Final Snap
        base.UpdateVisuals(targetPC.getHp(), targetPC.getMp());
    }

    protected override void UpdateVisuals(int hpValue, int mpValue)
    {
        base.UpdateVisuals(hpValue, mpValue);
        if (targetPC is ShieldCharBattle shieldChar)
        {
            shieldSlider.maxValue = shieldChar.maxShieldPoints;
            shieldText.text = $"{shieldChar.shieldPoints}/{shieldChar.maxShieldPoints} Shield";
        }
    }
}
