using UnityEngine;

public enum PlantState { Planted, Growing, Mature, Withered }

public class Plant : MonoBehaviour
{
    public PlantState currentState;

    [Header("Visual Models")]
    public GameObject plantedModel;
    public GameObject growingModel;
    public GameObject matureModel;
    public GameObject witheredModel;

    [Header("Settings")]
    public float timeToNextStage = 5.0f; // Seconds between growth stages
    public float timeToWither = 10.0f;   // Seconds at Mature before withering
    private float timer;

    void Start()
    {
        timer = timeToNextStage;
        UpdatePlantVisuals();
    }

    void Update()
    {
        if (currentState == PlantState.Planted || currentState == PlantState.Growing)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Grow();
                timer = timeToNextStage; // Reset timer for the next stage
            }
        }
        else if (currentState == PlantState.Mature)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                ChangeState(PlantState.Withered);
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
            timer = timeToWither; // Start the wither countdown
        }
    }

    public void ChangeState(PlantState newState)
    {
        currentState = newState;
        UpdatePlantVisuals();
    }

    void UpdatePlantVisuals()
    {
        plantedModel.SetActive(currentState == PlantState.Planted);
        growingModel.SetActive(currentState == PlantState.Growing);
        matureModel.SetActive(currentState == PlantState.Mature);
        witheredModel.SetActive(currentState == PlantState.Withered);
    }
}