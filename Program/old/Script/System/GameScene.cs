using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// change level
public class GameScene : MonoBehaviour {

	public void LoadGame(string name)
    {
        SceneManager.LoadScene(name);
    }

    static public void ReStartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    static public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
