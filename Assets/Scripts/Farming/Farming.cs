using UnityEngine;
using Character;

namespace Farming
{
    [RequireComponent(typeof(AnimatedController))]
    public class Farmer : MonoBehaviour
    {
        [SerializeField] private GameObject gardenHoe;
        [SerializeField] private GameObject waterCan;
        [SerializeField] private ProgressBar waterLevelUI; // eventually refactor this to a waterCan
        [SerializeField] private float waterLevel = 1f;
        [SerializeField] private float waterPerUse = 0.1f;
        AnimatedController animatedController;

        void Start()
        {
            Debug.Assert(waterCan, "Farmer requires a waterCan.");
            Debug.Assert(gardenHoe, "Farmer requires a gardenHoe.");
            Debug.Assert(waterLevelUI, "Farmer requires a water level");
            // SetTool("None");
            animatedController = GetComponent<AnimatedController>();
            waterLevelUI.SetText("Water Level");
            waterLevelUI.Fill = waterLevel;
        }

        // public void SetTool(string tool)
        // {
        //     waterCan.SetActive(false);
        //     gardenHoe.SetActive(false);
        //     switch (tool)
        //     {
        //         case "GardenHoe": gardenHoe.SetActive(true); break;
        //         case "WaterCan": waterCan.SetActive(true); break;
        //     }
        // }

        public void TryTileInteraction(FarmTile tile)
        {
            if (tile == null) {return;}

            switch (tile.GetCondition)
            {
                case FarmTile.Condition.Grass:
                    // triggers the tilling animation
                    animatedController.SetTrigger("Till");
                    // interacts with the tile
                    tile.Interact();
                    break;
                case FarmTile.Condition.Tilled:
                    if(waterLevel > waterPerUse)
                    {
                        animatedController.SetTrigger("Water");
                        tile.Interact();
                        waterLevel -= waterPerUse;
                        waterLevelUI.Fill = waterLevel;
                    }
                    break;

                default: break;
            }
        }
    }
}