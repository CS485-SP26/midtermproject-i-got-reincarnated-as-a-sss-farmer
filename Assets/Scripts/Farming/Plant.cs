using UnityEngine;

namespace Farming
{
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
        private bool isWatered = false; // Plant needs to be watered to start growing

        void Start()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            timer = timeToNextStage;
            currentState = PlantState.Planted; // Ensure we start in Planted state
            UpdatePlantVisuals();
        }

        void Update()
        {
<<<<<<< HEAD
            // Only grow if the plant has been watered and hasn't reached Mature or Withered yet
            if (isWatered && (currentState == PlantState.Planted || currentState == PlantState.Growing))
=======
            // changed so the plants can now wither (will be needed as withered plants shouldn't increment the plantInventory counter)
            if (currentState == PlantState.Planted || currentState == PlantState.Growing || currentState == PlantState.Mature)
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    Grow();
                    timer = timeToNextStage; // Reset timer for the next stage
                }
            }
        }
        
        /// <summary>
        /// Set the watered state of the plant. Plant will only grow when watered.
        /// </summary>
        public void SetWatered(bool watered)
        {
            bool wasWatered = isWatered;
            isWatered = watered;
            if (watered && !wasWatered)
            {
                Debug.Log($"[Plant] Plant watered - growth started!");
            }
        }

        void Grow()
        {
            if (currentState == PlantState.Planted)
            {
                Debug.Log("[Plant] Growing from Planted to Growing stage!");
                ChangeState(PlantState.Growing);
            }
            else if (currentState == PlantState.Growing)
            {
                Debug.Log("[Plant] Growing from Growing to Mature stage!");
                ChangeState(PlantState.Mature);
            }
            else if(currentState == PlantState.Mature)
            {
                ChangeState(PlantState.Withered);
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
}
