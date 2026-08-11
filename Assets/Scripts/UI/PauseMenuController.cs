using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject _pauseMenuPanel; // Drag 'Esc' GameObject here
    [SerializeField] private GameObject _photoPiecesPanel; // Drag 'PhotoPieces' GameObject here

    [Header("Scene Names")]
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    private bool _isPaused = false;

    void Start()
    {
        // Hide panels when the game begins so they don't block the screen
        if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
        if (_photoPiecesPanel != null) _photoPiecesPanel.SetActive(false);
        _isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Support both Input systems
        bool escPressed = Input.GetKeyDown(KeyCode.Escape);

#if ENABLE_INPUT_SYSTEM
        if (!escPressed && UnityEngine.InputSystem.Keyboard.current != null)
        {
            escPressed = UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
        }
#endif

        if (escPressed)
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    public void OpenPauseMenu()
    {
        _isPaused = true;
        if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game physics/time
    }

    public void ResumeGame()
    {
        _isPaused = false;
        if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
        if (_photoPiecesPanel != null) _photoPiecesPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    // --- BUTTON FUNCTIONS ---

    public void OnHomeButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    public void OnPhotoPiecesButtonClicked()
    {
        if (_photoPiecesPanel != null)
        {
            _photoPiecesPanel.SetActive(true);
        }
    }

    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}