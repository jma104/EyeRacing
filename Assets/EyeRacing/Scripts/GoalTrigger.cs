using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KartControl;

public class GoalTrigger : MonoBehaviour
{
    public UnityEvent PlayerEntered;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            
        }
    }
}
