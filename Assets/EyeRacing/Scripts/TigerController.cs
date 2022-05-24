using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartControl;

public class TigerController : MonoBehaviour
{
    public float maxAccelerationTowards = 10.0f;
    public float maxAccelerationAway = 20.0f;
    public float maxAccelerationAlongside = 10.0f;
    public float maxBrake = 40.0f;
    public float maxSpeedTowards = 40.0f;
    public float startSeekingInitialDistance = 200.0f;
    public float startFollowingDistance = 30.0f;
    public float startRunningAwayDistance = 5.0f;
    public float stopRunningAwayDistance = 15.0f;
    public float idealXOffsetDistance = -5.0f;
    public float idealXOffsetTolerance = 1.0f;
    public float keepAheadDistance = 15.0f;
    public float keepAheadTolerance = 3.0f;
    public float startSeekingAgainDistance = 80.0f;

    public Vector3 acceleration;
    public Vector3 newVel;

    private ArcadeKart target;
    private Animator animator;
    private Rigidbody _rigidbody;

    void Awake() {
        target = FindObjectOfType<ArcadeKart>();
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        if (target != null && Time.timeScale != 0) {
            float targetSpeed = target.GetMaxSpeed();
            Vector3 diff = transform.position - target.transform.position;
            diff.y = 0;
            float sqDistance = diff.sqrMagnitude;
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(0);
            nextState(targetSpeed, diff, sqDistance, state, transition);
            move(targetSpeed, diff, sqDistance, state, transition);
        }
    }

    public void nextState(float targetSpeed, Vector3 diff, float sqDistance, AnimatorStateInfo state, AnimatorTransitionInfo transition) {
        if (sqDistance < startRunningAwayDistance * startRunningAwayDistance) {
            animator.SetTrigger("Away");
        } else if (state.IsName("idle")) {
            if (sqDistance <= startSeekingInitialDistance * startSeekingInitialDistance) {
                animator.SetTrigger("Seek");
            }
        } else if (sqDistance > startSeekingAgainDistance * startSeekingAgainDistance) {
            animator.SetTrigger("Seek");
        } else if (state.IsName("run toward") || state.IsName("walk toward")) {
            if (sqDistance <= startFollowingDistance * startFollowingDistance) {
                animator.SetTrigger("Stop");
            }
        } else if (state.IsName("run away") || animator.GetAnimatorTransitionInfo(0).IsName("ToRunAway")) {
            if (sqDistance > stopRunningAwayDistance * stopRunningAwayDistance) {
                animator.SetTrigger("Stop");
            }
        }
        animator.SetFloat("Speed", _rigidbody.velocity.magnitude);
    }

    private void move(float targetSpeed, Vector3 diff, float sqDistance, AnimatorStateInfo state, AnimatorTransitionInfo transition) {
        float finalVel = 0.0f;
        float accel = 1.0f;
        newVel = Vector3.zero;
        Vector3 velDir;
        Vector3 targetPos = target.transform.position + new Vector3(idealXOffsetDistance, 0, keepAheadDistance);
        Vector3 diffToTarget = targetPos - transform.position;
        if (state.IsName("run toward") || state.IsName("walk toward")) {
            accel = maxAccelerationTowards;
            if (diff.z < 0.0f) { finalVel = sqDistance + target.Rigidbody.velocity.magnitude; accel -= diff.z / 2f; }
            else finalVel = maxSpeedTowards;
            velDir = diffToTarget.normalized;
            newVel = Mathf.Min(finalVel, diff.z < 0.0f ? finalVel : maxSpeedTowards) * velDir;
        } else if (state.IsName("run away")) {
            finalVel = stopRunningAwayDistance * stopRunningAwayDistance * targetSpeed / sqDistance;
            velDir = diff.normalized;
            velDir.z = Mathf.Abs(velDir.z);
            accel = maxAccelerationAway;
            newVel = finalVel * velDir;
        } else if (state.IsName("run alongside") || state.IsName("walk alongside") || state.IsName("idle alongside")) {
            accel = maxAccelerationAlongside;
            if (Mathf.Abs(diffToTarget.x) < idealXOffsetTolerance && Mathf.Abs(diffToTarget.z) < keepAheadTolerance) {
                newVel = target.Rigidbody.velocity;
            } else {
                newVel = target.Rigidbody.velocity + diffToTarget;
                if (diff.z < 0) {
                    accel -= diff.z / 2f;
                }
            }
        }
        if (Vector3.Dot(newVel, _rigidbody.velocity) < 0f || finalVel * finalVel < _rigidbody.velocity.sqrMagnitude) accel = maxBrake;

        // Vector3 velDiff = newVel - _rigidbody.velocity;
        // acceleration = Vector3.ClampMagnitude(velDiff / Time.fixedDeltaTime, accel);
        // _rigidbody.AddForce(2.3f * acceleration, ForceMode.Acceleration);


        
        _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, new Vector3(newVel.x, _rigidbody.velocity.y, newVel.z), 2.35f * accel * Time.deltaTime);

        if (newVel != Vector3.zero) {
            Vector3 dir = (newVel + _rigidbody.velocity).normalized;
            dir.y = 0;
            if (dir != Vector3.zero && Vector3.Angle(transform.forward, dir) > 1f && _rigidbody.velocity.sqrMagnitude > 0.1f) {
                transform.forward = dir;
            }
        }
    }
}
