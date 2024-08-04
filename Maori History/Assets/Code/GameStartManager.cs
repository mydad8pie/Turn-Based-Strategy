using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStartManager : MonoBehaviour
{
    public Button gameStartButton;

    public Button mapEditorButton;

    void Start(){
        gameStartButton.onClick.AddListener(GameStart);
        mapEditorButton.onClick.AddListener(MapStart);

    }


    public void GameStart(){
        GameSceneManager.Instance.StartGame();
    }

    public void MapStart(){
        GameSceneManager.Instance.StartMapEditor();
    }
}
