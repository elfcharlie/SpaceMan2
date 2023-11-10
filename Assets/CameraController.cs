using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    private Vector3 offset = new Vector3(0f, 1f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;

    // Update is called once per frame
    void Update()
    {
        targetPosition = new Vector3(0f, target.position.y + offset.y, target.position.z + offset.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
