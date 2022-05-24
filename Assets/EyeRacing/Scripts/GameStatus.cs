using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KartControl;

public class GameStatus : MonoBehaviour
{
    public ArcadeKart playerKart;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject pauseMenu;
    public GameObject checkpointContainer;

    public bool won { get; private set; } = false;
    public bool lost { get; private set; } = false;
    public float waitSecondsBeforeStart = 1f;
    public float maxTimeOnGround = 10.0f;

    private float gameStartTime = 0f;
    private float gameEndTime = 0f;
    private float totalGroundTime = 0f;
    private float groundStartTime = 0f;
    private float roadFocusedTime = 0f;
    private Checkpoint[] checkpoints;
    private EyeInput eyeInput;


    void Awake() {
        if (checkpointContainer == null) {
            checkpoints = new Checkpoint[0];
        } else {
            checkpoints = checkpointContainer.GetComponentsInChildren<Checkpoint>();
        }
        eyeInput = FindObjectOfType<EyeInput>();
    }

    void Start() {
        playerKart.SetCanMove(false);
        resumeTime();
        StartCoroutine(CountdownThenStartRaceRoutine());
    }

    IEnumerator CountdownThenStartRaceRoutine() {
        yield return new WaitForSeconds(waitSecondsBeforeStart);
        StartRace();
    }

    void StartRace() {
		playerKart.SetCanMove(true);
        gameStartTime = Time.time;
    }

    public void Win() {
        won = true;
        gameEndTime = Time.time;
        if (playerKart != null) {
            playerKart.SetCanMove(false);
        }
        if (winMenu != null) {
            WinMenuHandler winHandler = winMenu.GetComponent<WinMenuHandler>();
            int nCollected = 0;
            foreach (Checkpoint c in checkpoints) {
                if (c.Checked) nCollected++;
            }
            winMenu.SetActive(true);
            winHandler.SetScore(gameEndTime - gameStartTime, totalGroundTime, nCollected, checkpoints.Length, roadFocusedTime);
        }
    }

    public void Lose() {
        lost = true;
        gameEndTime = Time.time;
        if (playerKart != null) {
            playerKart.SetCanMove(false);
        }
        if (loseMenu != null) {
            WinMenuHandler loseHandler = loseMenu.GetComponent<WinMenuHandler>();
            int nCollected = 0;
            foreach (Checkpoint c in checkpoints) {
                if (c.Checked) nCollected++;
            }
            loseMenu.SetActive(true);
            loseHandler.SetScore(gameEndTime - gameStartTime, totalGroundTime, nCollected, checkpoints.Length, roadFocusedTime);
        }
    }

    void pauseTime() {
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    void resumeTime() {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void Pause() {
        if (pauseMenu != null && !won && !lost && !pauseMenu.activeInHierarchy) {
            pauseMenu.SetActive(true);
            pauseTime();
        }
    }

    public void Resume() {
        if (pauseMenu != null) {
            pauseMenu.SetActive(false);
            resumeTime();
        }
    }

    public void Quit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void ReloadScene() {
        resumeTime();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu() {
        resumeTime();
        SceneManager.LoadScene("IntroMenu");
    }
    public void OnPlayerStartTouchingGround() {
        groundStartTime = Time.time;
    }
    public void OnPlayerStopTouchingGround() {
        if (!won && !lost){
            totalGroundTime += Time.time - groundStartTime;
            groundStartTime = 0f;
        }
    }

    void LateUpdate() {
        if (lost || won) return;
        if (eyeInput != null && eyeInput.IsActive() && eyeInput.IsGazeOnRoad()) {
            roadFocusedTime += Time.deltaTime;
        }
        float currentTimeOnGround = groundStartTime == 0f? 0f : Time.time - groundStartTime;
        if (totalGroundTime + currentTimeOnGround >= maxTimeOnGround) {
            totalGroundTime += currentTimeOnGround;
            Lose();
        }
    }
}
