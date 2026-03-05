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

        [Header("Watering Plant Logic")]
        public GameObject waterReminderIcon;

        private float totalLifetime;
        private bool waterable = false;

        private bool watered = false;
        private int growthStagesAutoGrow;
        private int autoGrownStages = 0;



    // Here's what I'm trying to go for with the plant growth logic, copied from my message to Ryan:
    // I think maybe when you first plant, for the first 15 seconds you can’t add water, and the plant can grow 1-2 stages on its own.
    // Maybe 15 seconds per stage, but will require extra water to reach the final stage. If it isn’t watered after a minute, it’ll wither. 
    // And maybe after the 30 second mark, we spawn like a water droplet png on top of the plant to remind it to be watered, like zen garden in pvz.
    // - Salvador

        void Start()
        {
<<<<<<< HEAD
            Initialize();
        }
        
        public void Initialize()
        {
            timer = timeToNextStage;
            currentState = PlantState.Planted; // Ensure we start in Planted state
=======
            
            //timer = timeToNextStage;
            //UpdatePlantVisuals();
            currentState = PlantState.Planted;
            timer = 15f;
            totalLifetime = 0f;

            // random to grow 1 or 2 stages by itself before requiring watering, wanted RNG
            growthStagesAutoGrow = Random.Range(1, 3);
            autoGrownStages = 0;

>>>>>>> 553f965 (Small change to Part 10 and additions for Part 11)
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
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> d3dc40d (Modified Planting & Harvesting Logic [RM])
=======
>>>>>>> 8ac7de1 (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
=======
<<<<<<< HEAD
>>>>>>> 379e7c04d510f4bd23239b6446b07e47b5459184
            // Only grow if the plant has been watered and hasn't reached Mature or Withered yet
            if (isWatered && (currentState == PlantState.Planted || currentState == PlantState.Growing))
=======
            // changed so the plants can now wither (will be needed as withered plants shouldn't increment the plantInventory counter)
            if (currentState == PlantState.Planted || currentState == PlantState.Growing || currentState == PlantState.Mature)
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======

            totalLifetime += Time.deltaTime;

            // first 15 seconds, cannot water plant, let it grow naturally
            if (totalLifetime >= 15)
=======
>>>>>>> d3dc40d (Modified Planting & Harvesting Logic [RM])
=======
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
>>>>>>> 379e7c04d510f4bd23239b6446b07e47b5459184
            {
                waterable = true;
            }

            // only auto-grows for the randomly selected number of stages, then requires watering
            if (currentState == PlantState.Planted || currentState == PlantState.Growing)
>>>>>>> 553f965 (Small change to Part 10 and additions for Part 11)
            {
<<<<<<< HEAD
                if (autoGrownStages < growthStagesAutoGrow)
=======
                timer -= Time.deltaTime;

                if (timer <= 0f && (autoGrownStages < growthStagesAutoGrow || watered))
<<<<<<< HEAD
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
=======
>>>>>>> 379e7c04d510f4bd23239b6446b07e47b5459184
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
<<<<<<< HEAD
=======
=======
>>>>>>> 17a061b0211aa51644236eff296492aada7209ff
            // Only grow if the plant has been watered and hasn't reached Mature or Withered yet
            if (isWatered && (currentState == PlantState.Planted || currentState == PlantState.Growing))
=======
            // changed so the plants can now wither (will be needed as withered plants shouldn't increment the plantInventory counter)
            if (currentState == PlantState.Planted || currentState == PlantState.Growing || currentState == PlantState.Mature)
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])
            {
                waterable = true;
            }

            // only auto-grows for the randomly selected number of stages, then requires watering
            if (currentState == PlantState.Planted || currentState == PlantState.Growing)
            {
<<<<<<< HEAD
                if (autoGrownStages < growthStagesAutoGrow)
=======
                timer -= Time.deltaTime;

                if (timer <= 0f && (autoGrownStages < growthStagesAutoGrow || watered))
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
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
>>>>>>> 379e7c04d510f4bd23239b6446b07e47b5459184

            // show reminder at 30 seconds, but only while the plant still needs watering
            if (totalLifetime >= 30f && !watered && (currentState == PlantState.Planted || currentState == PlantState.Growing))
            {
                if (waterReminderIcon)
                    waterReminderIcon.SetActive(true);
            }

            // wither after 60 seconds if not watered and not yet mature
            if (totalLifetime >= 60f && !watered &&
                currentState != PlantState.Mature &&
                currentState != PlantState.Withered)
            {
                ChangeState(PlantState.Withered);
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
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
                if (autoGrownStages < growthStagesAutoGrow)
=======
                timer -= Time.deltaTime;

                if (timer <= 0f && (autoGrownStages < growthStagesAutoGrow || watered))
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
=======
                if (autoGrownStages < growthStagesAutoGrow)
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
                if (autoGrownStages < growthStagesAutoGrow)
>>>>>>> 379e7c04d510f4bd23239b6446b07e47b5459184
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

<<<<<<< HEAD
<<<<<<< HEAD
            // wither after 60 seconds if not watered and not yet mature
            if (totalLifetime >= 60f && !watered &&
                currentState != PlantState.Mature &&
                currentState != PlantState.Withered)
=======
            // wither after 60 seconds if not watered
            if (totalLifetime >= 60f && !watered)
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
            {
                ChangeState(PlantState.Withered);
=======
            // wither after 60 seconds if not watered
            if (totalLifetime >= 60f && !watered)
            {
                ChangeState(PlantState.Withered);
<<<<<<< HEAD
=======
            }
>>>>>>> 4241b9e09a5b44f57f7961716adeddcec728b790
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
>>>>>>> 17a061b0211aa51644236eff296492aada7209ff
>>>>>>> 379e7c04d510f4bd23239b6446b07e47b5459184
>>>>>>> 7545cdd (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
            }
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> 379e7c04d510f4bd23239b6446b07e47b5459184
        public bool TryWater()
        {
            // for when you can't water in the first 15s or already watered or withered
            if (!waterable || watered || currentState == PlantState.Withered || currentState == PlantState.Mature)
            {
                return false;
  
            }

            watered = true;

            // hide the droplet indicator
            if (waterReminderIcon){
                waterReminderIcon.SetActive(false);
            }

            if (currentState == PlantState.Planted)
            {
                ChangeState(PlantState.Growing);
            }
    
            else if (currentState == PlantState.Growing)
            {
                ChangeState(PlantState.Mature);
            }

            Debug.Log($"[Plant] {gameObject.name} has been watered!");

            return true;
        }       

<<<<<<< HEAD
=======
=======
>>>>>>> 17a061b0211aa51644236eff296492aada7209ff
>>>>>>> 379e7c04d510f4bd23239b6446b07e47b5459184
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
