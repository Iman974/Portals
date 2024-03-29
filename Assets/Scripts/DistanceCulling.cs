﻿using UnityEngine;

public class DistanceCulling : MonoBehaviour {

    [SerializeField] float minCullingDistance = 1f;

    private Transform mainCameraTransf;
    private Vector3 normal;
    private MeshRenderer selfRenderer;
    private float sqrMinCullingDistance;

    private void Start() {
        mainCameraTransf = GameObject.FindWithTag("MainCamera").transform;

        selfRenderer = GetComponent<MeshRenderer>();
        normal = transform.forward;
        sqrMinCullingDistance = minCullingDistance * minCullingDistance;
    }

    private void LateUpdate() {
        Vector3 toPlayer = mainCameraTransf.position - transform.position;
        float sqrShortestDistanceToPlane = (toPlayer - Vector3.ProjectOnPlane(toPlayer, normal)).sqrMagnitude;

        if (sqrShortestDistanceToPlane < minCullingDistance * minCullingDistance) {
            selfRenderer.enabled = false;
        } else {
            selfRenderer.enabled = true;
        }
    }
}
