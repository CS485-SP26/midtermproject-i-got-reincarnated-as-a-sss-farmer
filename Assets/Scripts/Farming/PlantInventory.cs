using UnityEngine;
<<<<<<< HEAD
using System;

namespace Farming
{
    public class PlantInventory : MonoBehaviour
{
    [SerializeField] private int plants = 0;

    public int PlantCount => plants;   // ✅ FIX

    public void AddPlant(int amount = 1)
    {
        plants += Mathf.Max(1, amount);
    }

    public bool TryRemovePlants(int amount)
    {
        if (plants < amount) return false;
        plants -= amount;
        return true;
    }
}
}
=======

namespace Farming
{
    public class PlantInventory : MonoBehaviour
{
    [SerializeField] private int plants = 0;

    public int PlantCount => plants;   // ✅ FIX

    public void AddPlant(int amount = 1)
    {
        plants += Mathf.Max(1, amount);
    }

    public bool TryRemovePlants(int amount)
    {
        if (plants < amount) return false;
        plants -= amount;
        return true;
    }
}
}
>>>>>>> d01161f (added the sell desk to the shop scene and edited the ShopPodium.cs script to allow selling plants based off plant inventory count which is gathered from PlantInventory.cs which should be attached to player. PlantCount is increased from FarmTile.cs)
