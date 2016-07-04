using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameScene : MonoBehaviour {

	public void LoadGame(string name)
    {
        SceneManager.LoadScene(name);
    }
}
