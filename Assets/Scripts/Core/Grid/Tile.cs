using UnityEngine;

/// <summary>
/// Stores data about an individual tile in the grid.
/// </summary>
public class Tile : MonoBehaviour
{
    public string tileId; // Unique identifier for the tile, can be used for debugging or saving/loading
    public int x, y;    // Grid coordinates of this tile
    public BiomeType biomeType; // Type of biome this tile belongs to
    public FeatureType features;    // Bitmask for features like forest, river, etc.
    public BuildingType buildingType;   // Type of building on this tile
    private Renderer tileRenderer;  // Reference to the Renderer component on the tile
    private MaterialPropertyBlock mpb; // Material property block for efficient material updates


    private void Awake()
    {
        // Cache the Renderer component for efficiency
        tileRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    /// <summary>
    /// Initializes the tile's grid coordinates.
    /// </summary>
    public void Init(int x, int y, BiomeType biomeType)
    {
        this.x = x;
        this.y = y;

        gameObject.name = $"Tile {x},{y}";
        SetTileType(biomeType);
    }

    #region Functions
    public void SetTileType(BiomeType type)
    {
        Color biomeColor = Color.white; // default
        // Change material color based on the TileType enum
        switch (type)
        {
            case BiomeType.Plain: biomeColor = Color.green; break;
            case BiomeType.Desert: biomeColor = Color.yellow; break;
            case BiomeType.Mountain: biomeColor = new Color(0.59f, 0.29f, 0f); break;
            case BiomeType.Water: biomeColor = Color.blue; break;
            case BiomeType.Swamp: biomeColor = new Color(0.5f, 0.5f, 0f); break;
            case BiomeType.Tundra: biomeColor = Color.gray; break;
            default: biomeColor = new Color(1f, 0.41f, 0.71f); break;
        }

        mpb.SetColor("_BaseColor", biomeColor);
        tileRenderer.SetPropertyBlock(mpb);
    }



    #endregion



    #region Helpers
    //Helper to check if feature is present on this tile
    public bool HasFeature(FeatureType feature)
    {
        return (features & feature) != feature;
    }

    // Helper to add a feature to this tile
    public void AddFeature(FeatureType feature)
    {
        features |= feature;
    }
    // Helper to remove a feature from this tile
    public void RemoveFeature(FeatureType feature)
    {
        features &= ~feature;
    }
    #endregion
}