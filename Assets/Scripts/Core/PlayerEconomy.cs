using UnityEngine;
using System;

namespace Farming
{
    /// <summary>
    /// Tracks the player's money. Earns money every time a progress square fills up.
    /// </summary>
    public class PlayerEconomy : MonoBehaviour
    {
        private static PlayerEconomy instance;
        
        [Header("Economy Settings")]
        [SerializeField] private int startingMoney = 0;
    [SerializeField] private int moneyPerSquare = 10; // Money earned per completed progress square

    [Header("Money Earned Effects")]
    [SerializeField] private ParticleSystem confettiEffect; // Confetti particle effect
    [SerializeField] private AudioClip moneyEarnedSound;    // Sound to play when earning money
    [SerializeField] private AudioSource audioSource;       // Audio source for playing sounds

    private int currentMoney;

    /// <summary>Fires whenever money changes (newAmount)</summary>
    public static event Action<int> OnMoneyChanged;

    /// <summary>Fires when money is earned (amountEarned)</summary>
    public static event Action<int> OnMoneyEarned;

    public int CurrentMoney => currentMoney;
    public int MoneyPerSquare => moneyPerSquare;

    void Awake()
    {
        // If this is a standalone GameObject, make it persist
        if (transform.parent == null) // Only persist if it's a root GameObject
        {
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
        
        // Ensure we have an AudioSource component for playing sounds
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        currentMoney = startingMoney;
    }

    void Start()
    {
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void EarnMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"[Economy] Earned ${amount}! Total: ${currentMoney}");
        
        // Play confetti particle effect
        if (confettiEffect != null)
        {
            confettiEffect.Play();
        }
        
        // Play money earned sound
        if (audioSource != null && moneyEarnedSound != null)
        {
            audioSource.PlayOneShot(moneyEarnedSound);
        }
        
        OnMoneyEarned?.Invoke(amount);
        OnMoneyChanged?.Invoke(currentMoney);
    }

    /// <summary>
    /// Try to spend money. Returns true if successful.
    /// </summary>
    public bool TrySpend(int amount)
        {
            if (currentMoney >= amount)
            {
                currentMoney -= amount;
                Debug.Log($"[Economy] Spent ${amount}. Remaining: ${currentMoney}");
                OnMoneyChanged?.Invoke(currentMoney);
                return true;
            }

            Debug.Log($"[Economy] Not enough money! Need ${amount}, have ${currentMoney}");
            return false;
        }

        public void AddMoney(int amount)
        {
            EarnMoney(amount);
        }
    }
}
