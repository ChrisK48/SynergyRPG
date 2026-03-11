using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
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
            
            // Interpolate the XP number for the text and slider
            int currentDisplayXP = (int)Mathf.Lerp(startXP, endXP, t);
            UpdateVisuals(currentDisplayXP);
            
            yield return null;
        }

        // Final snap to ensure it ends exactly on the right number
        UpdateVisuals(endXP);
    }

    private void UpdateVisuals(int xpValue)
    {
        // 1. Find what level this specific XP amount represents
        int displayLevel = attachedChar.GetLevelForXP(xpValue);
        
        // 2. The 'Floor' is the XP required to REACH the level you are currently in
        int xpFloor = attachedChar.GetExpToNextLevel(displayLevel);
        
        // 3. The 'Target' is the XP required to reach the NEXT level
        int xpTarget = attachedChar.GetExpToNextLevel(displayLevel + 1);

        // 4. Calculate progress only using the XP earned SINCE the last level up
        float currentProgressInRange = xpValue - xpFloor;
        float totalRangeNeeded = xpTarget - xpFloor;

        // Update Text
        CharNameText.text = attachedChar.CharName + " (Lv. " + displayLevel + ")";
        ExpText.text = $"{(int)currentProgressInRange} / {(int)totalRangeNeeded} XP";

        // Update Slider
        float sliderValue = currentProgressInRange / totalRangeNeeded;
        ExpSlider.value = Mathf.Clamp01(sliderValue);
    }
}
