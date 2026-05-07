using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillList", menuName = "Game/Skill List")]
public class SkillList : ScriptableObject
{
    [Serializable]
    public class SkillInfo
    {
        public string key;
        public string skillName;
        public string description;
        public Sprite sprite;
    }

    public SkillInfo[] skills;

    public bool TryGetSkill(string key, out SkillInfo skillInfo)
    {
        foreach (SkillInfo skill in skills)
        {
            if (skill.key == key)
            {
                skillInfo = skill;
                return true;
            }
        }

        skillInfo = null;
        return false;
    }
}
