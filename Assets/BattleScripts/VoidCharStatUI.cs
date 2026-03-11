using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
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

    protected override IEnumerator AnimateBar()
    {
        float elapsed = 0f;
        float startHp = hpSlider.value;
        float startMp = mpSlider.value;
        float startVoid = voidSlider.value; // Get starting void

        VoidCharBattle voidChar = targetPC as VoidCharBattle;

        while (elapsed < 1f) // using your fillSpeed variable here
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1f;

            int curHp = (int)Mathf.Lerp(startHp, targetPC.getHp(), t);
            int curMp = (int)Mathf.Lerp(startMp, targetPC.getMp(), t);
            
            // Update the visuals
            UpdateVisuals(curHp, curMp);
            
            // Update void manually if it's a void char
            if (voidChar != null)
            {
                int curVoid = (int)Mathf.Lerp(startVoid, voidChar.voidPoints, t);
                voidSlider.value = curVoid;
                voidText.text = $"{curVoid}/{voidChar.maxVoidPoints} Void";
            }

            yield return null;
        }
        
        // Final Snap
        base.UpdateVisuals(targetPC.getHp(), targetPC.getMp());
    }

    protected override void UpdateVisuals(int hpValue, int mpValue)
    {
        base.UpdateVisuals(hpValue, mpValue);
        if (targetPC is VoidCharBattle voidChar)
        {
            voidSlider.maxValue = voidChar.maxVoidPoints;
            voidSlider.value = voidChar.voidPoints;
            voidText.text = $"{voidChar.voidPoints}/{voidChar.maxVoidPoints} Void";
        }
    }
}
