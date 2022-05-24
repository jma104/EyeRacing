using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMenuHandler : MonoBehaviour
{
    public float outOfRoadPenaltyMultiplier = 3.0f;
    public float nonCollectedPenaltyMultiplier = 10.0f;

    private float totalTime;
    private float penaltyTime;
    private int nCollectables;
    private int totalCollectables;
    private float focusedTime;

    private Text totalTimeText;
    private Text penaltyTimeText;
    private Text focusTimeText;
    private Text checkpointsText;
    private Text totalScoreText;

    public void Awake() {
        foreach (Transform child in transform) {
            switch(child.name) {
                case "TotalScore": totalScoreText = child.GetComponent<Text>(); break;
                case "TotalTime": totalTimeText = child.GetComponent<Text>(); break;
                case "FocusTime": focusTimeText = child.GetComponent<Text>(); break;
                case "PenaltyTime": penaltyTimeText = child.GetComponent<Text>(); break;
                case "Checkpoints": checkpointsText = child.GetComponent<Text>(); break;
            }
        }
    }
    public void SetScore(float totalTime, float penaltyTime, int nCollectables, int totalCollectables, float focusedTime) {
        this.totalTime = totalTime;
        this.penaltyTime = penaltyTime;
        this.nCollectables = nCollectables;
        this.totalCollectables = totalCollectables;
        this.focusedTime = focusedTime;

        float nonCollected = (totalCollectables - nCollectables);

        float totalScore = totalTime + 
                           penaltyTime * outOfRoadPenaltyMultiplier +
                           nonCollected * nonCollectedPenaltyMultiplier;

        totalTimeText.text = totalTime.ToString("0.0") + " s";
        focusTimeText.text = focusedTime.ToString("0.0") + " s";
        penaltyTimeText.text = penaltyTime.ToString("0.0") + " s (x" + outOfRoadPenaltyMultiplier + ")";
        checkpointsText.text = nonCollected + " / " + totalCollectables + " (x" + nonCollectedPenaltyMultiplier + " s)";
        totalScoreText.text = totalScore.ToString("0.0") + " s";
    }
}
