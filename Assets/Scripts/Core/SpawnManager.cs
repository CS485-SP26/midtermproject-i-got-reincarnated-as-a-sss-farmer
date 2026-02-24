using UnityEngine;
using UnityEngine.SceneManagement;
using Core;
using Character;

// this script is for managing spawn locations when traversing through different scenes

// INSTRUCTIONS //
// - within the destination scene, drag this script onto an empty GameObject, as this will be our spawn point
// - set the transform to said GameObject
// - this version assumes the spawn point name is "ShopSpawn," however you can change this to whatever name (just make sure changes are universal both here & in SceneHandler.cs)
// - 

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform shopSpawn;
    [SerializeField] private string spawnPointName = "ShopSpawn";

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        var player = GameObject.FindGameObjectWithTag("Player");
        // "if our player loaded & a valid spawn point was detected, spawn the player there"
        if (player != null && GameManager.Instance.pendingSpawnPoint == spawnPointName) {
            // Ensure spawn point is valid before using it
            if (shopSpawn == null) {
                Debug.LogWarning($"[SpawnManager] shopSpawn is null in scene {scene.name}, skipping spawn positioning");
                GameManager.Instance.pendingSpawnPoint = null;
                return;
            }
            
            // Move player to spawn
            player.transform.position = shopSpawn.position;

            // finding the persistent camera dynamically
            var cameraFollow = FindFirstObjectByType<CameraFollow>();
            // "if our camera exists, assign the camera to the player after loading the new scene"
            if (cameraFollow != null) {
                cameraFollow.player = player;
                Debug.Log("CameraFollow assigned to player at runtime");

            }

            // reset the spawn point
            GameManager.Instance.pendingSpawnPoint = null;
        } // end of outside if-statement
    } // end of OneSceneLoaded
}