using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchButton : MonoBehaviour
{
    public GameObject tournamentUI;
    public GameObject gameUI;

    public GameObject enemy1x;
    public GameObject enemy2x;
    public GameObject enemy3x;

    private int winCount = 0;


    private void Start()
    {
        enemy1x.SetActive(false);
        enemy2x.SetActive(false);
        enemy3x.SetActive(false);
    }



    public void MatchStart()
    {
        winCount++;

        if (winCount >= 4)
        {
            SceneManager.LoadScene("Title");
            return;
        }

        tournamentUI.SetActive(false);
        gameUI.SetActive(true);               
    }


    public void Win()
    {
        winCount++;

        gameUI.SetActive(false);
        tournamentUI.SetActive(true);

        if (winCount == 1)
        {
            enemy1x.SetActive(true);
        }
        else if(winCount == 2)
        {
            enemy2x.SetActive(true);
        }
        else if(winCount == 3)
        {
            enemy3x.SetActive(true);
        }
    }


    public void Lose()
    {
        SceneManager.LoadScene("Title");
    }
}
