using UnityEngine;
using System.Collections;

//player skill
public class MagentaSkill : Skill {

    public override bool UseSkill()
    {
        skillObject.SetActive(true);
        return true;
    }

    public override bool CanSkill()
    {
        return !skillObject.activeSelf && CheckEnergy();
    }
}
