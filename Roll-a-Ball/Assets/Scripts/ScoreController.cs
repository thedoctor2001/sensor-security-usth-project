using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private CommonConfigurations configurations;
    private int _currentScore;
    private TextMeshProUGUI _textMeshProUGUI;

    [Header("Score Information")]
    public string userID;
    public string token;


    private void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        ShowScoreText();
    }

    private void OnEnable()
    {
        EventManager.StartListening("Increase score", IncreaseScore);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Player collided a pickup", IncreaseScore);
    }

    private void IncreaseScore()
    {
        _currentScore++;
        ShowScoreText();
        if (_currentScore == configurations.winningScore)
        {
            InsertScoreTrigger insertScoreTrigger = gameObject.AddComponent<InsertScoreTrigger>();
            insertScoreTrigger.token = token;
            insertScoreTrigger.value = _currentScore;
            insertScoreTrigger.userID = userID;
            insertScoreTrigger.InsertScore();
            Win();
        }
    }

    private void ShowScoreText()
    {
        _textMeshProUGUI.text = "Score: " + _currentScore;
    }

    private void Win()
    {
        EventManager.TriggerEvent("Victory");
    }
}