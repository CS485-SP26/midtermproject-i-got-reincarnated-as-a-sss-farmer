using Unity.VisualScripting;
using UnityEngine;

public class Plant : MonoBehaviour
{
    //Defines states of the plant
    public enum PlantState {Planted, Growing, Mature, Withered }
    public PlantState currentState;

    [Header("Visual Models")]
    public GameObject plantedModel;
    public GameObject growingModel;
    public GameObject matureModel;
    public GameObject witheredModel;

    [Header("Settings")]
    public float timeToNextStage = 5.0f; // Seconds between growth stages
    private float timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = timeToNextStage;
        UpdatePlantVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        //only grow if the plant hasn't matured or withered yet
        if (currentState == PlantState.Planted || currentState == PlantState.Growing)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Grow();
                timer = timeToNextStage; // reset timer for next stage
            }
        }
    }


    void Grow()
    {
        if (currentState == PlantState.Planted)
        {
            ChangeState(PlantState.Growing);
        }
        else if (currentState == PlantState.Growing)
        {
            ChangeState(PlantState.Mature);
        }
    }

    
    public void ChangeState(PlantState newState)
    {
     currentState = newState;
     UpdatePlantVisuals();   
    }

    void UpdatePlantVisuals()
    {
        //Disable all models first
        plantedModel.SetActive(false);
        growingModel.SetActive(false);
        matureModel.SetActive(false);
        witheredModel.SetActive(false);

        //Enable only model for the current state;
        switch(currentState)
        {
            case PlantState.Planted:
                plantedModel.SetActive(true);
                break;
            case PlantState.Growing:
                growingModel.SetActive(true);
                break;
            case PlantState.Mature:
                matureModel.SetActive(true);
                break;
            case PlantState.Withered:
                witheredModel.SetActive(true);
                break;
        }
    }
}
