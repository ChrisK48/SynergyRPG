using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowUI : MonoBehaviour
{
    private Slider flowSlider;
    private TextMeshProUGUI flowText;

     void Start()
    {
        flowSlider = GetComponentInChildren<Slider>();
        flowText = GetComponentInChildren<TextMeshProUGUI>();
        FlowManager.instance.OnFlowChanged += UpdateVisuals;
        setupUI();
    }

    public void setupUI()
    {
        flowSlider.maxValue = FlowManager.instance.maxFlow;
        flowSlider.value = FlowManager.instance.currentFlow;
        flowText.text = $"{FlowManager.instance.currentFlow}/{FlowManager.instance.maxFlow} Flow";
    }

    void UpdateVisuals(float currentFlow, float maxFlow)
    {
        flowSlider.value = currentFlow;
        flowSlider.maxValue = maxFlow;
        flowText.text = $"{currentFlow}/{maxFlow} Flow";
    }
}
