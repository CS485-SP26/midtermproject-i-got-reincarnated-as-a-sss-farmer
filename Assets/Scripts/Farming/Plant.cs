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

            if (waterReminderIcon)
            {
                waterReminderIcon.SetActive(false);
            }

        }

        void Update()
        {
<<<<<<< HEAD
<<<<<<< HEAD
            // Only grow if the plant has been watered and hasn't reached Mature or Withered yet
            if (isWatered && (currentState == PlantState.Planted || currentState == PlantState.Growing))
=======
            // changed so the plants can now wither (will be needed as withered plants shouldn't increment the plantInventory counter)
            if (currentState == PlantState.Planted || currentState == PlantState.Growing || currentState == PlantState.Mature)
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])
            {
                timer -= Time.deltaTime;

                if (timer <= 0f && (autoGrownStages < growthStagesAutoGrow || watered))
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
=======

            totalLifetime += Time.deltaTime;

            // first 15 seconds, cannot water plant, let it grow naturally
            if (totalLifetime >= 15)
            {
                waterable = true;
            }

            // only auto-grows for the randomly selected number of stages, then requires watering
            if (currentState == PlantState.Planted || currentState == PlantState.Growing)
            {
                if (autoGrownStages < growthStagesAutoGrow)
                {
                    timer -= Time.deltaTime;

                    if (timer <= 0f)
                    {
                        Grow();
                        autoGrownStages++;
                        timer = 15f; // reset timer for the next stage
                    }
                }
            }

            // show reminder at 30 seconds, but only while the plant still needs watering
            if (totalLifetime >= 30f && !watered && (currentState == PlantState.Planted || currentState == PlantState.Growing))
            {
                if (waterReminderIcon)
                    waterReminderIcon.SetActive(true);
            }

            // wither after 60 seconds if not watered
            if (totalLifetime >= 60f && !watered)
            {
                ChangeState(PlantState.Withered);
>>>>>>> 7545cdd (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
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

            // hide water reminder if plant is Mature or Withered, too late to water lol
            if (waterReminderIcon != null && (currentState == PlantState.Mature || currentState == PlantState.Withered))
            {
                waterReminderIcon.SetActive(false);
            }
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
