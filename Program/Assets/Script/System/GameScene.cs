using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameScene : MonoBehaviour {

	public void LoadGame(string name)
    {
        SceneManager.LoadScene(name);
    }

    static public void ReStartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
