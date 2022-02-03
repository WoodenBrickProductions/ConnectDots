using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private float transitionTime = 1;
    private float time;
    private Slider slider;
    private bool started = false;
    private Text text;
    
    
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = slider.GetComponent<RectTransform>();
        text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
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

    public void SetTargetPositions(Vector2 position1, Vector2 position2)
    {
        Vector2 delta = position2 - position1;
        rectTransform.position = new Vector3(position1.x + delta.x / 2, position1.y + delta.y / 2, 0);
        rectTransform.sizeDelta = new Vector2(20,delta.magnitude/2);
        print(Vector2.Angle(new Vector2(0, -1), delta));
        rectTransform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(new Vector2(0, 1), delta));
    }

    public void StartSlider()
    {
        started = true;
        gameObject.SetActive(true);
        time = transitionTime;
    }
}
