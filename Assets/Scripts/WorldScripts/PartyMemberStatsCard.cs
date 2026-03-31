using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PartyMemberStatsCard : MonoBehaviour
{
    public TextMeshProUGUI CharName;
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI MpText;
    public TextMeshProUGUI AtkText;
    public TextMeshProUGUI MagText;
    public TextMeshProUGUI DefText;
    public TextMeshProUGUI MdefText;
    public TextMeshProUGUI SpdText;
    public TextMeshProUGUI AccText;
    public TextMeshProUGUI EvaText;
    public TextMeshProUGUI LuckText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI ExpText;
    public void Initialize(PlayerCharData member)
    {
        CharName.text = member.name;
        HpText.text = $"HP: {member.currentHp}/{member.MaxHp}";
        MpText.text = $"MP: {member.currentMp}/{member.MaxMp}";
        AtkText.text = $"ATK: {member.Atk}";
        MagText.text = $"MAG: {member.Mag}";
        DefText.text = $"DEF: {member.Def}";
        MdefText.text = $"MDEF: {member.Mdef}";
        SpdText.text = $"SPD: {member.Spd}";
        AccText.text = $"ACC: {member.Acc}";
        EvaText.text = $"EVA: {member.Eva}";
        LuckText.text = $"LUCK: {member.Luck}";
        ExpText.text = $"EXP: {member.currentExp}/{member.GetExpToNextLevel(member.currentLevel)}";
        LevelText.text = $"LVL: {member.currentLevel}";
    }
}
