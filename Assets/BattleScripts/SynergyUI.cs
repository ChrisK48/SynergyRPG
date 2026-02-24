using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynergyUI : MonoBehaviour
{
    private Slider synergySlider;
    private TextMeshProUGUI synergyText;

     void Start()
    {
        synergySlider = GetComponentInChildren<Slider>();
        synergyText = GetComponentInChildren<TextMeshProUGUI>();
        SynergyManager.instance.OnSynergyChanged += UpdateVisuals;
        setupUI();
    }

    public void setupUI()
    {
        synergySlider.maxValue = SynergyManager.instance.maxSynergy;
        synergySlider.value = SynergyManager.instance.currentSynergy;
        synergyText.text = $"{SynergyManager.instance.currentSynergy}/{SynergyManager.instance.maxSynergy} SP";
    }

    void UpdateVisuals(float currentSynergy, float maxSynergy)
    {
        synergySlider.value = currentSynergy;
        synergySlider.maxValue = maxSynergy;
        synergyText.text = $"{currentSynergy}/{maxSynergy} SP";
    }
}
