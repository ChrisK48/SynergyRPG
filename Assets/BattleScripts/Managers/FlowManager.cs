using UnityEngine;

public class FlowManager : MonoBehaviour
{
    public static FlowManager instance;
    public float currentFlow = 0f;
    public float maxFlow = 100f; // Maybe this will not be static in the future?
    public System.Action<float, float> OnFlowChanged;

    void Awake() => instance = this;

    public void GainFlow(float amount)
    {
        currentFlow = Mathf.Clamp(currentFlow + amount, 0, maxFlow);
        Debug.Log("Gained " + amount + " SP. Current SP: " + currentFlow);
        OnFlowChanged?.Invoke(currentFlow, maxFlow);
    }

    public void ConsumeFlow(float amount)
    {
        currentFlow = Mathf.Clamp(currentFlow - amount, 0, maxFlow);
        Debug.Log("Consumed " + amount + " SP. Current SP: " + currentFlow);
        OnFlowChanged?.Invoke(currentFlow, maxFlow);
    }
}
