using UnityEngine;
using UnityEngine.InputSystem;

namespace OrbitalRescue
{
    /// <summary>Desktop A1 walkthrough. Replace locomotion when the course introduces XR.</summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Transform cameraPivot;
        [SerializeField, Min(.1f)] private float walkSpeed = 2.5f;
        [SerializeField, Min(.1f)] private float sprintSpeed = 4.5f;
        [SerializeField, Min(.001f)] private float mouseSensitivity = .12f;
        [SerializeField, Min(1f)] private float stickLookSpeed = 120f;
        [SerializeField, Range(1f, 89f)] private float pitchLimit = 80f;
        [SerializeField] private float gravity = -15f;

        private CharacterController body;
        private InputActionAsset ownedActions;
        private InputActionMap gameplay;
        private InputAction move, mouseLook, stickLook, sprint, releaseCursor, captureCursor, reset;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private float pitch, verticalSpeed;
        private bool skipLook;

        public bool IsCaptured { get; private set; }

        private void Awake()
        {
            body = GetComponent<CharacterController>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
            if (inputActions == null || cameraPivot == null)
            {
                Debug.LogError("FirstPersonController needs its InputActionAsset and child camera assigned.", this);
                enabled = false;
                return;
            }
            // Own the enabled state: disabling one player never disables another instance's asset.
            ownedActions = Instantiate(inputActions);
            gameplay = ownedActions.FindActionMap("Gameplay", true);
            move = gameplay.FindAction("Move", true);
            mouseLook = gameplay.FindAction("MouseLook", true);
            stickLook = gameplay.FindAction("StickLook", true);
            sprint = gameplay.FindAction("Sprint", true);
            releaseCursor = gameplay.FindAction("ReleaseCursor", true);
            captureCursor = gameplay.FindAction("CaptureCursor", true);
            reset = gameplay.FindAction("Reset", true);
        }

        private void OnEnable()
        {
            if (gameplay == null) return;
            gameplay.Enable();
            SetCaptured(true);
        }

        private void Update()
        {
            if (releaseCursor.WasPressedThisFrame())
            {
                // Start toggles capture on a gamepad. Escape always frees the desktop cursor.
                SetCaptured(releaseCursor.activeControl?.device is Gamepad && !IsCaptured);
                return;
            }
            if (reset.WasPressedThisFrame())
            {
                ResetToSpawn();
                return;
            }
            if (!IsCaptured)
            {
                if (captureCursor.WasPressedThisFrame()) SetCaptured(true);
                return;
            }

            if (skipLook) skipLook = false;
            else
            {
                // Mouse delta already measures displacement; only the stick is scaled by time.
                Vector2 look = mouseLook.ReadValue<Vector2>() * mouseSensitivity
                    + stickLook.ReadValue<Vector2>() * (stickLookSpeed * Time.deltaTime);
                transform.Rotate(0, look.x, 0, Space.Self);
                pitch = Mathf.Clamp(pitch - look.y, -pitchLimit, pitchLimit);
                cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
            }

            Vector2 input = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f);
            Vector3 horizontal = transform.right * input.x + transform.forward * input.y;
            if (body.isGrounded && verticalSpeed < 0) verticalSpeed = -2f;
            verticalSpeed += gravity * Time.deltaTime;
            float speed = sprint.IsPressed() ? sprintSpeed : walkSpeed;
            body.Move((horizontal * speed + Vector3.up * verticalSpeed) * Time.deltaTime);
        }

        private void SetCaptured(bool captured)
        {
            IsCaptured = captured;
            Cursor.lockState = captured ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !captured;
            // A capture click or focus transition may produce a large one-frame mouse delta.
            skipLook = true;
        }

        private void ResetToSpawn()
        {
            body.enabled = false;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            cameraPivot.localRotation = Quaternion.identity;
            pitch = verticalSpeed = 0;
            body.enabled = true;
            skipLook = true;
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) SetCaptured(false);
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) SetCaptured(false);
        }

        private void OnDisable()
        {
            gameplay?.Disable();
            SetCaptured(false);
        }

        private void OnDestroy()
        {
            if (ownedActions != null) Destroy(ownedActions);
        }
    }
}
