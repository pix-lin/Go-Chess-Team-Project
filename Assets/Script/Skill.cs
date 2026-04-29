using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public Image skillImage;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillText;

    private Image buttonSprite;

    string skillName;
    string description;



    void Start()
    {
        buttonSprite = gameObject.GetComponent<Image>();

    }

    


    public void SkillSelect()
    {
        switch (gameObject.name)
        {
            case "TetrisBlock":
                skillName = "TetrisBlock";
                description = "Generate tetris block that contains two black and two white";
                break;

            case "Baduk":
                skillName = "Baduk";
                description = "If you trap the other person's stone, you destroy it";
                break;

            case "NeutralBlock":
                skillName = "NeutralBlock";
                description = "Next turn, turn your opponent's stone into a neutral stone";
                break;
        }


        skillImage.sprite = buttonSprite.sprite;
        skillNameText.text = skillName;
        skillText.text = description;
    }
}
