using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 5;
        [SerializeField] private float stickToGroundForce = 10;
        [SerializeField] private float gravityMultiplier = 3;
        [SerializeField] private MouseLook mouseLook = null;
        [SerializeField] private float stepInterval = 10;

        private Camera mainCamera;
        private float rotationY;
        private Vector2 input;
        private Vector3 moveDirection;
        private CharacterController characterController;
        private CollisionFlags collisionFlags;
        private bool previouslyGrounded;
        private Vector3 startCameraPosition;
        private float stepCycle;
        private float nextStep;

        private void Start() {
            characterController = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            startCameraPosition = mainCamera.transform.localPosition;
            mouseLook.Init(transform, mainCamera.transform);
        }

        private void Update() {
            RotateView();

            if (!previouslyGrounded && characterController.isGrounded) {
                moveDirection.y = 0f;
            }
            if (!characterController.isGrounded && previouslyGrounded) {
                moveDirection.y = 0f;
            }

            previouslyGrounded = characterController.isGrounded;
        }

        private void FixedUpdate() {
            GetInput();
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = (transform.forward * input.y) + (transform.right * input.x);

            // get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out RaycastHit hitInfo,
                               characterController.height * 0.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            moveDirection.x = desiredMove.x * walkSpeed;
            moveDirection.z = desiredMove.z * walkSpeed;

            if (characterController.isGrounded) {
                moveDirection.y = -stickToGroundForce;
            } else {
                moveDirection += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
            }
            collisionFlags = characterController.Move(moveDirection * Time.fixedDeltaTime);

            ProgressStepCycle(walkSpeed);

            mouseLook.UpdateCursorLock();
        }

        private void GetInput() {
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (input.sqrMagnitude > 1) {
                input.Normalize();
            }
        }

        private void ProgressStepCycle(float speed) {
            if (characterController.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0)) {
                stepCycle += (characterController.velocity.magnitude + speed) * Time.fixedDeltaTime;
            }

            if (stepCycle <= nextStep) {
                return;
            }

            nextStep = stepCycle + stepInterval;
        }

        private void RotateView() {
            mouseLook.LookRotation(transform, mainCamera.transform);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (collisionFlags == CollisionFlags.Below) {
                return;
            }

            if (body == null || body.isKinematic) {
                return;
            }
            body.AddForceAtPosition(characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
