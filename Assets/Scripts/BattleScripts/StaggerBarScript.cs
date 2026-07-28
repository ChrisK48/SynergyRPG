using UnityEngine;
using UnityEngine.UI;

public class StaggerBar : MonoBehaviour
{
    public Slider staggerSlider;
    private float maxStaggerValue;
    private float staggerValue;
    private NpcBattle enemy;
    public void Setup(NpcBattle enemy)
    {
        this.enemy = enemy;
        maxStaggerValue = enemy.GetMaxStagger();
        staggerValue = enemy.GetCurrentStagger();
        staggerSlider.maxValue = maxStaggerValue;
        staggerSlider.value = staggerValue;
        Debug.Log($"Generating stagger bar for {enemy.EntityName} with current stagger {staggerValue} and max stagger {maxStaggerValue}");
    }
}
