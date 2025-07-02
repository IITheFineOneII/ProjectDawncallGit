using UnityEngine;

/// <summary>
/// Manages UI elements like menus, tooltips, population panels.
/// Placeholder for now.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowBuildMenu()
    {
        // Show building panel UI
    }

    public void UpdatePopulationDisplay(int totalPop)
    {
        // Update UI text with current population
    }
}