using UnityEngine.SceneManagement;
using Youregone.GameplaySystems;
using UnityEngine.UI;
using UnityEngine;
using Zenject;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _background;
    [SerializeField] private Image _gameOverImage;
    [SerializeField] private Button _restartButton;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    private GameState _gameState;

    [Inject]
    public void Construct(GameState gameState)
    {
        _gameState = gameState;
        _gameState.OnGameStateChanged += GameState_OnGameStateChanged;
    }

    private void Awake()
    {
        Hide();

        _restartButton.onClick.AddListener(() =>
        {

        });
    }

    private void OnDestroy()
    {
        _gameState.OnGameStateChanged -= GameState_OnGameStateChanged;
    }

    private void GameState_OnGameStateChanged(EGameState gameState)
    {
        if (gameState == EGameState.GameOver)
            Show();
    }

    private void Show()
    {
        _background.gameObject.SetActive(true);
        _gameOverImage.gameObject.SetActive(true);
        _restartButton.gameObject.SetActive(true);
        _gameOverText.gameObject.SetActive(true);
    }

    private void Hide()
    {
        _background.gameObject.SetActive(false);
        _gameOverImage.gameObject.SetActive(false);
        _restartButton.gameObject.SetActive(false);
        _gameOverText.gameObject.SetActive(false);
    }
}
