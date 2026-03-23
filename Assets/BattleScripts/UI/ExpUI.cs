using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.Mathematics;
public class ExpUI : MonoBehaviour
{
    public TextMeshProUGUI CharNameText;
    public TextMeshProUGUI ExpText;
    public Slider ExpSlider;
    private PlayerCharData attachedChar;
    private float fillSpeed = 1f;

    public void SetupUI(PlayerCharData attachedChar, int oldExp, int xpGained)
    {
        this.attachedChar = attachedChar;
        CharNameText.text = attachedChar.CharName;
        StartCoroutine(AnimateBar(oldExp, attachedChar.currentExp));
    }

    private IEnumerator AnimateBar(int startXP, int endXP)
    {
        float elapsed = 0f;
        
        while (elapsed < fillSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fillSpeed;
            
            int currentDisplayXP = (int)Mathf.Lerp(startXP, endXP, t);
            UpdateVisuals(currentDisplayXP);
            
            yield return null;
        }

        UpdateVisuals(endXP);
    }

    private void UpdateVisuals(int xpValue)
    {
        int displayLevel = attachedChar.GetLevelForXP(xpValue);
        
        int xpFloor = attachedChar.GetExpToNextLevel(displayLevel);
        
        int xpTarget = attachedChar.GetExpToNextLevel(displayLevel + 1);

        float currentProgressInRange = xpValue - xpFloor;
        float totalRangeNeeded = xpTarget - xpFloor;

        CharNameText.text = attachedChar.CharName + " (Lv. " + displayLevel + ")";
        ExpText.text = $"{(int)currentProgressInRange} / {(int)totalRangeNeeded} XP";

        float sliderValue = currentProgressInRange / totalRangeNeeded;
        ExpSlider.value = Mathf.Clamp01(sliderValue);
    }
}
