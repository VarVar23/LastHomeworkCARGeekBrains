using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager Instance { get; set; }

    public Text scoreLabel;
    int currentScore;
    int bestScore;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    void Start () {

        ResetScore();

        //load best score
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

	}
	
    //update score and score label
    public void UpdateScore(int value)
    {
        currentScore += value;

        if (GameManager.Instance.gameState == GameState.PLAYING)
            scoreLabel.text = currentScore + "";
    }

    //reset score
    public void ResetScore()
    {
        currentScore = 0;
        scoreLabel.text = currentScore + "";
    }

    //check for highscore on gameover
    public bool CheckHighScore()
    {
        return currentScore > bestScore;
    }

    //set and save highscore
    public void SetHighScore()
    {
        if (currentScore > bestScore)
            bestScore = currentScore;

        PlayerPrefs.SetInt("BestScore", bestScore);
    }

    //get best score
    public int BestScore()
    {
        return bestScore;
    }

    //get current score
    public int CurrentScore()
    {
        return currentScore;
    }
}
