using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static SkillList;

public class TournamentUI : MonoBehaviour
{
    public Image playerImage;
    public TextMeshProUGUI playerSkillNameText;
    public Image enemy1Image;
    public TextMeshProUGUI enemy1SkillNameText;
    public Image enemy2Image;
    public TextMeshProUGUI enemy2SkillNameText;
    public Image enemy3Image;
    public TextMeshProUGUI enemy3SkillNameText;

    public SkillList skillList;

    string[] skillNames = { "TetrisBlock", "Baduk", "NeutralBlock" };



    void Start()
    {
        //시작할 때 플레이어가 선택한 스킬 표시
        if (!SelectedSkillData.HasSelection)
        {
            return;
        }

        playerImage.sprite = SelectedSkillData.SkillSprite;
        playerSkillNameText.text = SelectedSkillData.SkillName;


        //랜덤으로 3개의 스킬을 선택하여 적으로 표시
        //enemy 1
        int rand = Random.Range(0, skillNames.Length);
        skillList.TryGetSkill(skillNames[rand], out SkillList.SkillInfo skillInfo);

        enemy1Image.sprite = skillInfo.sprite;
        enemy1SkillNameText.text = skillInfo.skillName;

        //enemy 2
        int rand2 = Random.Range(0, skillNames.Length);
        skillList.TryGetSkill(skillNames[rand2], out SkillList.SkillInfo skillInfo2);

        enemy2Image.sprite = skillInfo2.sprite;
        enemy2SkillNameText.text = skillInfo2.skillName;

        //enemy 3
        int rand3 = Random.Range(0, skillNames.Length);
        skillList.TryGetSkill(skillNames[rand3], out SkillList.SkillInfo skillInfo3);

        enemy3Image.sprite = skillInfo3.sprite;
        enemy3SkillNameText.text = skillInfo3.skillName;





    }

}
