using UnityEngine;


// this script simply preserves the UI from the main scene
// note: this was made as its own script so other UI can be preserved as well
public class PersistentUI : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}