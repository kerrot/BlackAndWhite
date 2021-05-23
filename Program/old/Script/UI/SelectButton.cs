using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// button select effect.
public class SelectButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private GameObject selected;

    private Subject<SelectButton> mouseOver = new Subject<SelectButton>();
    public IObservable<SelectButton> OnMouseOver { get { return mouseOver; } }

    private Animator anim;
    private Button btn;
    private Image img;

    // Use this for initialization
    void Awake()
    {
        btn = GetComponent<Button>();
              
    }

    //mouse over to select
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver.OnNext(this);
    }

    public void Select(bool v)
    {
        if (!anim)
        {
            anim = GetComponent<Animator>();
        }

        anim.enabled = v;

        if (!img)
        {
            img = GetComponent<Image>();
        }

        img.color = (v) ? Color.white : Color.gray;

        selected.SetActive(v);
    }

    public void Click()
    {
        if (btn)
        {
            btn.onClick.Invoke();
        }
    }
}
