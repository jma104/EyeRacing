using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Tobii.Gaming;

public class GazeAwareButton : MonoBehaviour
{
    public float clickDuration = 1.0f;
    public Vector2 sizePctWhenFullySelected = new Vector2(0.9f, 0.95f);
    private Vector2 originalSize;
    private GazeAware gazeAware;
    private Button button;
    private RectTransform rectTransform;
    private bool isFocused;
    private float focusTime;
    private bool alreadyClicked = false;

    void Awake()
    {
        button = GetComponent<Button>();
        gazeAware = GetComponent<GazeAware>();
        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;
    }

    void changeSize(float width, float height, float t) {
        Vector2 prev = rectTransform.sizeDelta;
        rectTransform.sizeDelta = new Vector2(Mathf.Lerp(prev.x, width, t*t*t), Mathf.Lerp(prev.y, height, t*t*t));

    }

    void Update()
    {
        if (gazeAware.HasGazeFocus && !isFocused) {
            isFocused = true;
            focusTime = Time.unscaledTime;
            rectTransform.sizeDelta = originalSize;
        } else if (gazeAware.HasGazeFocus && Time.unscaledTime - focusTime < clickDuration && !alreadyClicked) {
            float t = (Time.unscaledTime - focusTime) / clickDuration;
            changeSize(originalSize.x * sizePctWhenFullySelected.x, originalSize.y * sizePctWhenFullySelected.y, t);
        } else if (gazeAware.HasGazeFocus && Time.unscaledTime - focusTime >= clickDuration && !alreadyClicked) {
            alreadyClicked = true;
            button.onClick.Invoke();
        } else if (!gazeAware.HasGazeFocus) {
            if (isFocused) {
                rectTransform.sizeDelta = originalSize;
            }
            isFocused = false;
            alreadyClicked = false;
        }
    }
}
