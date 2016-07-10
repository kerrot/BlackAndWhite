using UnityEngine;
using System.Collections;

public class CameraEffect : SingletonMonoBehaviour<CameraEffect>
{
    [SerializeField]
    private MonoBehaviour FishEye;
    [SerializeField]
    private MonoBehaviour Vignette;

    public void WhiteSkillEffect(bool active)
    {
        FishEye.enabled = active;
        Vignette.enabled = active;
    }
}
