using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public HexGrid hexGrid;
    private UnitMovement selectedUnit;
    private GameObject selectedVillage;
    public GameObject builderUI;
    public GameObject warriorUI;
    public GameObject settlerUI;
    public GameObject villageUI;
    public GameObject villagePrefab;

    public Button trainBuilderButton;
    public Button trainWarriorButton;
    public Button trainSettlerButton;

    public Button placeVillageButton;

    void Start(){
        builderUI.SetActive(false);
        warriorUI.SetActive(false);
        settlerUI.SetActive(false);
        villageUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !PauseManager.Instance.IsPaused)
        {
            if(selectedUnit != null){

                MoveSelectedUnit();
                
            }
            else{
                SelectVillage();

                if (selectedVillage == null){
                    SelectUnit();
                }
                else{

                }
                trainBuilderButton.onClick.AddListener(TrainBuilder);
                trainWarriorButton.onClick.AddListener(TrainWarrior);
                trainSettlerButton.onClick.AddListener(TrainSettler);

            }
        }
        if (Input.GetKeyDown(KeyCode.M) && selectedUnit != null)
        {
            PlaceVillage();
        }

        if (TurnManager.Instance.unitTrainingStartTurn != -1){
            TrainBuilder();
        }
       
    }

    void SelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject selectedObject = hit.collider.gameObject;
            UnitMovement unitMovement = selectedObject.GetComponent<UnitMovement>();

            if (unitMovement != null)
            {
                selectedUnit = unitMovement;
                Debug.Log("Selected unit: " + selectedUnit.gameObject.name);
                Debug.Log("Selected unit max move range: " + selectedUnit.maxMoveRange);
                unitMovement.HighlightReachableCells();


                if (selectedUnit.maxMoveRange == 0)
                {
                    Debug.Log("Unit has no move range.");
                }


                if(selectedUnit.gameObject.name == "Builder(Clone)"){
                    builderUI.SetActive(true);
                }
                else if(selectedUnit.gameObject.name == "Warrior(Clone)"){
                    warriorUI.SetActive(true);
                }
                else if(selectedUnit.gameObject.name == "Settler(Clone)"){
                    settlerUI.SetActive(true);
                    placeVillageButton.onClick.AddListener(PlaceVillage);
                    
                }
            }
            
            else
            {
                Debug.Log("No unit found at clicked position.");
            }
            
        }
    }

    void SelectVillage(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)){
            GameObject selectedObject = hit.collider.gameObject;

            if (selectedObject.name == "Village 1(Clone)"){
                selectedVillage = selectedObject;
                Debug.Log("Selected village: " + selectedVillage.gameObject.name);
                villageUI.SetActive(true);
            }
            else{
                DeselectVillage();
            }
        }
    }

    void MoveSelectedUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HexCell targetCell = hexGrid.GetCell(hit.point);
            if(targetCell == selectedUnit.CurrentCell){
                DeselectUnit();
                Debug.Log("Deselected unit");
            }
            else if (targetCell != null && selectedUnit != null)
            {
                selectedUnit.MoveTo(targetCell);
                DeselectUnit();
            }
            
        }
    }

    public void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.ClearReachableCells();
            selectedUnit = null;
        }
        builderUI.SetActive(false);
        warriorUI.SetActive(false);
        settlerUI.SetActive(false);
        villageUI.SetActive(false);
    }

    public void DeselectVillage(){
        if (selectedVillage != null){
            selectedVillage = null;
        }
        villageUI.SetActive(false);
    }

    public void PlaceVillage(){
        if (selectedUnit !=null && selectedUnit.gameObject.name == "Settler(Clone)" && selectedUnit.currentMoveRange > 0){
            Instantiate(villagePrefab, selectedUnit.CurrentCell.Position +Constants.UnitOffset, Quaternion.identity);
            Destroy(selectedUnit.gameObject);
            DeselectUnit(); 
        }

    }

    public void TrainBuilder(){
        if (selectedVillage != null){
            if(TurnManager.Instance.unitTrainingStartTurn == -1){
                TurnManager.Instance.unitTrainingStartTurn = TurnManager.Instance.turnCounter;
                Debug.Log("Training builder on turn: " + TurnManager.Instance.turnCounter);

            }
            else if(TurnManager.Instance.turnCounter > TurnManager.Instance.unitTrainingStartTurn + 2){
                Debug.Log("Builder trained");
                TurnManager.Instance.unitTrainingStartTurn = -1;
            }

            else{
                Debug.Log("Training builder in progress " + (TurnManager.Instance.turnCounter - TurnManager.Instance.unitTrainingStartTurn) + " turns passed");
            }
            
        }
    }

    public void TrainWarrior(){
        if (selectedVillage != null){
            Debug.Log("Training warrior");
        }
    }

    public void TrainSettler(){
        if (selectedVillage != null){
            Debug.Log("Training settler");
        }
    }

}


