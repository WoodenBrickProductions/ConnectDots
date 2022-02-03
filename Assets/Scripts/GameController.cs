using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.Touch;


public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private string pathToLevelData;
    
    private List<Point> points;

    public LevelCollection levelCollection;
    // Start is called before the first frame update
    void Start()
    {
        using (StreamReader reader = new StreamReader(pathToLevelData))
        {
            string json = reader.ReadToEnd();
            levelCollection = JsonUtility.FromJson<LevelCollection>(json);
        }
        
        // // levelCollection = 
        //
        // levelCollection = new LevelCollection();
        // levelCollection.levels = new Level[2];
        // levelCollection.levels[0] = new Level();
        // levelCollection.levels[0].level_data = new string[]{"100", "100", "100", "100"};
        // levelCollection.levels[1] = new Level();
        // levelCollection.levels[1].level_data = new string[]{"10", "00", "10", "10"};
        //
        
        using (StreamWriter writer = new StreamWriter("Assets/Resources/Data/test.json"))
        {
            string json = JsonUtility.ToJson(levelCollection);
            writer.Write(json);
        }

        // LevelCollection levelCollection2 = JsonUtility.FromJson<LevelCollection>("Assets/Resources/Data/test.json");
        
        
        TouchSimulation.Enable();
    }

    // Update is called once per frame
    void Update()
    {
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
