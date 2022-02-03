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
    public const int baseWidth = 1920;
    public const int baseHeight = 1080;
    
    public static GameController _gameController;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject ropeCanvas;
    [SerializeField] private GameObject levelSelect;
    
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private string pathToLevelData;
    
    private TouchControl touchControl;
    private List<Point> points;

    private Queue<SliderScript> sliders;
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

    // Start is called before the first frame update
    void Start()
    {
        touchControl.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControl.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
        points = new List<Point>();
        sliders = new Queue<SliderScript>();
        dropdown = levelSelect.GetComponentInChildren<Dropdown>();
        
        print(Screen.height + " " + Screen.width);
        
        LoadLevelData();
        LoadDropdown();
    }
    
    // Update is called once per frame
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
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.position = new Vector3(0,1080, 0);

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

    void HideLevelSelect()
    {
        levelSelect.SetActive(false);
    }

    void ShowLevelSelect()
    {
        levelSelect.SetActive(true);
    }
    
    void LoadLevelData()
    {
        using (StreamReader reader = new StreamReader(pathToLevelData))
        {
            string json = reader.ReadToEnd();
            levelCollection = JsonUtility.FromJson<LevelCollection>(json);
        }
    }

    void LoadDropdown()
    {
        List<string> options = new List<string>();
        for(int i = 0; i < levelCollection.levels.Length; i++)
        {
            options.Add("Level " + (i + 1));
        }
        dropdown.AddOptions(options);
    }
    
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
    
    public void ClearCurrentSlider()
    {
        currentSlider = null;
        sliders.Dequeue();
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

    public void CreateRope(int startPoint, int endPoint)
    {
        GameObject obj = Instantiate(ropePrefab, ropeCanvas.transform);
        SliderScript sliderScript = obj.GetComponent<SliderScript>();
        sliders.Enqueue(sliderScript);
        sliderScript.SetTargetPositions(points[startPoint].GetPosition(), points[endPoint].GetPosition());
        sliderScript.gameObject.SetActive(false);
    }
    
    
    public void LevelEnd()
    {
        ShowLevelSelect();
        ClearLevel();
        endOfLevel = false;
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
