using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string otherTag = "Player";
    public bool Checked {get; private set;} = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 1);
    private AudioSource sfx;

    public void Awake() {
        sfx = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(otherTag)) {
            Checked = true;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            sfx.Play();
        }
    }

    public void Update() {
        transform.Rotate(Time.deltaTime * rotationSpeed);
    }
}
