using UnityEngine;
using UnityEngine.InputSystem;
using Farming;

namespace Character 
{
    [RequireComponent(typeof(PlayerInput))] // Input is required and we don't store a reference
    [RequireComponent(typeof(Farmer))] //added from Al's

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TileSelector tileSelector;
        [SerializeField] private Transform holdPosition;
        [SerializeField] private float equipRange = 3f;

        private EquippableItem equippedItem;
    
        private MovementController moveController;
        private AnimatedController animatedController;

        Farmer farmer; // added by Al
   
        void Start()
        {
            farmer = GetComponent<Farmer>(); // added by Al
            moveController = GetComponent<MovementController>();
            animatedController = GetComponent<AnimatedController>();

            Debug.Assert(animatedController, "PlayerController requires an animatedController");
            Debug.Assert(moveController, "PlayerController requires a MovementController");
            Debug.Assert(tileSelector, "PlayerController requires a TileSelector.");
        }

        public void OnMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            moveController.Move(inputVector);
        }

        public void OnJump(InputValue inputValue) {moveController.Jump();}

        private EquippableItem FindItem()
        {   
            // Find all colliders within equipRange
            Collider[] hits = Physics.OverlapSphere(transform.position, equipRange);

            foreach (Collider hit in hits)
            {
                if(hit.CompareTag("Equippable"))
                {
                    EquippableItem item = hit.GetComponent<EquippableItem>();

                    if(item != null) {return item;}

                }
            }

            return null;
        
        }

        private void TryEquip()
        {
            EquippableItem itemToEquip = FindItem();
            if(itemToEquip != null)
            {
                equippedItem = itemToEquip;
                itemToEquip.OnEquip(holdPosition);

            }
            
        }

        public void OnEquip(InputValue value)
        {
            Debug.Log("equip pressed");
            if(!value.isPressed) {return;}
            
            
            if(value.isPressed && equippedItem == null)
            {
                TryEquip();
            } 
            else
            {
                DropItem();
            }
        }

        public void DropItem()
        {
            if(equippedItem == null) {return;}

            Vector3 dropPos = transform.position + transform.forward * 0.6f + transform.up * 0.2f;
            equippedItem.OnDrop(dropPos);

            equippedItem = null;
        }
        
        public void OnInteract(InputValue value)
        {
            Debug.Log("interact button pressed");
            FarmTile tile = tileSelector.GetSelectedTile();
            farmer.TryTileInteraction(tile);
        }

        // note: I (Ryan) changed the Interact() function a little bit from the source, which is why this *almost* works
        // public void OnInteract(InputValue value)
        // {
        //     Debug.Log("interact button pressed");
        //     FarmTile tile = tileSelector.GetSelectedTile();
        //     // "if the tile is NOT null (implies that it can still change states), change its state to the next one"
        //     if (tile != null)
        //     {
        //         // note: I've changed Interact() so it's a string function that returns the name of the action the player needs to do: till --> water --> nothing
        //         string animationTrigger = tile.Interact();
        //         farmer.TryTileInteraction(tile);
        //         // "if the action was NOT null (implies the player can either till or water a tile), set that action trigger"
        //         if(!string.IsNullOrEmpty(animationTrigger)) {animatedController.SetTrigger(animationTrigger);}

        //     }
        // }
    }
}