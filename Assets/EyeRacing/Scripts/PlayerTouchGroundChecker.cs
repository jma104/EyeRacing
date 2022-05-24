using UnityEngine;
using UnityEngine.Events;

namespace KartControl {
    public class PlayerTouchGroundChecker : MonoBehaviour
    {
        public PlayerTouchGroundListener game;
        private ArcadeKart kart;
        private int groundLayer;
        private int trackLayer;
        private bool touchingGround = false;
        private bool touchingRoad = false;

        void Awake()
        {
            kart = GetComponent<ArcadeKart>();
            groundLayer = LayerMask.NameToLayer("Ground");
            trackLayer = LayerMask.NameToLayer("Track");
        }

        void Update()
        {
            if (kart == null || game == null) return;

            bool ground = false;
            bool road = false;
            if (kart.FrontLeftWheel.isGrounded && kart.FrontLeftWheel.GetGroundHit(out WheelHit hit)) {
                if (hit.collider.gameObject.layer == groundLayer) ground = true;
                else if (hit.collider.gameObject.layer == trackLayer) road = true;
            }
            if (kart.FrontRightWheel.isGrounded && kart.FrontRightWheel.GetGroundHit(out hit)) {
                if (hit.collider.gameObject.layer == groundLayer) ground = true;
                else if (hit.collider.gameObject.layer == trackLayer) road = true;
            }
            if (kart.RearLeftWheel.isGrounded && kart.RearLeftWheel.GetGroundHit(out hit)) {
                if (hit.collider.gameObject.layer == groundLayer) ground = true;
                else if (hit.collider.gameObject.layer == trackLayer) road = true;
            }
            if (kart.RearRightWheel.isGrounded && kart.RearRightWheel.GetGroundHit(out hit)) {
                if (hit.collider.gameObject.layer == groundLayer) ground = true;
                else if (hit.collider.gameObject.layer == trackLayer) road = true;
            }
            
            if (!touchingGround && ground) {
                game.OnPlayerEnterGround();
            } else if (touchingGround && !ground) {
                game.OnPlayerExitGround();
            } else if (!touchingRoad && road) {
                game.OnPlayerEnterRoad();
            } else if (touchingRoad && !road) {
                game.OnPlayerExitRoad();
            }
            touchingGround = ground;
            touchingRoad = road;
        }
    }
}
