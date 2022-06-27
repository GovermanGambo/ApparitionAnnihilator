using UnityEngine;
using UnityEngine.SceneManagement;

public class Core : MonoBehaviour
{
    private void Awake()
    {
        var scene = SceneManager.GetSceneByName("BaseScene");
        if (!scene.isLoaded)
        {
            SceneManager.LoadSceneAsync("BaseScene", LoadSceneMode.Additive);
        }
        
        int levelCounter = int.Parse(gameObject.scene.name.Substring(5));
        Game.LevelCounter = levelCounter;
    }
}
