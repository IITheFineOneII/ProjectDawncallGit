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
        // Init managers here
    }


    void Start()
    {
        GridManager.Instance.GenerateGrid();
    }
}
