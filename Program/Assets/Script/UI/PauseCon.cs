using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseCon : MonoBehaviour {

    [SerializeField]
    GameSystem gameSytem;
    [SerializeField]
    private AudioSource BGM;

    [SerializeField]
    private GameObject pausePlane;
    [SerializeField]
    private GameObject backGround;
    [SerializeField]
    private GameObject yesButton;
    [SerializeField]
    private GameObject noButton;
    [SerializeField]
    private GameObject pauseText;
    private GameObject pasueButton;

    private const float SCALE_POS = 0f;
    private const float MAXSCALE_POS = 1.0f;
    private const float SPEED = 0.1f;
    private Vector3 maxScale;
    private Vector3 minScale;
    private bool onFadeIn = false;
    private bool onFadeOut = false;

    void Awake() {  
        pasueButton = GameObject.Find( "Pause Button" );
        gameSytem = GameObject.Find( "Target" ).GetComponent<GameSystem>();
        minScale = new Vector3( SCALE_POS, SCALE_POS, MAXSCALE_POS );
        maxScale = new Vector3( MAXSCALE_POS, MAXSCALE_POS, MAXSCALE_POS );
        pasueButton.SetActive( false );
        backGround.transform.localScale = minScale;
        this.UpdateAsObservable().Subscribe(_ => UniRxUpdate());
       
    }

	void Start() {
        pasueButton.GetComponent<Button>().OnClickAsObservable().Subscribe(_ => OnPauseButtonClicked() );
        noButton.GetComponent<Button>().OnClickAsObservable().Subscribe(_ => onFadeOut = true );
        yesButton.GetComponent<Button>().OnClickAsObservable().Subscribe(_ => SceneManager.LoadScene("TCATitle") );
    }

	// Update is called once per frame
	void UniRxUpdate () {

        if ( gameSytem.State == GameSystem.GameState.GAME_STATE_PLAYING ) {
            pasueButton.SetActive( true );

            if (Input.GetButtonDown("Pause"))
            {
               OnPauseButtonClicked();
            }

        } else {
            pasueButton.SetActive( false );
        }

        ButtonFadeIn();
        ButtonFadeOut();
	}

    void ButtonFadeIn() {

        if ( onFadeIn ) {
            backGround.transform.localScale = Vector3.MoveTowards( backGround.transform.localScale, maxScale, SPEED );
        } else {
            return;
        }

        if ( backGround.transform.localScale == maxScale ) {
            onFadeIn = false;
            pauseText.SetActive( true );
            yesButton.SetActive( true );
            noButton.SetActive( true );
        }
    }

    void ButtonFadeOut() {

        if ( onFadeOut ) {
            pauseText.SetActive( false );
            yesButton.SetActive( false );
            noButton.SetActive( false );
            backGround.transform.localScale = Vector3.MoveTowards( backGround.transform.localScale, minScale, SPEED );
        } else {
            return;
        } 

        if ( backGround.transform.localScale == minScale ) {
            onFadeOut = false;
            pausePlane.SetActive( false );
            gameSytem.GameResume();
            BGM.UnPause();
        }
    }

    void OnPauseButtonClicked() {
        if (PlayerBattle.IsDead)
        {
            return;
        }

        pausePlane.SetActive( true );
        onFadeIn = true;
        gameSytem.GamePause();
        BGM.Pause();
    }

}
