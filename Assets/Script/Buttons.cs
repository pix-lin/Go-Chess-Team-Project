using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    int currentSceneIndex;

    private void Awake()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    

    public void SingleButton()
    {
        SceneManager.LoadScene("SkillSelect");
    }

    public void MultiButton()
    {
        SceneManager.LoadScene("PvPLobby");
    }

    public void SettingButton()
    {
        //세팅 UI 활성화 하기
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        
        SceneManager.LoadScene("MainMenu");
    }

    public void StartButton()
    {
        SceneManager.LoadScene("Tournament");
    }

    
}
