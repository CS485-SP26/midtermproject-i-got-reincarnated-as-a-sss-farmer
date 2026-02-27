using UnityEngine;

public class Minimap : MonoBehaviour
{
    private static Minimap instance;
    
    public Transform player;
    [SerializeField] private bool rotateWithPlayer = false;

    void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;
        
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        if (rotateWithPlayer)
        {
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}