using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Счётчик")]
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _summaryText;

    private int _totalShots = 0;
    private int _successfulHits = 0;
    private int _totalTargetsSpawned = 0;

    private void Start()
    {
        UpdateScoreDisplay();
    }

    public void RegisterShot()
    {
        _totalShots++;
        UpdateScoreDisplay();
    }

    public void RegisterHit()
    {
        _successfulHits++;
        UpdateScoreDisplay();
    }

    public void RegisterTargetSpawned()
    {
        _totalTargetsSpawned++;
    }

    private void UpdateScoreDisplay()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Выстрелы: {_totalShots} | Попадания: {_successfulHits}";
        }

        if (_summaryText != null && _totalShots > 0)
        {
            float accuracy = (_successfulHits / (float)_totalShots) * 100f;
            _summaryText.text = $"Точность: {accuracy:F1}%";
        }
    }

    public void ShowFinalSummary()
    {
        float accuracy = _totalShots > 0 ? (_successfulHits / (float)_totalShots) * 100f : 0f;

        string summary = $@"ИТОГОВАЯ СВОДКА:
Всего выстрелов: {_totalShots}
Попаданий: {_successfulHits}
Точность: {accuracy:F1}%
Мишеней создано: {_totalTargetsSpawned}";

        Debug.Log(summary);

        if (_summaryText != null)
        {
            _summaryText.text = summary;
        }
    }

  
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowFinalSummary();
        }
    }
}