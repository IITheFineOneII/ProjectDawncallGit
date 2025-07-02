using UnityEngine;

/// <summary>
/// Handles creation and management of the tile grid.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public int width = 10;
    public int height = 10;
    public GameObject tilePrefab;

    private GameObject[,] gridArray; // 2D array to store tile references

    void Awake()
    {
        // Singleton init
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #region Functions
    /// <summary>
    /// Initializes the grid at the beginning of the game.
    /// </summary>
    public void GenerateGrid()
    {
        gridArray = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Spawn a tile at the grid position
                Vector3 spawnPos = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
                tile.name = $"Tile {x},{y}";
                gridArray[x, y] = tile;

                // Attach tile data script and set its coordinates
                tile.GetComponent<Tile>().Init(x, y, BiomeType.Mountain);
            }
        }
        CenterCameraOnGrid();
    }

    /// <summary>
    /// Centers the camera on the middle tile of the grid.
    /// </summary>
    private void CenterCameraOnGrid()
    {
        int centerX = width / 2;
        int centerY = height / 2;

        GameObject centerTile = gridArray[centerX, centerY];

        if (centerTile != null)
        {
            CameraController camController = Camera.main.GetComponent<CameraController>();
            if (camController != null)
            {
                camController.targetPosition = centerTile.transform.position;
            }
            else
            {
                Debug.LogWarning("CameraController component not found on Main Camera.");
            }
        }
        else
        {
            Debug.LogWarning("Center tile is null.");
        }
    }
    #endregion
}
