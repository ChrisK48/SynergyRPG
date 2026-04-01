using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public Image PortraitDisplay;
    public float ImageScale;
    public void Initialize(PlayerCharData member)
    {
        nameText.text = member.name;
        levelText.text = $"Level: {member.currentLevel}";
        expText.text = $"EXP: {member.currentExp}/{member.GetExpToNextLevel(member.currentLevel + 1)}";
        hpText.text = $"HP: {member.currentHp}/{member.MaxHp}";
        mpText.text = $"MP: {member.currentMp}/{member.MaxMp}";
        
        PortraitDisplay.sprite = member.MenuImage;
        PortraitDisplay.SetNativeSize();
        PortraitDisplay.rectTransform.localScale = new Vector3(ImageScale, ImageScale, 1);

        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => {
            MenuManager.instance.BuildSkillBoardMenu(member.CharacterBoard);
        });
    }
}
