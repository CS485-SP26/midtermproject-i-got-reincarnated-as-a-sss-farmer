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
        bool isRunning;

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

            Debug.Assert(moveController, "PlayerController requires a MovementController");
        }

        public void OnMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            moveController.Move(inputVector);
        }

        public void OnSprint(InputValue inputValue)
        {
            isRunning = inputValue.isPressed;
            physicsMovement?.SetRunning(isRunning);
        }

        public void OnJump(InputValue inputValue)
        {
            moveController.Jump();
        }
    }
}
