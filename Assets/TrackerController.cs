using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerController : MonoBehaviour
{
    public Transform trackedObject;
    public float updateSpeed = 3f;
    public Vector2 trackingOffset;
    private Vector3 offset;
    private Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {
        offset = (Vector3)trackingOffset;
        offset.z = transform.position.z - trackedObject.position.z;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleTrackingMovement();
    }

    void HandleTrackingMovement()
    {
        newPos = new Vector3(0f, trackedObject.position.y + offset.y, trackedObject.position.z + offset.z);
        transform.position = Vector3.MoveTowards(transform.position, newPos, updateSpeed * Time.fixedDeltaTime);
    }
}
