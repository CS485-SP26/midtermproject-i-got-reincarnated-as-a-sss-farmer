using Character;
using Farming;
using UnityEngine;


// note: AI was used in formatting & polishing RaycastSelector
public class RaycastSelector : TileSelector
{
    [SerializeField] private float rayDistance = 5f;

    void Update()
    {
        // making a new raycast given our position
        Ray ray = new Ray(transform.position, Vector3.down);
        // AI-recommended debugging
        // Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);

        // assuming the newTile is null at the start
        FarmTile newTile = null;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance))
        {
            hitInfo.collider.TryGetComponent(out newTile);
        }

        // the newTile becomes the activeTile (it gets selected)
        SetActiveTile(newTile);

    }
}

// RaycastSelector from video demo
/*
public class RaycastSelector : TileSelector
{
    [SerializeField] private float rayDistance = 5f;
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance))
        {
            if (hitInfo.collider.TryGetComponent<FarmTile>(out FarmTile tile))
            {
                SetActiveTile(tile);
            }
        }
        else
        {
            SetActiveTile(null);
        }
    }
}
*/