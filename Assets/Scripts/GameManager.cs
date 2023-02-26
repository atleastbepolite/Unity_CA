using UnityEngine;
using UnityEngine.SceneManagement;

// If there is 0 or 1 player remaining, restart the scene round 
public class GameManager : MonoBehaviour
{
    public GameObject[] players;

    public void CheckWinState()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        int aliveCount = 0;

        foreach (GameObject player in players)
        {
            
            if (player.activeSelf) {
                aliveCount++;
            }
        }

        if (aliveCount <= 1) {
            Invoke(nameof(NewRound), 2f);
        }
    }

    private void NewRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
