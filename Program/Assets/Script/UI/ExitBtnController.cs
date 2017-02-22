using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ExitBtnController : MonoBehaviour {

    [SerializeField]
    GameScene gameScene;

    public GameObject btnText;
    private Subject<Color> btnColor = new Subject<Color>();

	void Awake () {
		GetComponent<Button>().OnClickAsObservable().Subscribe( _ => OnBtnClicked() );
        btnColor.Where( x => x == Color.white ).Subscribe( x => gameScene.LoadGame( "GFF" ) );
	}

    void OnBtnClicked() {
        GameObject.Destroy( GetComponent<Animator>() );
        btnText.GetComponent<Text>().color = Color.white;
        btnColor.OnNext( btnText.GetComponent<Text>().color );
    }
}
