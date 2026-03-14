using System;
using TMPro;
using UnityEngine;

public class PartyMemberCard : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public void Initialize(PlayerCharData member)
    {
        nameText.text = member.name;
        levelText.text = $"Level: {member.currentLevel}";
        expText.text = $"EXP: {member.currentExp}/{member.GetExpToNextLevel(member.currentLevel)}";
        hpText.text = $"HP: {member.currentHp}/{member.MaxHp}";
        mpText.text = $"MP: {member.currentMp}/{member.MaxMp}";
    }
}
