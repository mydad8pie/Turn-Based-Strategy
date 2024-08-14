using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageActions : MonoBehaviour
{

    public Button trainBuilderButton;
    public Button trainWarroirButton;
    public Button trainSettlerButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnManager.Instance.currentPlayerIndex != 0)
        {
            trainBuilderButton.interactable = false;
            trainWarroirButton.interactable = false;
            trainSettlerButton.interactable = false;
        }
        else
        {
            trainBuilderButton.interactable = true;
            trainWarroirButton.interactable = true;
            trainSettlerButton.interactable = true;
        }
        trainBuilderButton.onClick.AddListener(TrainBuilder);
        trainWarroirButton.onClick.AddListener(TrainWarrior);
        trainSettlerButton.onClick.AddListener(TrainSettler);
    }

    public void TrainBuilder()
    {
        Debug.Log("Train Builder");
    }

    public void TrainWarrior()
    {
        Debug.Log("Train Warrior0");
    }

    public void TrainSettler()
    {
        Debug.Log("Train Settler");
    }


}
