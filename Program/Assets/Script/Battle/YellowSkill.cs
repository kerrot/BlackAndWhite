using UnityEngine;
using System.Collections;

public class YellowSkill : Skill {

    public override bool Activated()
    {
        return GameObject.FindObjectOfType<YellowDebuff>();
    }

    public override void SkillEnd()
    {
        YellowDebuff debuff = GameObject.FindObjectOfType<YellowDebuff>();
        if (debuff)
        {
            debuff.End();
        }
    }
}
