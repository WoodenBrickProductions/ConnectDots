using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for storing point information and handling text disappearing
/// </summary>
public class Point : MonoBehaviour
{
    [SerializeField] private float distance = 2;
    [SerializeField] private float transitionTime = 1;
    private float time;
    public const float gemBaseScale = 1;
    
    
    private static Sprite blueGem = null;
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
        print(Screen.height);
        rectTransform.localScale = new Vector3(Screen.height * gemBaseScale / GameController.baseHeight, 
                                               Screen.height * gemBaseScale / GameController.baseHeight, 
                                               1);
    }

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

    /// <summary>
    /// Check if point is in radius of object's transform
    /// </summary>
    /// <param name="point">point in 2D space</param>
    /// <returns>true if point is within object radius</returns>
    public bool InRadius(Vector2 point)
    {
        return Vector2.Distance(rectTransform.position, point) <= distance;
    }

    /// <summary>
    /// Change current Gem image to blue
    /// </summary>
    public void ChangeColor()
    {
        GetComponent<Image>().sprite = GetBlueGem();
        started = true;
        time = transitionTime;
    }

    /// <summary>
    /// Method to get position of point
    /// </summary>
    /// <returns>RectTransform position</returns>
    public Vector3 GetPosition()
    {
        return rectTransform.position;
    }
    
    /// <summary>
    /// Method to get Blue Gem sprite
    /// </summary>
    /// <returns>Sprite for Blue Gem (Blue button)</returns>
    public static Sprite GetBlueGem()
    {
        if (blueGem == null)
        {
            blueGem = Resources.Load<Sprite>("Textures/button_blue");
        }
    
        return blueGem;
    }

    /// <summary>
    /// Method to set this points displayed text
    /// </summary>
    /// <param name="newText">New text</param>
    public void SetText(string newText)
    {
        if (text == null)
        {
            text = GetComponentInChildren<Text>();
        }
        text.text = newText;
    }
}
