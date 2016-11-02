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

    void Awake()
    {
        numbers.Add(num);
        gameObject.SetActive(false);
    }

    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => 
        {
            if (gameObject.activeSelf && Time.realtimeSinceStartup - startTime > displayTime)
            {
                gameObject.SetActive(false);
            }
        });

        Display(1234);
    }

    public void Display(int number)
    {
        int index = 0;

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
