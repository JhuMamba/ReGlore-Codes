using JhutenFPP.PlayerControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    public GameObject EndScene;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        State = GameState.PLAY;
    }
    private void Update()
    {
        if (PlayerController.Instance.IsDead && State == GameState.PLAY)
        {
            State = GameState.END;
            ShowEndScene();
            Time.timeScale = 0f;
        }
    }
    private void ShowEndScene()
    {
        EndScene.SetActive(!EndScene.activeSelf);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ShowEndScene();
        Time.timeScale = 1f;
    }
}
public enum GameState
{
    PLAY,
    END
}