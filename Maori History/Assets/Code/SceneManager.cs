using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManager : MonoBehaviour
{

    public static SceneManager Instance;
    
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
   public void LoadScene(string sceneName){
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
   }

   public void RestartCurrentScene(){
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
   }

   public void LoadMainMenu(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
   }

   public void QuitGame(){
        Application.Quit();
   }
}
