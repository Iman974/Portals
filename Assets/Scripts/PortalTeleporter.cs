using UnityEngine;

public class PortalTeleporter : MonoBehaviour {

    [SerializeField] private Transform receiver = null;
    //[SerializeField] private float minTeleportDistance = 0.1f;
    public bool pauseAppDebug;

    private bool isPlayerOverlapping;
    private Transform player;
    //private Vector3 normal;

    public static System.Action<Transform> OnPlayerTeleportEvent;

    private void Start() {
        //normal = transform.right;
        player = GameObject.FindWithTag("GameController")?.transform;
        if (player == null) {
            Debug.LogWarning("Player transform could not be found.");
        }
    }

    private void FixedUpdate() {
        if (isPlayerOverlapping) {
            Vector3 toPlayer = player.position - transform.position;

            bool isPlayerFacingPortal = Vector3.Dot(transform.right, toPlayer) < 0f;
            if (isPlayerFacingPortal) {
                //float sqrShortestDistanceToPlane = (toPlayer - Vector3.ProjectOnPlane(toPlayer, normal)).sqrMagnitude;
                //if (sqrShortestDistanceToPlane < minTeleportDistance * minTeleportDistance) {
                //    Debug.Log("Aborted teleport because too close: " + Mathf.Sqrt(sqrShortestDistanceToPlane));
                //    isPlayerOverlapping = false;
                //    return;
                //}
            } else {
                isPlayerOverlapping = false;
                return;
            }
            if (pauseAppDebug) {
                UnityEditor.EditorApplication.isPaused = true;
                return;
            }
            player.position = toPlayer + receiver.position + transform.localPosition - receiver.localPosition;
            OnPlayerTeleportEvent?.Invoke(receiver.parent);
            isPlayerOverlapping = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        isPlayerOverlapping = true;
    }

    private void OnTriggerExit(Collider other) {
        isPlayerOverlapping = false; // not needed ?
    }
}
