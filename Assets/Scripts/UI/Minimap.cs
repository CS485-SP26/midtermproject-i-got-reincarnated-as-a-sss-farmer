using UnityEngine;

public class Minimap : MonoBehaviour
{
    private static Minimap instance;
    
    public Transform player;

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

        // this line lets the minimap rotate based on if the player turns
        // commented it out, I like the way the minimap moves when it's not turning with player movement, if you wanna keep it, just uncomment the line -Salvador
        //transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}