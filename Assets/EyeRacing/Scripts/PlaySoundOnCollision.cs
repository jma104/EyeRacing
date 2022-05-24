using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public LayerMask collisionLayer;
    private AudioSource sfx;
    private float tolerance = 0.1f;

    public void Awake() {
        sfx = GetComponent<AudioSource>();
    }

    public void OnCollisionEnter(Collision collision) {
        // Debug.Log("Collision enter with " + collision.gameObject.tag + ", " + LayerMask.LayerToName(collision.gameObject.layer));
        if ((collisionLayer & 1<<collision.gameObject.layer) != 0) {
            Vector3 hitLocation = collision.contacts[0].point;
            float terrainHeight = Terrain.activeTerrain.SampleHeight(hitLocation);
            if (hitLocation.y - tolerance > terrainHeight) {
                // Debug.Log("Collision with tree");
                sfx.Play();
            }
            // else Debug.Log("Collision with Ground");
        }
    }
}
