using UnityEngine;
using Core;

// this script is for handling scene transitions
// note: this script only works with the assumption that triggers are set by box colliders that check for "Player" collisions

public class SceneHandler : MonoBehaviour
{
    // the destination scene
    [SerializeField] private string sceneName;
    // the name of the spawnPoint our character will load at
    [SerializeField] private string spawnPointName; 

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if the player enters
        if (other.CompareTag("Player"))
        {
            Debug.Log("scene trigger was made");
            // setting the name of our desired spawnPoint
            GameManager.Instance.pendingSpawnPoint = spawnPointName;

            // call the function to load the scene
            GameManager.Instance.LoadScenebyName(sceneName);
        }
    }
}