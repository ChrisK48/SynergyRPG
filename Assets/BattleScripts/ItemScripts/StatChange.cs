using UnityEngine;
public enum StatType {HP, MP, SP}

[CreateAssetMenu(fileName = "New Stat Change Item Effect", menuName = "Items/Effects/Stat Change")]
public class StatChange : ItemEffect
{
    public StatType statType;
    public int changeAmount;
    public override void ApplyEffect(CharBattle user, CharBattle target)
    {
        switch(statType)
        {
            case StatType.HP:
                target.Heal(changeAmount);
                break;
            case StatType.MP:
                if(target is PlayerCharBattle pcTarget)
                    pcTarget.ChangeMp(changeAmount);
                    break;
            /*
            case StatType.SP:
                target.ChangeSp(changeAmount);
                break;
            */
        }
    } 
}
