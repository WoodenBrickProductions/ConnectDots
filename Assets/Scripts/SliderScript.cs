using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for Rope data and animations
/// </summary>
public class SliderScript : MonoBehaviour
{
    [SerializeField] private float transitionTime = 1;
    private float time;
    private bool started = false;
    
    private Slider slider;
    private RectTransform rectTransform;

    void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = slider.GetComponent<RectTransform>();
        slider.value = 0;
    }

    void Update()
    {
        if (started)
        {
            slider.value = (transitionTime - time) / transitionTime;
            if (time <= 0)
            {
                started = false;
                GameController._gameController.ClearCurrentSlider();
            }
        }

        if (time > 0)
        {
            time -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Set start and end positions for rope
    /// </summary>
    /// <param name="startPosition">Start position</param>
    /// <param name="endPosition">End position</param>
    public void SetTargetPositions(Vector2 startPosition, Vector2 endPosition)
    {
        Vector2 delta = endPosition - startPosition;
        rectTransform.position = new Vector3(startPosition.x + delta.x / 2, startPosition.y + delta.y / 2, 0);
        rectTransform.sizeDelta = new Vector2(20,delta.magnitude/2);
        rectTransform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(new Vector2(0, 1), delta));
    }

    /// <summary>
    /// Start rope fill animation
    /// </summary>
    public void StartSlider()
    {
        started = true;
        gameObject.SetActive(true);
        time = transitionTime;
    }
}
