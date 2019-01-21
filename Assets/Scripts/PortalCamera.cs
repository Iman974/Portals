using UnityEngine;

public class PortalCamera : MonoBehaviour {

    [SerializeField] private Transform selfPortal = null;
    [SerializeField] private Transform startOtherPortal = null;

    private Transform playerCamera;
    private Quaternion portalRotationalDifference;
    private Transform referencePortal;

    private void Awake() {
        PortalTeleporter.OnPlayerTeleportEvent += OnPlayerTeleport;
    }

    private void Start() {
        //float angularDifferenceBetweenPortalRotations = Quaternion.Angle(selfPortal.rotation, referencePortal.rotation);
        //portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform;
        referencePortal = startOtherPortal;
    }

    private void LateUpdate() {
        transform.position = playerCamera.position - referencePortal.position + selfPortal.position;

        //Vector3 newCameraDirection = portalRotationalDifference * playerCamera.forward;
        transform.rotation = playerCamera.rotation;//Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }

    private void OnPlayerTeleport(Transform newReferencePortal) {
        referencePortal = newReferencePortal;
    }

    private void OnDestroy() {
        PortalTeleporter.OnPlayerTeleportEvent -= OnPlayerTeleport;
    }
}
