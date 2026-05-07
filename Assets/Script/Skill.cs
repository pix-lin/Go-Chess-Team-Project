using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SkillList;

public class Skill : MonoBehaviour
{
    public Image skillImage;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillText;

    public SkillList skillList;

    string skillName;
    string description;



    void Start()
    {
        skillList.TryGetSkill("TetrisBlock", out SkillList.SkillInfo basicSkillInfo);

        skillImage.sprite = basicSkillInfo.sprite;
        skillNameText.text = basicSkillInfo.skillName;
        skillText.text = basicSkillInfo.description;

        SelectedSkillData.Set(basicSkillInfo.sprite, basicSkillInfo.skillName, basicSkillInfo.description);
    }

    


    public void SkillSelect()
    {
        if (!skillList.TryGetSkill(gameObject.name, out SkillList.SkillInfo skillInfo))
        {
            return;
        }


        skillImage.sprite = skillInfo.sprite;
        skillNameText.text = skillInfo.skillName;
        skillText.text = skillInfo.description;

        SelectedSkillData.Set(skillInfo.sprite, skillInfo.skillName, skillInfo.description);
    }
}
