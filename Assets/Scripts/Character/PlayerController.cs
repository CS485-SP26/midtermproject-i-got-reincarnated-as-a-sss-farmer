using UnityEngine;
using UnityEngine.InputSystem;

namespace Character 
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        private static PlayerController instance;
        
        MovementController moveController;
        PhysicsMovement physicsMovement;
        EnergyResource energyResource;
        bool isRunning;
        Vector2 currentMoveInput;

        // prevents Doozy from getting obliterated when moving across scenes
        void Awake()
        {
            // Singleton pattern to prevent duplicate players across scenes
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Destroy duplicate player that may be in the new scene
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            moveController = GetComponent<MovementController>();
            physicsMovement = GetComponent<PhysicsMovement>();
            energyResource = GetComponent<EnergyResource>();

            Debug.Assert(moveController, "PlayerController requires a MovementController");
            
            if (!energyResource)
                Debug.LogWarning("No EnergyResource found - sprinting will not drain energy");
        }

        void Update()
        {
            // Update energy resource with sprinting state
            if (energyResource != null)
            {
                bool isMoving = currentMoveInput.sqrMagnitude > 0.01f;
                energyResource.SetSprinting(isRunning, isMoving);
                
                // Stop sprinting if out of energy
                if (isRunning && !energyResource.HasEnergy)
                {
                    isRunning = false;
                    physicsMovement?.SetRunning(false);
                    Debug.Log("[PlayerController] Out of energy - stopped sprinting");
                }
            }
        }

        public void OnMove(InputValue inputValue)
        {
            currentMoveInput = inputValue.Get<Vector2>();
            moveController.Move(currentMoveInput);
        }

        public void OnRun(InputValue inputValue)
        {
            Debug.Log($"[PlayerController] OnRun called: isPressed={inputValue.isPressed}, hasEnergy={energyResource?.HasEnergy}");
            
            // Only allow sprinting if we have energy
            if (inputValue.isPressed && energyResource != null && !energyResource.HasEnergy)
            {
                Debug.Log("[PlayerController] Cannot sprint - no energy");
                return;
            }
            
            isRunning = inputValue.isPressed;
            physicsMovement?.SetRunning(isRunning);
            Debug.Log($"[PlayerController] Set running to: {isRunning}");
        }

        public void OnJump(InputValue inputValue)
        {
            moveController.Jump();
        }
    }
}
