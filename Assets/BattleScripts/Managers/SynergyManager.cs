using UnityEngine;

public class SynergyManager : MonoBehaviour
{
    public static SynergyManager instance;
    public float currentSynergy = 0f;
    public float maxSynergy = 100f; // Maybe this will not be static in the future?
    public System.Action<float, float> OnSynergyChanged;

    void Awake() => instance = this;

    public void GainSP(float amount)
    {
        currentSynergy = Mathf.Clamp(currentSynergy + amount, 0, maxSynergy);
        Debug.Log("Gained " + amount + " SP. Current SP: " + currentSynergy);
        OnSynergyChanged?.Invoke(currentSynergy, maxSynergy);
    }

    public void ConsumeSP(float amount)
    {
        currentSynergy = Mathf.Clamp(currentSynergy - amount, 0, maxSynergy);
        Debug.Log("Consumed " + amount + " SP. Current SP: " + currentSynergy);
        OnSynergyChanged?.Invoke(currentSynergy, maxSynergy);
    }
}
