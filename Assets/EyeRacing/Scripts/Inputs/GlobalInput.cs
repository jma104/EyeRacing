using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalInput : MonoBehaviour
{
    public UnityEvent pauseRequested;
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            pauseRequested.Invoke();
        }
    }
}
