using UnityEngine;

/// <summary>
/// Central manager for game state and high-level systems.
/// Usually one per scene. Singleton pattern for global access.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }



    private void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("Game Initialized");
        // Init managers here if needed
    }


    void Start()
    {
        // Kick off grid generation, UI setup, etc.
        GridManager.Instance.GenerateGrid();
    }
}
