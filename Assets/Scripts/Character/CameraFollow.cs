using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Character
{
    public class CameraFollow : MonoBehaviour
    {
        private static CameraFollow instance;

        [Header("Target")]
        public Transform player;

        [Header("Camera Distance")]
        [SerializeField] private float distance = 7f;
        [SerializeField] private float height = 2.5f;

        [Header("Store Scene")]
        [SerializeField] private string storeSceneName = "Store";
        [SerializeField] private float storeDistance = 5f;
        [SerializeField] private float storeHeight = 2f;

        [Header("Mouse Control")]
        [SerializeField] private float mouseSensitivity = 0.18f;

        [Header("Keyboard Rotation")]
        [SerializeField] private float rotationSpeed = 120f;

        [Header("Zoom")]
        [SerializeField] private float minDistance = 3f;
        [SerializeField] private float maxDistance = 12f;
        [SerializeField] private float zoomSpeed = 3f;

        [Header("Pitch Limits")]
        [SerializeField] private float minPitch = -15f;
        [SerializeField] private float maxPitch = 65f;

        [Header("Smoothing")]
        [SerializeField] private float followSmoothTime = 0.12f;
        [SerializeField] private float rotationSmoothSpeed = 10f;

        [Header("Camera Collision")]
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float collisionRadius = 0.3f;
        [SerializeField] private float collisionOffset = 0.2f;

        private float yaw;
        private float pitch = 20f;

        private Vector3 velocity;

        public static CameraFollow Instance => instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LockCursor();
        }

        void Update()
        {
            HandleCursor();
        }

        void LateUpdate()
        {
            if (!player) return;

            HandleInput();
            UpdateCamera();
        }

        void HandleCursor()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                UnlockCursor();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }
        }

        void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void HandleInput()
        {
            // Stop camera movement if cursor is unlocked
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            if (Mouse.current != null)
            {
                Vector2 mouse = Mouse.current.delta.ReadValue();

                yaw += mouse.x * mouseSensitivity * 100f * Time.deltaTime;
                pitch -= mouse.y * mouseSensitivity * 100f * Time.deltaTime;

                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                float scroll = Mouse.current.scroll.ReadValue().y;

                if (scroll != 0)
                {
                    distance -= scroll * zoomSpeed * Time.deltaTime;
                    distance = Mathf.Clamp(distance, minDistance, maxDistance);
                }
            }

            var keyboard = Keyboard.current;

            if (keyboard != null)
            {
                if (keyboard.qKey.isPressed)
                    yaw -= rotationSpeed * Time.deltaTime;

                if (keyboard.eKey.isPressed)
                    yaw += rotationSpeed * Time.deltaTime;
            }
        }

        void UpdateCamera()
        {
            float currentDistance = distance;
            float currentHeight = height;

            if (SceneManager.GetActiveScene().name == storeSceneName)
            {
                currentDistance = storeDistance;
                currentHeight = storeHeight;
            }

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            Vector3 pivot = player.position + Vector3.up * currentHeight;

            Vector3 desiredPosition =
                pivot + rotation * new Vector3(0, 0, -currentDistance);

            RaycastHit hit;

            if (Physics.SphereCast(
                pivot,
                collisionRadius,
                (desiredPosition - pivot).normalized,
                out hit,
                currentDistance,
                collisionLayers))
            {
                float adjustedDistance = hit.distance - collisionOffset;

                desiredPosition =
                    pivot + rotation * new Vector3(0, 0, -adjustedDistance);
            }

            if (desiredPosition.y < player.position.y + 0.5f)
                desiredPosition.y = player.position.y + 0.5f;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                followSmoothTime
            );

            Quaternion targetRotation = Quaternion.LookRotation(
                (player.position + Vector3.up * 1.5f) - transform.position
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothSpeed * Time.deltaTime
            );
        }

        public Vector3 GetForward()
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            return forward.normalized;
        }

        public Vector3 GetRight()
        {
            Vector3 right = transform.right;
            right.y = 0;
            return right.normalized;
        }
    }
}