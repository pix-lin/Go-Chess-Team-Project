using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public Image skillImage;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillText;

    private Image buttonSprite;

    public SkillList skillList;

    string skillName;
    string description;



    void Start()
    {
        buttonSprite = gameObject.GetComponent<Image>();
    }

    


    public void SkillSelect()
    {
        if (!skillList.TryGetSkill(gameObject.name, out SkillList.SkillInfo skillInfo))
        {
            return;
        }


        skillImage.sprite = buttonSprite.sprite;
        skillNameText.text = skillInfo.skillName;
        skillText.text = skillInfo.description;

        SelectedSkillData.Set(buttonSprite.sprite, skillInfo.skillName, skillInfo.description);
    }
}
