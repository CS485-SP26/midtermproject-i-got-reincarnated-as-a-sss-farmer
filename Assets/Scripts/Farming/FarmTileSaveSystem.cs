using UnityEngine;
using System.Collections.Generic;

namespace Farming
{
    /// <summary>
    /// Saves and loads farm tile states when transitioning between scenes.
    /// Ensures tiles maintain their condition (grass, tilled, watered, planted) across scene loads.
    /// </summary>
    [System.Serializable]
    public class FarmTileStateData
    {
        public string tileName;
        public FarmTile.Condition condition;
        public int daysSinceInteraction;
    }

    public class FarmTileSaveSystem : MonoBehaviour
    {
        private static FarmTileSaveSystem instance;
        
        /// <summary>
        /// Check if instance exists without creating it (safe for OnDestroy)
        /// </summary>
        public static bool HasInstance => instance != null;
        
        public static FarmTileSaveSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("FarmTileSaveSystem");
                    instance = go.AddComponent<FarmTileSaveSystem>();
                    DontDestroyOnLoad(go);
                    Debug.Log("[FarmTileSaveSystem] Auto-created and persisting across scenes");
                }
                return instance;
            }
        }

        // Dictionary to store tile states by scene name
        private Dictionary<string, List<FarmTileStateData>> savedTileStates = new Dictionary<string, List<FarmTileStateData>>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Save all tile states in the current scene
        /// </summary>
        public void SaveTileStates(string sceneName, List<FarmTile> tiles)
        {
            if (tiles == null || tiles.Count == 0)
            {
                Debug.LogWarning($"[FarmTileSaveSystem] No tiles to save for scene {sceneName}");
                return;
            }

            List<FarmTileStateData> stateData = new List<FarmTileStateData>();

            foreach (FarmTile tile in tiles)
            {
                if (tile == null) continue;

                FarmTileStateData data = new FarmTileStateData
                {
                    tileName = tile.gameObject.name,
                    condition = tile.GetCondition
                };

                stateData.Add(data);
                Debug.Log($"[FarmTileSaveSystem] Saving {tile.gameObject.name}: {tile.GetCondition}");
            }

            savedTileStates[sceneName] = stateData;
            Debug.Log($"[FarmTileSaveSystem] Saved {stateData.Count} tile states for scene '{sceneName}'");
        }

        /// <summary>
        /// Load tile states for the current scene
        /// </summary>
        public void LoadTileStates(string sceneName, List<FarmTile> tiles)
        {
            if (!savedTileStates.ContainsKey(sceneName))
            {
                Debug.Log($"[FarmTileSaveSystem] No saved states found for scene '{sceneName}' - starting fresh");
                return;
            }

            List<FarmTileStateData> stateData = savedTileStates[sceneName];
            Debug.Log($"[FarmTileSaveSystem] Loading {stateData.Count} tile states for scene '{sceneName}'");
            
            // Match tiles by name and restore their state
            int restoredCount = 0;
            foreach (FarmTile tile in tiles)
            {
                if (tile == null) continue;

                FarmTileStateData data = stateData.Find(d => d.tileName == tile.gameObject.name);
                if (data != null)
                {
                    tile.SetCondition(data.condition);
                    restoredCount++;
                    Debug.Log($"[FarmTileSaveSystem] Restored {tile.gameObject.name} to {data.condition}");
                }
            }

            Debug.Log($"[FarmTileSaveSystem] Successfully restored {restoredCount}/{stateData.Count} tiles");
        }

        /// <summary>
        /// Check if there are saved states for a scene
        /// </summary>
        public bool HasSavedStates(string sceneName)
        {
            return savedTileStates.ContainsKey(sceneName);
        }

        /// <summary>
        /// Clear all saved tile states (useful for restarting)
        /// </summary>
        public void ClearAllStates()
        {
            savedTileStates.Clear();
            Debug.Log("[FarmTileSaveSystem] Cleared all saved tile states");
        }
    }
}
