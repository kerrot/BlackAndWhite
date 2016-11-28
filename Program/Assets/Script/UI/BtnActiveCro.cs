using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BtnActiveCro : MonoBehaviour {

    [SerializeField]SkillBtn skillBtn;

    // 按钮激活
	public bool redBtnActived;
	public bool blueBtnActived;
	public bool greenBtnActived;
    public bool powerBtnActived;

	private Image redBtn;
	private Image greeBtn;
	private Image blueBtn;
    private GameObject powerBtn;


	// Use this for initialization
	void Start () {
        redBtn = GameObject.Find("RedBtn").GetComponent<Image> ();
        greeBtn = GameObject.Find("GreenBtn").GetComponent<Image> ();
        blueBtn = GameObject.Find("BlueBtn").GetComponent<Image> ();
        powerBtn = GameObject.Find ("PowerBtn");
	}
	
	// Update is called once per frame
	void Update () {   

        // 按钮状态激活控制
        powerBtn.SetActive(powerBtnActived);
        skillBtn.redBtnActive.interactable = redBtnActived;
        skillBtn.greenBtnActive.interactable = greenBtnActived;
        skillBtn.blueBtnActive.interactable = blueBtnActived;  

	}
}
