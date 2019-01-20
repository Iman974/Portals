using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings {
            public float ForwardSpeed = 7.0f;
            public float BackwardSpeed = 7.0f;
            public float StrafeSpeed = 7.0f;
            [HideInInspector] public float CurrentTargetSpeed = 8f;

            public void UpdateDesiredTargetSpeed(Vector2 input) {
                if (input == Vector2.zero) return;
                if (Mathf.Abs(input.x) > 0f) {
                    CurrentTargetSpeed = StrafeSpeed;
                }
                if (input.y < 0f) {
                    CurrentTargetSpeed = BackwardSpeed;
                }
                if (input.y > 0f) {
                    //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                    CurrentTargetSpeed = ForwardSpeed;
                }
            }
        }

        [Serializable]
        public class AdvancedSettings {
            public float groundCheckDistance = 0.01f;
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 1f; // rate at which the controller comes to a stop when there is no input
            public bool airControl;
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }

        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        private new Camera camera;
        private new Rigidbody rigidbody;
        private CapsuleCollider capsuleCollider;
        private float rotationY;
        private Vector3 groundContactNormal;
        private bool previouslyGrounded;
        private bool isGrounded;

        public Vector3 Velocity => rigidbody.velocity;

        private void Start() {
            rigidbody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            camera = GetComponentInChildren<Camera>();
            mouseLook.Init(transform, camera.transform);
        }

        private void Update() {
            RotateView();
        }

        private void FixedUpdate() {
            CheckGround();
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > 0f || Mathf.Abs(input.y) > 0f)) {
                if (!(advancedSettings.airControl || isGrounded)) {
                    return;
                }
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = (camera.transform.forward * input.y) + (camera.transform.right * input.x);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, groundContactNormal).normalized;

                desiredMove *= movementSettings.CurrentTargetSpeed;
                if (rigidbody.velocity.sqrMagnitude < (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed)) {
                    rigidbody.AddForce(desiredMove, ForceMode.Impulse);
                }
            } else {
                rigidbody.velocity *= advancedSettings.slowDownRate;
            }

            if (isGrounded) {
                rigidbody.drag = 5f;

                if (Mathf.Abs(input.x) < 0f && Mathf.Abs(input.y) < 0f && rigidbody.velocity.sqrMagnitude < 1f) {
                    rigidbody.Sleep();
                }
            } else {
                rigidbody.drag = 0f;
                if (previouslyGrounded) {
                    StickToGroundHelper();
                }
            }
        }

        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, capsuleCollider.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((capsuleCollider.height/2f) - capsuleCollider.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal);
                }
            }
        }

        private Vector2 GetInput() {
            Vector2 input = new Vector2 {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }

        private void RotateView() {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) {
                return;
            }

            // get the rotation before it's changed
            float oldRotationY = transform.eulerAngles.y;
            mouseLook.LookRotation(transform, camera.transform);

            if (isGrounded || advancedSettings.airControl) {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion rotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldRotationY, Vector3.up);
                rigidbody.velocity = rotation * rigidbody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void CheckGround() {
            previouslyGrounded = isGrounded;
            if (Physics.SphereCast(transform.position, capsuleCollider.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out RaycastHit hitInfo,
                                   ((capsuleCollider.height / 2f) - capsuleCollider.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                isGrounded = true;
                groundContactNormal = hitInfo.normal;
            } else {
                isGrounded = false;
                groundContactNormal = Vector3.up;
            }
        }
    }
}
