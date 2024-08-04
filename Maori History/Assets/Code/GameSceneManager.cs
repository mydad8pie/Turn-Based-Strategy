using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameSceneManager : MonoBehaviour
{
   
    public static GameSceneManager Instance;
    
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

   public void StartGame(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game"); 
   }

   public void StartMapEditor(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapEditor");
   }
}
