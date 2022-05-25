using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using Tobii.Gaming;

public class PlayerTouchGroundListener : MonoBehaviour
{
    public PostProcessVolume effect;
    private GameStatus gameStatus;
    public float effectDurationOn = 0f;
    public float effectDurationOff = 0f;
    public float maxIntensity = 1f;
    public float pulsationIntensity = 0.1f;
    public float pulsationTime = 0.5f;
    private bool isTouchingGround = false;
    private bool isTouchingRoad = false;
    private bool isIncreasing = false;
    private bool isDecreasing = false;
    private bool isEffectActive = false;  // Either increasing or pulsing but not decreasing.
    private float effectEndTime = 0f;
    private float lastChangeTime = 0f;
    private float timeDiff = 0f;
    private float pct = 0f;
    public UnityEvent PlayerStartedtouchingGround;
    public UnityEvent PlayerStoppedtouchingGround;

    public void Awake() {
        gameStatus = GetComponent<GameStatus>();
        if (gameStatus == null) {
            Debug.LogWarning("GameStatus is null");
        }
    }

    public void LateUpdate() {
        if (effect != null) {
            if (isEffectActive && gameStatus != null && (gameStatus.won || gameStatus.lost)) {
                checkStartOrStopEffect();
            }
            Vignette vignette;
            effect.profile.TryGetSettings(out vignette);
            if (Time.timeScale == 0) {
                if (vignette.enabled) vignette.enabled.Override(false);
            } else {
                if (!vignette.enabled) vignette.enabled.Override(true);
                float currentTime = Time.time;
                if (currentTime < effectEndTime) {
                    float timeRemaining = effectEndTime - currentTime;
                    float duration = isIncreasing? effectDurationOn : effectDurationOff;
                    float pct_remaining = timeRemaining / duration;
                    pct = isDecreasing? pct_remaining : (1f - pct_remaining);
                    vignette.intensity.Override(pct * maxIntensity);
                } else {
                    isIncreasing = false;
                    isDecreasing = false;
                    if (isEffectActive && isTouchingGround) {
                        float diff = currentTime - effectEndTime;
                        float w = diff / pulsationTime;
                        float pct = Mathf.Sin(2f * Mathf.PI * w);
                        vignette.intensity.Override(maxIntensity - pct * pulsationIntensity);
                    }
                }
                if (isEffectActive || isDecreasing) {
                    Vector2 center = new Vector2(0.5f, 0.5f);
                    GazePoint gp = TobiiAPI.GetGazePoint();
                    if (gp.IsValid && gp.IsRecent()) {
                        center = gp.Viewport;
                    }
                    vignette.center.Override(center);
                }
            }
        }
    }

    private void startEffect() {
        isIncreasing = true;
        isEffectActive = true;
        bool wasDecreasing = isDecreasing;
        isDecreasing = false;
        float currentTime = Time.time;
        lastChangeTime = currentTime;
        if (effectEndTime < currentTime) {
            timeDiff = effectDurationOn;
        } else if (wasDecreasing) {
            float pct_done = (effectEndTime - currentTime) / effectDurationOff;
            timeDiff = (1f - pct_done) * effectDurationOn;
        } else {
            float pct_remaining = (effectEndTime - currentTime) / effectDurationOff;
            timeDiff = pct_remaining * effectDurationOn;
        }
        effectEndTime = currentTime + timeDiff;
        PlayerStartedtouchingGround?.Invoke();
    }
    private void stopEffect() {
        isEffectActive = false;
        isDecreasing = true;
        bool wasIncreasing = isIncreasing;
        isIncreasing = false;
        float currentTime = Time.time;
        if (effectEndTime < currentTime) {
            timeDiff = effectDurationOff;
        } else if (wasIncreasing) {
            float pct_done = (effectEndTime - currentTime) / effectDurationOn;
            timeDiff = (1f - pct_done) * effectDurationOff;
        } else {
            float pct_remaining = (effectEndTime - currentTime) / effectDurationOn;
            timeDiff = pct_remaining * effectDurationOff;
        }
        effectEndTime = currentTime + timeDiff;
        PlayerStoppedtouchingGround?.Invoke();
    }

    private void checkStartOrStopEffect() {
        if (!isEffectActive && isTouchingGround && (gameStatus == null || !gameStatus.won && !gameStatus.lost)) {
            startEffect();
        } else if (isEffectActive && ((isTouchingRoad && !isTouchingGround) || (gameStatus != null && (gameStatus.won || gameStatus.lost)))) {
            stopEffect();
        }
    }

    public void OnPlayerEnterRoad() {
        // Debug.Log("Player entered road");
        isTouchingRoad = true;
        checkStartOrStopEffect();
    }
    public void OnPlayerExitRoad() {
        // Debug.Log("Player exited road");
        isTouchingRoad = false;
        // checkStartOrStopEffect();
    }

    public void OnPlayerEnterGround() {
        // Debug.Log("Player entered ground");
        isTouchingGround = true;
        checkStartOrStopEffect();
    }
    public void OnPlayerExitGround() {
        // Debug.Log("Player exited ground");
        isTouchingGround = false;
        checkStartOrStopEffect();
    }
}
