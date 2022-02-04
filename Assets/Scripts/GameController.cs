using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Touch = UnityEngine.Touch;


public class GameController : MonoBehaviour
{
    public static GameController _gameController;
    
    public const int baseWidth = 1920;
    public const int baseHeight = 1080;
    public const float DropdownBaseScale = 3;
    
    [Header("Canvas control")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject ropeCanvas;
    [SerializeField] private GameObject levelSelect;
    
    [Header("Object copies")]
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject ropePrefab;
    
    [SerializeField] private TextAsset jsonFile;
    
    private List<Point> points;
    private Queue<SliderScript> sliders;

    private TouchControl touchControl;
    public LevelCollection levelCollection;
    private SliderScript currentSlider;
    private int currentPoint;
    private Dropdown dropdown;
    private bool endOfLevel;
    
    private void Awake()
    {
        _gameController = this;
        touchControl = new TouchControl();
    }

    void Start()
    {
        touchControl.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControl.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
        points = new List<Point>();
        sliders = new Queue<SliderScript>();
        dropdown = levelSelect.GetComponentInChildren<Dropdown>();
        dropdown.transform.localScale = new Vector3(Screen.height * DropdownBaseScale / baseHeight, 
            Screen.height * DropdownBaseScale / baseHeight, 
            1);
        
        LoadLevelData();
        LoadDropdown();
    }
    
    void Update()
    {
        if (sliders.Count != 0)
        {
            if (currentSlider == null)
            {
                currentSlider = sliders.Peek();
                currentSlider.StartSlider();
            }
        }
        else
        {
            if (currentSlider == null && endOfLevel)
            {
                LevelEnd();
            }
        }
    }

    /// <summary>
    /// Load selected level for level dropdown
    /// </summary>
    public void LoadLevel()
    {
        if (points != null)
        {
            ClearLevel();
        }
        
        int index = dropdown.value;
        string[] pointData = levelCollection.levels[index].level_data;
        GameObject container = new GameObject("Container", typeof(RectTransform));
        container.transform.SetParent(canvas.transform);
        RectTransform rect = container.GetComponent<RectTransform>();
        print(Screen.height);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.position = new Vector3(0,Screen.height, 0);

        for (int i = 0; i < pointData.Length; i+=2)
        {
            GameObject obj = Instantiate(pointPrefab, container.transform);
            obj.GetComponent<RectTransform>().position = new Vector3(Screen.width * int.Parse(pointData[i]) / baseWidth, Screen.height - (Screen.height * int.Parse(pointData[i + 1])/baseHeight), 0);
            Point point = obj.GetComponent<Point>();
            point.SetText((i/2+1).ToString());
            points.Add(point);
        }

        currentPoint = 0;
        HideLevelSelect();
    }
    
    /// <summary>
    /// Load level data from .json file
    /// </summary>
    void LoadLevelData()
    {
        // using (StreamReader reader = new StreamReader(pathToLevelData))
        // {
        //     string json = reader.ReadToEnd();
        //     levelCollection = JsonUtility.FromJson<LevelCollection>(json);
        // }
        levelCollection = JsonUtility.FromJson<LevelCollection>(jsonFile.text);
    }

    /// <summary>
    /// Load level choices from level_data
    /// </summary>
    void LoadDropdown()
    {
        List<string> options = new List<string>();
        for(int i = 0; i < levelCollection.levels.Length; i++)
        {
            options.Add("Level " + (i + 1));
        }
        dropdown.AddOptions(options);
    }
    
    // Clear current level GameObjects
    void ClearLevel()
    {
        points = new List<Point>();
        currentPoint = 0;
        currentSlider = null;
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            Destroy(canvas.transform.GetChild(i).gameObject);            
        }
        for (int i = 0; i < ropeCanvas.transform.childCount; i++)
        {
            Destroy(ropeCanvas.transform.GetChild(i).gameObject);            
        }
    }
    
    /// <summary>
    /// Reset current animating slider to null
    /// </summary>
    public void ClearCurrentSlider()
    {
        currentSlider = null;
        sliders.Dequeue();
    }

    /// <summary>
    /// Hide level select canvas
    /// </summary>
    void HideLevelSelect()
    {
        levelSelect.SetActive(false);
    }

    /// <summary>
    /// Show level select canvas
    /// </summary>
    void ShowLevelSelect()
    {
        levelSelect.SetActive(true);
    }

    private void OnEnable()
    {
        touchControl.Enable();
    }

    private void OnDisable()
    {
        touchControl.Disable();
    }

    /// <summary>
    /// Method called upon touchscreen touch start
    /// </summary>
    /// <param name="context">Input context</param>
    private void StartTouch(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = touchControl.Touch.TouchPosition.ReadValue<Vector2>();
        if (currentPoint == points.Count)
        {
            return;
        }

        foreach (var point in points)
        {
            if (point.InRadius(touchPosition))
            {
                if (points[currentPoint] == point && !point.started)
                {
                    point.ChangeColor();

                    if (currentPoint != 0)
                    {
                        CreateRope(currentPoint - 1, currentPoint % points.Count);
                    }
                    currentPoint++;
                }

                if (currentPoint == points.Count)
                {
                    CreateRope(points.Count - 1, 0);
                    endOfLevel = true;
                }
            }
        }
    }

    /// <summary>
    /// Create rope between designated points in List<Point> points.
    /// </summary>
    /// <param name="startPoint">Rope starting point index in array</param>
    /// <param name="endPoint">Rope end point index in array</param>
    public void CreateRope(int startPoint, int endPoint)
    {
        GameObject obj = Instantiate(ropePrefab, ropeCanvas.transform);
        SliderScript sliderScript = obj.GetComponent<SliderScript>();
        sliders.Enqueue(sliderScript);
        sliderScript.SetTargetPositions(points[startPoint].GetPosition(), points[endPoint].GetPosition());
        sliderScript.gameObject.SetActive(false);
    }

    /// <summary>
    /// Method called on LevelEnd
    /// </summary>
    public void LevelEnd()
    {
        ShowLevelSelect();
        ClearLevel();
        endOfLevel = false;
    }
    
    private void EndTouch(InputAction.CallbackContext context)
    {
    }

    /// <summary>
    /// Class for reading .json data
    /// </summary>
    [Serializable]
    public class LevelCollection
    {
        public Level[] levels;
    }
    
    /// <summary>
    /// Class for reading .json data
    /// </summary>
    [Serializable]
    public class Level
    {
        public string[] level_data;
    }
}
