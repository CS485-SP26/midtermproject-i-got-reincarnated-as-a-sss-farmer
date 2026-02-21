using UnityEngine;

/// <summary>
/// Add this to any UI GameObject (like a Canvas parent) to make it persist across scenes.
/// Useful for GUI elements that should stay visible when transitioning between scenes.
/// </summary>
public class UIPersistence : MonoBehaviour
{
    [Header("Persistence Settings")]
    [SerializeField] private bool persistAcrossScenes = true;
    [SerializeField] private bool useSingleton = true; // Prevent duplicates

    private static UIPersistence instance;

    void Awake()
    {
        if (!persistAcrossScenes)
            return;

        if (useSingleton)
        {
            // Singleton pattern - only one instance allowed
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log($"[UIPersistence] {gameObject.name} will persist across scenes");
            }
            else
            {
                Debug.Log($"[UIPersistence] Destroying duplicate {gameObject.name}");
                Destroy(gameObject);
            }
        }
        else
        {
            // Just persist without singleton check
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[UIPersistence] {gameObject.name} will persist across scenes");
        }
    }
}
