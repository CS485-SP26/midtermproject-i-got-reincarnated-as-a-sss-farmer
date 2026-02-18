using UnityEngine;

// TODO: Consider the benefits of refactoring to namespace Movement
namespace Character
{
    public class PhysicsMovement : MovementController
    {
        [SerializeField] float drag = 0.5f;
        [SerializeField] float rotationSpeed = 0.1f;
        [SerializeField] float jumpForce = 4f;

        protected override void Start()
        {
            base.Start();
            rb.linearDamping = drag;
        }

        public override float GetHorizontalSpeedPercent()
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            return Mathf.Clamp01(horizontalVelocity.magnitude / maxVelocity);;
        }


        public override void Jump() 
        { 
            Debug.Log("Jump requested");
            jumpInput = true;
        }

        protected override void FixedUpdate()
        {
            ApplyMovement();
            ClampVelocity();
            ApplyRotation();
            ApplyJump();
        }
        
        void ApplyMovement()
        {
            Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
            movement *= Time.deltaTime * acceleration;
            rb.AddForce(movement, ForceMode.Force);
        }

        void ApplyJump()
        {

            // "if the jump input was detected, then apply that motion to the 'player'"
            if(jumpInput)
            {
                airborn = true;

                if(canJump)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    canJump = false;

                } 
                jumpInput = false;
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                canJump = true;
                airborn = false;
                jumpInput = false;

            }
        }

        void ClampVelocity()
        {
            // Clamp horizontal velocity while preserving vertical (for jumping/falling)
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            if (horizontalVelocity.magnitude > maxVelocity)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxVelocity;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        void ApplyRotation()
        {
            Vector3 direction = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (direction.magnitude > 0.5f)
            {
                // 1. Calculate the target rotation (where we WANT to look)
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

                // 2. Smoothly rotate from our current rotation toward the target
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }
    }
  }
