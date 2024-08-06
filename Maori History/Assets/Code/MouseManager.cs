using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Color hoverColor = Color.white;

    private HexCell previousCell = null;

    private Color previousColor;

    public HexGrid hexGrid;
    public GameObject objectToPlace;

    public GameObject builderToPlace;

    public TurnManager turnManager;

    private GameObject selectedBuilder = null;

    // Update is called once per frame
    void Update()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)&& !PauseManager.Instance.IsPaused){

            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (currentCell != previousCell){
                if (previousCell != null){
                    previousCell.Color = previousColor;
                }
                if (currentCell != null){
                    previousColor = currentCell.Color;
                    currentCell.Color = hoverColor;
                }
                previousCell = currentCell;
            }

            //if (Input.GetMouseButtonDown(0) && currentCell != null && IsPlayerTurn()){ 
            //    PlaceObject(currentCell);
            //}

            if (Input.GetMouseButtonDown(1) && currentCell != null && IsPlayerTurn()){ 
                PlaceBuilder(currentCell);
            }
            if (Input.GetMouseButtonDown(0) && currentCell != null){
                HandleLeftClick(currentCell, hit.collider.gameObject);
            }

            
        }
        else if (previousCell != null){
            previousCell.Color = previousColor;
            previousCell = null;
        }
    }
    void HandleLeftClick(HexCell cell, GameObject clickedObject){
        if (clickedObject.CompareTag("Builder")){
            SelectBuilder(clickedObject);
        }
    }

    void SelectBuilder(GameObject builder){
        if (selectedBuilder != null){
            DeslectBuilder(selectedBuilder);

        }
        selectedBuilder = builder;
        HighLightBuilder(selectedBuilder);
    }
    void DeslectBuilder(GameObject builder){
        Renderer builderRenderer = builder.GetComponent<Renderer>();
        if (builderRenderer != null){
            builderRenderer.material.color = Color.white;
            
        }
    }

    void HighLightBuilder(GameObject builder){
        Renderer builderRenderer = builder.GetComponent<Renderer>();
        if (builderRenderer != null){
            builderRenderer.material.color = Color.yellow;
        }

    }

    void PlaceObject(HexCell cell){
        Vector3 postion = cell.transform.position + new Vector3(0, 3f, 0);
        Instantiate(objectToPlace, postion, Quaternion.identity);
    }

    void PlaceBuilder(HexCell cell){
        Vector3 postion = cell.transform.position + new Vector3(0, 3f, 0);
        Instantiate(builderToPlace, postion, Quaternion.identity);
    }

    bool IsPlayerTurn(){
        return TurnManager.Instance.currentPlayerIndex == 0 && !TurnManager.Instance.playerHasCompletedTurn && !PauseManager.Instance.IsPaused;
    }
}
