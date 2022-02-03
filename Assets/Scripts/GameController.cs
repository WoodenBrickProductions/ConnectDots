using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.Touch;


public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private string pathToLevelData;
    
    private TouchControl touchControl;
    private List<Point> points;

    public LevelCollection levelCollection;

    private void Awake()
    {
        touchControl = new TouchControl();
    }

    // Start is called before the first frame update
    void Start()
    {
        touchControl.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControl.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
        points = new List<Point>(); 
        
        LoadLevelData();
        LoadLevel(0);
    }

    void LoadLevel(int index)
    {
        string[] pointData = levelCollection.levels[index].level_data;
        GameObject container = new GameObject("Container", typeof(RectTransform));
        container.transform.SetParent(canvas.transform);
        RectTransform rect = container.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.position = new Vector3(0,0, 0);
        
        for (int i = 0; i < pointData.Length/2; i++)
        {
            GameObject obj = Instantiate(pointPrefab, container.transform);
            obj.GetComponent<RectTransform>().position = new Vector3(int.Parse(pointData[i]), Screen.height - int.Parse(pointData[i + 1]), 0);
            points.Add(obj.GetComponent<Point>());
        }
    }

    void LoadLevelData()
    {
        using (StreamReader reader = new StreamReader(pathToLevelData))
        {
            string json = reader.ReadToEnd();
            levelCollection = JsonUtility.FromJson<LevelCollection>(json);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        touchControl.Enable();
    }

    private void OnDisable()
    {
        touchControl.Disable();
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = touchControl.Touch.TouchPosition.ReadValue<Vector2>();
        print("Touch started" + touchPosition);

        foreach (var point in points)
        {
            if (point.InRadius(touchPosition))
            {
                print("Finger near gem");
            }
        }
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        print("Touch ended");
    }

    [Serializable]
    public class LevelCollection
    {
        public Level[] levels;
    }
    
    [Serializable]
    public class Level
    {
        public string[] level_data;
    }
}
