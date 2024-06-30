using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public static PauseManager Instance { get; private set; }


    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

    public Button settingsButton;

    private bool isPaused = false;

    public bool IsPaused
    {
        get { return isPaused; }
    }


    void Awake(){
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        pauseMenuUI.SetActive(false);
        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
        settingsButton.onClick.AddListener(Pause);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
                
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        settingsButton.onClick.AddListener(Pause);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        settingsButton.onClick.AddListener(Resume);
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        Debug.Log("Restarting");
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.LoadMainMenu();

        
    }
}
