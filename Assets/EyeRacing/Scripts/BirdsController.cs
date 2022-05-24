using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdsController : MonoBehaviour
{
    public Transform target;
    public Vector3 variableDistanceFromTarget;
    public float startFollowingDistance = 100.0f;
    public float baseFlightSpeed = 400.0f;
    public float speedVariabilityHigh = 300.0f;
    public float speedVariabilityLow = 150.0f;
    private lb_Bird[] birds;

    void Awake()
    {
        birds = GetComponentsInChildren<lb_Bird>();
        foreach (lb_Bird bird in birds) {
            bird.gameObject.SetActive(true);
        }
    }

    void Start() {
        if (target != null) {
            StartCoroutine(findTargets());
        }
    }

    IEnumerator findTargets() {
        while (true) {
            foreach (lb_Bird bird in birds) {
                Vector3 diff = bird.transform.position - target.position;
                if (bird.followingTarget == Vector3.zero && diff.sqrMagnitude < startFollowingDistance * startFollowingDistance) {
                    // Vector3 offset = new Vector3(
                    //     Random.Range(-variableDistanceFromTarget.x, variableDistanceFromTarget.x),
                    //     Random.Range(0, variableDistanceFromTarget.y),
                    //     Random.Range(-variableDistanceFromTarget.z, variableDistanceFromTarget.z)
                    // );
                    bird.flightSpeed = Mathf.Max(0f, baseFlightSpeed + Random.Range(-speedVariabilityLow, speedVariabilityHigh));
                    bird.SendMessage("FlyToTarget", target.transform);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
