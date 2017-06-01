using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class NumberDisplayUI : MonoBehaviour {
    [SerializeField]
    private NumberUI num;
    [SerializeField]
    private float displayTime;

    List<NumberUI> numbers = new List<NumberUI>();
    float startTime;

    const string animName = "Combo";
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        numbers.Add(num);
    }

    void Start()
    {
        this.UpdateAsObservable().Where(_ => gameObject.activeSelf && Time.realtimeSinceStartup - startTime > displayTime)
                                 .Subscribe(_ => gameObject.SetActive(false));

        //Display(1234);
    }

    private void OnEnable()
    {
        anim.Play(animName, 0, 0);
    }

    public void Display(int number)
    {
        int index = 0;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        startTime = Time.realtimeSinceStartup;
        numbers.ForEach(n => n.gameObject.SetActive(false));
        num.SetNumber(0);

        while (number > 0)
        {
            if (numbers.Count <= index)
            {
                GameObject obj = Instantiate(num.gameObject, new Vector3(), num.gameObject.transform.rotation) as GameObject;
                obj.transform.SetParent(transform);
                numbers.Add(obj.GetComponent<NumberUI>());

                Image newImg = obj.GetComponent<Image>();
                Image oriImg = num.gameObject.GetComponent<Image>();

                newImg.gameObject.UpdateAsObservable().Subscribe(_ => newImg.color = oriImg.color);

                RectTransform t = numbers[index - 1].gameObject.GetComponent<RectTransform>();
                RectTransform r = obj.GetComponent<RectTransform>();
                r.sizeDelta = t.sizeDelta;
                r.localPosition = new Vector3(t.localPosition.x - t.rect.width, t.localPosition.y, t.localPosition.z);
            }

            numbers[index].SetNumber(number % 10);

            ++index;
            number /= 10;
        }
    }

}
