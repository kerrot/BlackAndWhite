using UnityEngine;
using System.Collections;

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
