using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public HexGrid hexGrid;

    UnitManager unitManager;
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

    private List<TrainingVillage> trainingVillages = new List<TrainingVillage>();

    void Start()
    {
        unitManager = FindObjectOfType<UnitManager>();
        builderUI.SetActive(false);
        warriorUI.SetActive(false);
        settlerUI.SetActive(false);
        villageUI.SetActive(false);

        trainBuilderButton.onClick.AddListener(TrainBuilder);
        trainWarriorButton.onClick.AddListener(TrainWarrior);
        trainSettlerButton.onClick.AddListener(TrainSettler);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !PauseManager.Instance.IsPaused)
        {
            if (selectedUnit != null)
            {
                MoveSelectedUnit();
            }
            else
            {
                SelectVillage();

                if (selectedVillage == null)
                {
                    SelectUnit();
                }
            }
        }

        CheckIfTraining();

        if (Input.GetKeyDown(KeyCode.M) && selectedUnit != null)
        {
            PlaceVillage();
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

                if (selectedUnit.gameObject.name == "Builder(Clone)")
                {
                    builderUI.SetActive(true);
                }
                else if (selectedUnit.gameObject.name == "Warrior(Clone)")
                {
                    warriorUI.SetActive(true);
                }
                else if (selectedUnit.gameObject.name == "Settler(Clone)")
                {
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

    void SelectVillage()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject selectedObject = hit.collider.gameObject;

            if (selectedObject.name.Contains("Village"))
            {
                selectedVillage = selectedObject;
                Debug.Log("Selected village: " + selectedVillage.gameObject.name);
                villageUI.SetActive(true);
            }
            else
            {
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
            if (targetCell == selectedUnit.CurrentCell)
            {
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

    public void DeselectVillage()
    {
        if (selectedVillage != null)
        {
            selectedVillage = null;
        }
        villageUI.SetActive(false);
    }

    public void PlaceVillage()
    {
        if (selectedUnit != null && selectedUnit.gameObject.name == "Settler(Clone)" && selectedUnit.currentMoveRange > 0)
        {
            GameObject newVillage = Instantiate(villagePrefab, selectedUnit.CurrentCell.Position + Constants.UnitOffset, Quaternion.identity);
            Village VillageComponent = newVillage.GetComponent<Village>();
            if (VillageComponent != null)
            {
                VillageComponent.hexGrid = hexGrid;
                VillageComponent.CurrentCell = selectedUnit.CurrentCell;
            }

            Destroy(selectedUnit.gameObject);
            DeselectUnit();
        }
    }

    public void TrainBuilder()
    {
        if (selectedVillage != null)
        {
            if (!IsVillageTraining(selectedVillage))
            {
                AddVillageToTrainingList(selectedVillage, "Builder", 0);
            }
        }
    }

    public void TrainWarrior()
    {
        if (selectedVillage != null)
        {
            if (!IsVillageTraining(selectedVillage))
            {
                AddVillageToTrainingList(selectedVillage, "Warrior", 2);
            }
        }
    }

    public void TrainSettler()
    {
        if (selectedVillage != null)
        {
            if (!IsVillageTraining(selectedVillage))
            {
                AddVillageToTrainingList(selectedVillage, "Settler", 1);
            }
        }
    }

    void AddVillageToTrainingList(GameObject village, string unitType, int trainingDuration)
    {
        TrainingVillage trainingVillage = new TrainingVillage
        {
            village = village,
            unitType = unitType,
            startTurn = TurnManager.Instance.turnCounter,
            trainingDuration = trainingDuration
        };

        trainingVillages.Add(trainingVillage);

        // Start the coroutine to flash the village color red when training starts
        StartCoroutine(FlashVillageColor(village));

        Debug.Log("Started training " + unitType + " in " + village.name + " on turn: " + TurnManager.Instance.turnCounter);
    }

    bool IsVillageTraining(GameObject village)
    {
        return trainingVillages.Exists(tv => tv.village == village);
    }

    public void CheckIfTraining()
    {
        for (int i = trainingVillages.Count - 1; i >= 0; i--)
        {
            TrainingVillage tv = trainingVillages[i];
            if (TurnManager.Instance.turnCounter > tv.startTurn + tv.trainingDuration)
            {
                SpawnTrainedUnit(tv);
                trainingVillages.RemoveAt(i);
            }
        }
    }

    void SpawnTrainedUnit(TrainingVillage tv)
    {
        HexCell villageCell = tv.village.GetComponent<Village>().CurrentCell;
        switch (tv.unitType)
        {
            case "Builder":
                unitManager.SpawnBuilder(villageCell);
                break;
            case "Warrior":
                unitManager.SpawnWarrior(villageCell);
                break;
            case "Settler":
                unitManager.SpawnSettler(villageCell);
                break;
        }

        // Stop the flashing and reset the village color to its original color after training
        StopAllCoroutines();
        ChangeVillageColor(tv.village, Color.white);

        Debug.Log(tv.unitType + " trained in " + tv.village.name);
    }

    IEnumerator FlashVillageColor(GameObject village)
    {
        Renderer villageRenderer = village.GetComponent<Renderer>();
        Color originalColor = villageRenderer.material.color;
        Color flashColor = Color.red;

        while (true)
        {
            villageRenderer.material.color = flashColor;
            yield return new WaitForSeconds(0.5f);
            villageRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ChangeVillageColor(GameObject village, Color color)
    {
        Renderer villageRenderer = village.GetComponent<Renderer>();
        if (villageRenderer != null)
        {
            villageRenderer.material.color = color;
        }
    }

    class TrainingVillage
    {
        public GameObject village;
        public string unitType;
        public int startTurn;
        public int trainingDuration;
    }
}
