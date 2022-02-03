using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] private float distance = 2;
    private RectTransform rectTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool InRadius(Vector2 point)
    {
        return Vector2.Distance(rectTransform.position, point) <= distance;
    }
    
    public void OnClick(Vector2 mousePosition)
    {
        
    }
    
}
