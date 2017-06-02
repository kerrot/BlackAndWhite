using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

// exit button used in tutorial stage, to main stage
public class ExitBtnController : MonoBehaviour {

    [SerializeField]
    private GameScene gameScene;
    [SerializeField]
    private string stageName = "Main";

    public GameObject btnText;
    private Subject<Color> btnColor = new Subject<Color>();

	void Awake () {
		GetComponent<Button>().OnClickAsObservable().Subscribe( _ => OnBtnClicked() );
        btnColor.Where( x => x == Color.white ).Subscribe( x => gameScene.LoadGame(stageName) );
	}

    void OnBtnClicked() {
        GameObject.Destroy( GetComponent<Animator>() );
        btnText.GetComponent<Text>().color = Color.white;
        Observable.NextFrame( ).Subscribe( _ => btnColor.OnNext( btnText.GetComponent<Text>().color ) );
    }
}
