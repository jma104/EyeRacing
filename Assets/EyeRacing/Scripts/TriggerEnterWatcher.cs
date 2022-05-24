using UnityEngine;
using UnityEngine.Events;

public class TriggerEnterWatcher : MonoBehaviour
{
    public string otherTag;
    public int requiredTouches = 1;
    private int touchCount = 0;
    public UnityEvent otherIn;
    public UnityEvent otherOut;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(otherTag)) {
            touchCount++;
            if (touchCount == requiredTouches)
                otherIn?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(otherTag)) {
            touchCount--;
            if (touchCount == requiredTouches-1)
                otherOut?.Invoke();
        }
    }
}
