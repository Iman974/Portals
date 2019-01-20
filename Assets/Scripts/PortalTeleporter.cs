using UnityEngine;

public class PortalTeleporter : MonoBehaviour {

    [SerializeField] Transform receiver = null;
    //[SerializeField] Transform rightReceiver = null;
    //[SerializeField] float minTeleportDistance = 0.23f;
    //[SerializeField] bool doLeftDistanceCheck = true;
    //[SerializeField] bool doRightDistanceCheck = false;
    private bool isPlayerOverlapping;
    private Transform player;

    public bool pauseAppDebug;

    public static System.Action<Transform> OnPlayerTeleportEvent;

    private void Awake() {
        enabled = true; //Debug
    }

    private void Start() {
        player = GameObject.FindWithTag("GameController")?.transform;
        if (player == null) {
            Debug.LogWarning("Player transform could not be found.");
        }
    }

    private void FixedUpdate() {
        if (!isPlayerOverlapping) {
            return;
        }

        Vector3 toPlayer = player.position - transform.position;

        if (!(Vector3.Dot(toPlayer, transform.right) > 0f)) {
            isPlayerOverlapping = false;
            return;
        }

        if (pauseAppDebug) {
            UnityEditor.EditorApplication.isPaused = true;
        }

        //if ((side == Side.Right && doRightDistanceCheck) || (side == Side.Left && doLeftDistanceCheck)) {
        //    // cf unity source of projectonplane -> optimize by removing "toPlayer -"
        //    float sqrShortestDistanceToPlayer = (toPlayer - Vector3.ProjectOnPlane(toPlayer, transform.right)).sqrMagnitude;
        //    if (sqrShortestDistanceToPlayer < minTeleportDistance * minTeleportDistance) {
        //        Debug.Log("Aborted teleport because too close: " + Mathf.Sqrt(sqrShortestDistanceToPlayer));
        //        isPlayerOverlapping = false;
        //        return;
        //    }
        //}
        player.position = toPlayer + receiver.position + transform.localPosition - receiver.localPosition;
        OnPlayerTeleportEvent?.Invoke(receiver.parent);
        isPlayerOverlapping = false;
    }

    private void OnTriggerEnter(Collider other) {
        isPlayerOverlapping = true;
    }

    private void OnTriggerExit(Collider other) {
        isPlayerOverlapping = false; // not needed ?
    }
}
