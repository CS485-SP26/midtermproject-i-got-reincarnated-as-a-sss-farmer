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
    private float timer;

    void Awake()
    {
        Debug.Assert(plantedModel != null, "Plant: plantedModel is not assigned in the Inspector.", this);
        Debug.Assert(growingModel != null, "Plant: growingModel is not assigned in the Inspector.", this);
        Debug.Assert(matureModel != null, "Plant: matureModel is not assigned in the Inspector.", this);
        Debug.Assert(witheredModel != null, "Plant: witheredModel is not assigned in the Inspector.", this);
    }

    void Start()
    {
        timer = timeToNextStage;
        UpdatePlantVisuals();
    }

    void Update()
    {
        // Only grow if the plant hasn't reached Mature or Withered yet
        if (currentState == PlantState.Planted || currentState == PlantState.Growing)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Grow();
                timer = timeToNextStage; // Reset timer for the next stage
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
        if (plantedModel != null) plantedModel.SetActive(currentState == PlantState.Planted);
        if (growingModel != null) growingModel.SetActive(currentState == PlantState.Growing);
        if (matureModel != null) matureModel.SetActive(currentState == PlantState.Mature);
        if (witheredModel != null) witheredModel.SetActive(currentState == PlantState.Withered);
    }
}