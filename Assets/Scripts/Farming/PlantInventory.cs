using UnityEngine;

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