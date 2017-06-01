using UnityEngine;
using System.Collections;

public class EnergyBase : MonoBehaviour {

    public ElementType Type;

    protected EnergyBase gatherTarget;
    public EnergyBase GatherTarget { get { return gatherTarget; } }
}
