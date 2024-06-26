using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{

    public static PauseManager Instance { get; private set; }


    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

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
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
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

        Debug.Log("Quitting");  

        
    }
}
