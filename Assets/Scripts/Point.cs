using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    [SerializeField] private float distance = 2;
    [SerializeField] private float transitionTime = 1;
    private float time;
    public int number = 0;
    public const int gemBaseScale = 1;
    
    
    private static Sprite blueGem = null;
    private static Sprite magentaGem = null;
    private RectTransform rectTransform;
    public bool started;
    private Text text;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<Text>();
    }

    private void Start()
    {
        rectTransform.localScale = new Vector3(Screen.height * gemBaseScale / GameController.baseHeight, Screen.height * gemBaseScale / GameController.baseHeight, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            Color color = text.color;
            color.a = 1 - (transitionTime - time) / transitionTime;
            text.color = color;
            if (time <= 0)
            {
                started = false;
            }
        }

        if (time > 0)
        {
            time -= Time.deltaTime;
        }
    }

    public bool InRadius(Vector2 point)
    {
        return Vector2.Distance(rectTransform.position, point) <= distance;
    }
    
    public void OnClick(Vector2 mousePosition)
    {
        
    }

    public void ChangeColor()
    {
        GetComponent<Image>().sprite = GetBlueGem();
        started = true;
        time = transitionTime;
    }

    public Vector3 GetPosition()
    {
        return rectTransform.position;
    }
    
    public static Sprite GetBlueGem()
    {
        if (blueGem == null)
        {
            blueGem = Resources.Load<Sprite>("Textures/button_blue");
        }
    
        return blueGem;
    }

    public void SetText(string newText)
    {
        if (text == null)
        {
            text = GetComponentInChildren<Text>();
        }
        text.text = newText;
    }
    
    public static Sprite GetMagentaGem()
    {
        if (magentaGem == null)
        {
            magentaGem = Resources.Load<Sprite>("Textures/button_magenta");
        }
    
        return magentaGem;
    }

}
