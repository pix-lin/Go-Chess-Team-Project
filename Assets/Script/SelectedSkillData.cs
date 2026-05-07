using UnityEngine;

public static class SelectedSkillData
{
    public static Sprite SkillSprite { get; private set; }
    public static string SkillName { get; private set; }
    public static string SkillDescription { get; private set; }
    public static bool HasSelection => SkillSprite != null && !string.IsNullOrEmpty(SkillName);

    public static void Set(Sprite skillSprite, string skillName, string skillDescription)
    {
        SkillSprite = skillSprite;
        SkillName = skillName;
        SkillDescription = skillDescription;
    }
}
