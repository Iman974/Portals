using UnityEngine;

public class PortalTeleporter2 : MonoBehaviour {

    [SerializeField] private Transform leftDestination = null;
    [SerializeField] private Transform rightDestination = null;
    [SerializeField] private float minTeleportDistance = 0.1f;
    public bool pauseAppDebug;

    private bool isPlayerOverlapping;
    private Transform player;

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
        if (isPlayerOverlapping) {
            Vector3 toPlayer = player.position - transform.position;

            Transform receiver = Vector3.Dot(transform.right, toPlayer) > 0f ? rightDestination : leftDestination;

            float sqrShortestDistanceToPlayer = (toPlayer - Vector3.ProjectOnPlane(toPlayer, transform.right)).sqrMagnitude;
            if (sqrShortestDistanceToPlayer < minTeleportDistance * minTeleportDistance) {
                Debug.Log("Aborted teleport because too close: " + Mathf.Sqrt(sqrShortestDistanceToPlayer));
                isPlayerOverlapping = false;
                return;
            }

            if (pauseAppDebug) {
                UnityEditor.EditorApplication.isPaused = true;
                Debug.Log(receiver == rightDestination ? "Right" : "Left");
                return;
            }
            player.position = toPlayer + receiver.position;
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
