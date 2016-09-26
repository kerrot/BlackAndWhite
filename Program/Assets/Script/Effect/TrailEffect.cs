using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Collections;

public class TrailEffect : MonoBehaviour {
    [SerializeField]
    private GameObject refPos;
    [SerializeField]
    private GameObject trailObject;

    private GameObject current;

    public void EffectStart()
    {
        if (current == null)
        {
            current = Instantiate(trailObject) as GameObject;
            current.transform.parent = refPos.transform;
            current.transform.localPosition = Vector3.zero;
        }
    }

    public void EffectEnd()
    {
        if (current != null)
        {
            current.transform.parent = null;
            StartCoroutine(EndTrail(current));
            current = null;
        }
    }

    IEnumerator EndTrail(GameObject obj)
    {
        float time = obj.GetComponent<TrailRenderer>().time;
        yield return new WaitForSeconds(time);
        DestroyObject(obj);
    }
}
