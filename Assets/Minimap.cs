using UnityEngine;

public class Minimap : MonoBehaviour
{

    public Transform player;

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        // this line lets the minimap rotate based on if the player turns
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}