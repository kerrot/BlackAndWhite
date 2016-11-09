using UnityEngine;
using System.Collections;

public class UnitBattle : MonoBehaviour {

    public virtual bool Attacked(UnitBattle unit, Attack attack)
    {
        return false;
    }
}
