using UnityEngine;
using System.Collections;

public class ButtonEffect : MonoBehaviour {
    [SerializeField]
    private float From;
    [SerializeField]
    private float To;
    [SerializeField]
    private float Speed;

    private float now;
    void Start()
    {
        now = To;
    }

    // Update is called once per frame
    void Update () {
        now += Speed;

        if (now < From)
        {
            now = From;
            Speed = -Speed;
        }
        else if (now > To)
        {
            now = To;
            Speed = -Speed;
        }

        transform.localScale = new Vector3(now, now, now);
	}
}
