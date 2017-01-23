using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseCon : MonoBehaviour {

    [SerializeField]
    GameSystem gameSytem;

    public GameObject pausePlane;
    public GameObject backGround;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject pauseText;
    private GameObject pasueButton;

    private const float scalePos = 0.01f;
    private const float maxScalePos = 1.0f;
    private const float speed = 0.1f;
    private Vector3 maxScale;
    private bool isScale = false;

    void Awake() {  
        pasueButton = GameObject.Find( "Pause Button" );
        gameSytem = GameObject.Find( "Target" ).GetComponent<GameSystem>();
        backGround.transform.localScale = new Vector3( scalePos, scalePos, maxScalePos );
        maxScale = new Vector3( maxScalePos, maxScalePos, maxScalePos );
        pasueButton.SetActive( false ); 
    }

	
	// Update is called once per frame
	void Update () {

        if ( gameSytem.State == GameSystem.GameState.GAME_STATE_PLAYING ) {
            pasueButton.SetActive( true );
        } else {
            pasueButton.SetActive( false );
        }

        if ( isScale ) {
            backGround.transform.localScale = Vector3.MoveTowards( backGround.transform.localScale, pausePlane.transform.localScale, speed );
        }

        if ( backGround.transform.localScale == maxScale ) {
            isScale = false;
            pauseText.SetActive( true );
            yesButton.SetActive( true );
            noButton.SetActive( true );
        } else { 
            pauseText.SetActive( false );
            yesButton.SetActive( false );
            noButton.SetActive( false );
        }

	}

    public void OnYesButtonClicked() {
        SceneManager.LoadScene("TCATitle");
    }

    public void OnNoButtonClicked() {
        backGround.transform.localScale = new Vector3( scalePos, scalePos, maxScalePos );
        pausePlane.SetActive( false );
        gameSytem.GameResume();
    }

    public void OnPauseButtonClicke() {
        pausePlane.SetActive( true );
        isScale = true;
        gameSytem.GamePause();
    }

}
