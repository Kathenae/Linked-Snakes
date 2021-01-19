using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    float playerScore;

    [SerializeField]
    private float countDownSeconds = 5;
    private float countDownTimer;

    [SerializeField]
    private float scoreToReach;

    public GameObject startDisplay;
    public GameObject endDisplay;
    public Text startText;
    public Text endText;
    public Text countDownText;
    public Text playerScoreText;
    public Text enemyScoreText;

    public static GameManager instance;

    void Awake()
    {
        GameEvents.collectibleEaten += OnCollectibleEatenTriggered;
        GameEvents.snakeDeath += OnSnakeDeathTriggered;

        countDownTimer = countDownSeconds;

        startText.text = startText.text.Replace("#", scoreToReach.ToString());
        Time.timeScale = 1;
        UpdateScoreText();
    }

    void OnDestroy()
    {
        GameEvents.collectibleEaten -= OnCollectibleEatenTriggered;
        GameEvents.snakeDeath -= OnSnakeDeathTriggered;
    }

    bool gameStarted;

    void Update()
    {
        if (gameStarted == false)
        {
            countDownTimer -= Time.deltaTime;
            countDownText.text = "" + Mathf.Round(countDownTimer);

            if (countDownTimer <= 0)
            {
                gameStarted = true;
                GameEvents.TriggerGameStart();
                startDisplay.SetActive(false);
            }
        }
    }

    void OnCollectibleEatenTriggered(Snake eater, GameObject collectible)
    {
        if (eater is PlayerSnake)
            IncreasePlayerScore(1);
    }

    //void OnNodeStolenTriggered(Snake stealer, SnakeNode node)
    //{
    //    if (stealer is PlayerSnake)
    //        IncreasePlayerScore(2);
    //}

    void OnSnakeDeathTriggered(Snake deased)
    {
        if (deased is EnemySnake)
            IncreasePlayerScore(10);

        if (deased is PlayerSnake)
            LoseGame();
    }

    void LoseGame()
    {
        Time.timeScale = 0;
        GameEvents.TriggerGamePause(true);
        endDisplay.SetActive(true);
        endText.text = "<color=orange>You Died</color>\n\n\nRestart?";
    }

    void WinGame()
    {
        Time.timeScale = 0;
        GameEvents.TriggerGamePause(true);
        endText.text = "<color=orange>You Won</color>\n\n\n Replay?";
        endDisplay.SetActive(true);
    }

    void IncreasePlayerScore(int amount)
    {
        playerScore += amount;
        UpdateScoreText();

        if (playerScore >= scoreToReach)
            WinGame();
    }

    void UpdateScoreText()
    {
        playerScoreText.text = "" + playerScore + " pts";
    }

}
