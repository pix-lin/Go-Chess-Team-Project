using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    // Scene List의 다음 Scene으로 이동
    // 현재 Scene List (0: MainMenu, 1: Game)

    public void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
    }
}