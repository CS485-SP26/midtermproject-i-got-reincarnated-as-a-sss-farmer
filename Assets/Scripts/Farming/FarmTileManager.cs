using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Environment;

namespace Farming
{
    public class FarmTileManager:MonoBehaviour
    {
        [SerializeField] private GameObject farmTilePrefab;
        [SerializeField] DayController dayController;
        [SerializeField] private PlayerEconomy playerEconomy;
        
        private const int AllWateredReward = 50;
        // Tracks tiles that made a fresh Tilled→Watered transition this cycle.
        // All tiles must do so before the reward fires. Cleared after award, so
        // players must water the entire field fresh every time.
        private System.Collections.Generic.HashSet<FarmTile> wateredThisCycle =
            new System.Collections.Generic.HashSet<FarmTile>();
        [SerializeField] private int rows = 4;
        [SerializeField] private int cols = 4;
        [SerializeField] private float tileGap = 0.1f;
        private List<FarmTile> tiles = new List<FarmTile>();
        
        void Start()
        {
            Debug.Assert(farmTilePrefab, "FarmTileManager requires a farmTilePrefab");
            Debug.Assert(dayController, "FarmTileManager requires a dayController");
        }

        void OnEnable()
        {
            dayController.dayPassedEvent.AddListener(this.OnDayPassed);
            FarmingEvents.OnTileFarmed += OnTileFarmed;
        }

        void OnDisable()
        {
            dayController.dayPassedEvent.RemoveListener(this.OnDayPassed);
            FarmingEvents.OnTileFarmed -= OnTileFarmed;
        }

        private void OnTileFarmed(FarmTile tile, FarmTile.Condition previous, FarmTile.Condition next)
        {
            // Only count a tile when it makes a fresh Tilled→Watered transition.
            // This prevents the exploit of watering the same tile repeatedly.
            if (previous == FarmTile.Condition.Tilled && next == FarmTile.Condition.Watered)
            {
                wateredThisCycle.Add(tile);
            }

            // If a tile regresses back to an un-watered state, it's no longer "done".
            if (next == FarmTile.Condition.Grass || next == FarmTile.Condition.Tilled)
            {
                wateredThisCycle.Remove(tile);
            }

            // Award only when every tile has been freshly watered this cycle.
            if (tiles.Count > 0 && wateredThisCycle.Count >= tiles.Count)
            {
                wateredThisCycle.Clear(); // Reset — player must water all tiles fresh again
                if (playerEconomy != null)
                {
                    playerEconomy.EarnMoney(AllWateredReward);
                    Debug.Log($"[FarmTileManager] All tiles freshly watered! Awarded ${AllWateredReward}.");
                }
                else
                {
                    Debug.LogWarning("[FarmTileManager] PlayerEconomy not assigned — cannot award funds.");
                }
            }
        }

        public void OnDayPassed()
        {
            IncrementDays(1);
        }

        public void IncrementDays(int count)
        {
            while (count > 0)
            {
                foreach (FarmTile farmTile in tiles)
                {
                    farmTile.OnDayPassed();
                }
                count--;
            }
        }

        void InstantiateTiles()
        {
            Vector3 spawnPos = transform.position;
            int count = 0;
            GameObject clone = null; 

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    clone = Instantiate(farmTilePrefab, spawnPos, Quaternion.identity);
                    clone.name = "Farm Tile " + count++.ToString();
                    spawnPos.x += clone.transform.localScale.x + tileGap;
                    clone.transform.parent = transform; // build heirarchy
                    tiles.Add(clone.GetComponent<FarmTile>()); // for resize/delete
                }
                spawnPos.z += clone.transform.localScale.z + tileGap;
                spawnPos.x = transform.position.x;
            }
        }

        // ***************************************************************** //
        // Below this line is code to suppor the Unity Editor (Advanced)
        // Please feel free to disregard everything below this
        // ***************************************************************** //
        void OnValidate()
        {
            #if UNITY_EDITOR
            EditorApplication.delayCall += () => {
                if (this == null) return; // Guard against the object being deleted
                ValidateGrid();
            };
            #endif
        }

        void ValidateGrid() 
        {
            if (!farmTilePrefab) return;
            tiles.Clear();
            foreach (Transform child in transform)
            {
                if (child.gameObject.TryGetComponent<FarmTile>(out var tile))
                {
                    tiles.Add(tile);
                }
            }

            int newCount = rows * cols;

            if (tiles.Count != newCount)
            {
                DestroyTiles();
                InstantiateTiles();
            }
        }

        void DestroyTiles()
        {
            foreach (FarmTile tile in tiles)
            {
                #if UNITY_EDITOR
                DestroyImmediate(tile.gameObject);
                #else
                Destroy(tile.gameObject);
                #endif
            }
            tiles.Clear();
        }
    }
}