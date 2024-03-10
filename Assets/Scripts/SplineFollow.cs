//This script allows an object to follow along a spline using unitys physics.
//It is not perfect but works pretty good

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineFollow : MonoBehaviour
{
    Rigidbody rb;
    GameObject car;
    [SerializeField] SplineContainer track;
    // [SerializeField] GameObject car;
    public float directionSwitchThreshold = 0.3f;
    public float velocityDampeningThreshold = 2.5f;
    public float velocityDampeningRate = 0.8f;
    public float stopMinAngleThreshold = 12f;
    public bool isFollowingSpline = true;

    //Information about our object.
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float velocityMagnitude;
    [SerializeField] private float direction;
    [SerializeField] private float angle;
    [SerializeField] private bool movingBackward = false;

    private void Start()
    {
        car = gameObject;
        rb = car.GetComponent<Rigidbody>();
    }

    int resolution = 4;
    int iterations = 2;

    //We make sure to update our position using update() because it is called every frame.
    void Update()
    {
        if (isFollowingSpline) {
            var native = new NativeSpline(track.Spline);
            float distance = SplineUtility.GetNearestPoint(native, car.transform.position, out float3 nearest,
            out float t,resolution,iterations);

            car.transform.position = nearest;

            Vector3 forward = Vector3.Normalize(track.EvaluateTangent(t));
            Vector3 up = track.EvaluateUpVector(t);

            var remappedForward = new Vector3(0,0,1);
            var remappedUp = new Vector3(0,1,0);
            var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward,remappedUp));

            car.transform.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;
        }
    }

    //We update the physics with the physics engine on fixedupdate.
    void FixedUpdate()
    {
        if (isFollowingSpline) {
            Vector3 engineForward = car.transform.forward;

            direction = Vector3.Dot(rb.velocity,engineForward);

            if (movingBackward)
            {
                if (direction > -directionSwitchThreshold)
                {
                    movingBackward = false;
                }
            }
            else
            {
                if (direction < directionSwitchThreshold)
                {
                    movingBackward = true;
                }
            }

            if (movingBackward)
            {
                engineForward *= -1;
            }
            // Check if the car's up direction is aligned with the world up direction (typically the ground)
            angle = Vector3.Angle(car.transform.up, Vector3.up);

            // If our velocity is low enough lets start to scale it down to a stop. Need to check if we are on a slope.
            if (rb.velocity.magnitude < velocityDampeningThreshold && angle < stopMinAngleThreshold)
            {
                rb.velocity = rb.velocity.magnitude * engineForward;
                rb.velocity *= velocityDampeningRate;
            }
            else
            {
                rb.velocity = rb.velocity.magnitude * engineForward;
            }
            velocity = rb.velocity;
            velocityMagnitude = rb.velocity.magnitude;
        }
    }
}
