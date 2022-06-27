using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private InputAction startAction;

    private void Awake()
    {
        startAction.performed += cbx => Game.StartGame(this);
        SceneManager.LoadSceneAsync("BaseScene", LoadSceneMode.Additive);
    }

    private void OnEnable()
    {
        startAction.Enable();
    }

    private void OnDisable()
    {
        startAction.Disable();
    }
}
