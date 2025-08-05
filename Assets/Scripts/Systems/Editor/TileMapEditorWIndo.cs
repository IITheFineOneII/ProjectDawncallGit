using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class MapEditorWindow : EditorWindow
{
    //Tile Size 
    private int width = 10;
    private int height = 10;
    private GameObject[] tilePrefabs;     // Array to hold tile prefabs loaded from Resources
    private int selectedPrefabIndex = 0;
    private Tile[,] tilemapData;   
    private Vector2 scrollPos;
    private MapData mapData; // Holds the map data 
    /// <summary>
    /// Opens the Map Editor window.
    /// </summary>
    [MenuItem("Tools/Map Editor")]
    public static void ShowWindow()

    {
        MapEditorWindow window = GetWindow<MapEditorWindow>("Map Editor");
        window.minSize = new Vector2(400, 500);
    }

    private void OnEnable()
    {
        LoadTilePrefabs();
        InitializeMapData("Default");
    }

    /// <summary>
    /// Loads all tile prefabs from the Resources folder.
    /// </summary>
    private void LoadTilePrefabs()
    {
        tilePrefabs = Resources.LoadAll<GameObject>("Tiles");
        if (tilePrefabs.Length == 0)
        {
            Debug.LogWarning("No tile prefabs found in Resources/Tiles/");
        }
    }

    /// <summary>
    /// Initializes the map data for the specified map name by loading the corresponding JSON file from the Resources
    /// folder and instantiating the tiles defined in the map.
    /// </summary>
    /// <remarks>This method attempts to load a JSON file from the Resources/Maps directory using the provided
    /// <paramref name="mapName"/>. If the file is not found, an error is logged and the method exits. The JSON file is
    /// expected to contain map dimensions and tile definitions, which are used to instantiate tile prefabs at their
    /// respective positions.</remarks>
    /// <param name="mapName">The name of the map to initialize. This corresponds to the name of the JSON file located in the Resources/Maps
    /// directory.</param>
    private void InitializeMapData(string mapName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Maps/{mapName}");
        if (mapName == "Default" && jsonFile == null)
        {
            tilemapData = new Tile[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tilemapData[x, y] = new Tile
                    {
                        x = x,
                        y = y,
                        tileId = "Plain",
                        biomeType = BiomeType.Plain
                    };
                }
            }
        }
        if (jsonFile == null)
        {
            Debug.LogError($"Map file '{mapName}' not found in Resources/Maps/");
            return;
        }
        mapData = JsonUtility.FromJson<MapData>(jsonFile.text);
        width = mapData.MapSizeX;
        height = mapData.MapSizeY;
        foreach (Tile tile in mapData.Tilemap)
        {
            GameObject prefab = FindPrefabById(tile.tileId);
            if (prefab != null)
            {
                Instantiate(prefab, new Vector3(tile.x, 0, tile.y), Quaternion.identity);
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Map Size", EditorStyles.boldLabel);
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        if (GUILayout.Button("Reset Map Size"))
        {
            InitializeMapData("Default");
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Select Tile Prefab", EditorStyles.boldLabel);
        if (tilePrefabs != null && tilePrefabs.Length > 0)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < tilePrefabs.Length; i++)
            {
                GUIStyle style = (i == selectedPrefabIndex) ? EditorStyles.boldLabel : EditorStyles.label;
                if (GUILayout.Button(tilePrefabs[i].name, style))
                {
                    selectedPrefabIndex = i;
                    //selectedPrefab = GetBiomeFromPrefab(tilePrefabs[i]);
                }
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("No tile prefabs found in Resources/Tiles/", MessageType.Warning);
        }

        EditorGUILayout.Space();

        // Draw grid of tiles
        EditorGUILayout.LabelField("Map Editor", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        float tileSize = 40f;
        int tilesPerRow = Mathf.FloorToInt(position.width / tileSize);

        for (int y = height - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < width; x++)
            {
                Color originalColor = GUI.backgroundColor;

                // Color tile button based on biome type
                GUI.backgroundColor = BiomeToColor(tilemapData[x, y].biomeType);

                if (GUILayout.Button("", GUILayout.Width(tileSize), GUILayout.Height(tileSize)))
                {
                    GameObject selectedPrefab = tilePrefabs[selectedPrefabIndex];
                    Tile prefabTile = selectedPrefab.GetComponent<Tile>();
                    tilemapData[x, y] = new Tile
                    {
                        x = x,
                        y = y,
                        tileId = prefabTile.tileId,
                        biomeType = prefabTile.biomeType
                    };
                }
                GUI.backgroundColor = originalColor;
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        if (GUILayout.Button("Save Map to Scene"))
        {
            SaveMapToScene();
        }
    }

    private Color BiomeToColor(BiomeType biome)
    {
        switch (biome)
        {
            case BiomeType.Plain: return Color.green;
            case BiomeType.Desert: return Color.yellow;
            case BiomeType.Mountain: return new Color(0.59f, 0.29f, 0f);
            case BiomeType.Water: return Color.blue;
            case BiomeType.Swamp: return new Color(0.5f, 0.5f, 0f);
            case BiomeType.Tundra: return Color.gray;
            default: return Color.magenta;
        }
    }
private void SaveMapToScene()
{
    // Clear existing map tiles in scene
    GameObject existingMapParent = GameObject.Find("MapTiles");
    if (existingMapParent != null)
    {
        DestroyImmediate(existingMapParent);
    }

    GameObject mapParent = new GameObject("MapTiles");

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            GameObject prefab = FindPrefabById(tilemapData[x, y].tileId);
            if (prefab != null)
            {
                Vector3 pos = new Vector3(x, 0, y);

                // Instantiate prefab properly in editor mode
                GameObject tileGO = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                tileGO.transform.position = pos;
                tileGO.name = $"Tile {x},{y}";
                tileGO.transform.SetParent(mapParent.transform);

                Undo.RegisterCreatedObjectUndo(tileGO, "Create Tile");

                // Set tile data to be safe
                Tile tileScript = tileGO.GetComponent<Tile>();
                if (tileScript != null)
                {
                    tileScript.Init(x, y, tilemapData[x, y].biomeType);
                }
            }
        }
    }
}


    private GameObject FindPrefabById(string id)
    {
        if (tilePrefabs == null) return null;
        else
        {
            GameObject prefab = System.Array.Find(tilePrefabs, obj => obj.GetComponent<Tile>().tileId == id);
            return prefab;
        }
    }
}
