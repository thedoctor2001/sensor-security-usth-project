using TMPro;
using UnityEngine;

public class VictoryController : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;

    private void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _textMeshProUGUI.text = "";
    }

    private void OnEnable()
    {
        EventManager.StartListening("Victory", ShowVictory);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Victory", ShowVictory);
    }

    private void ShowVictory()
    {
        _textMeshProUGUI.text = "Victory after" + " " + Time.time + " " + "seconds";
        Time.timeScale = 0;
    }
}