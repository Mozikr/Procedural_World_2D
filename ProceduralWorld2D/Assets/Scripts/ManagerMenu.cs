using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerMenu : MonoBehaviour
{
    AudioManager _audioManager;
    public Texture2D cursorArrow;

    private void Start()
    {
        Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _audioManager = FindObjectOfType<AudioManager>();
        _audioManager.Play("MenuNaturaAmbient");
    }

    public void StartTheGame()
    {
        StartCoroutine(StartG());
    }

    public void EndTheGame()
    {
        StartCoroutine(EndG());
    }

    IEnumerator StartG()
    {
        _audioManager.Play("Clicked");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Game");
    }

    IEnumerator EndG()
    {
        _audioManager.Play("Clicked");
        yield return new WaitForSeconds(1);
        Application.Quit();
    }
}
