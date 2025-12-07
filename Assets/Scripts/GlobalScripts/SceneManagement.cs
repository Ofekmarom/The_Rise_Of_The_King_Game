using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


/// <summary>
/// Manages scene transitions, level loading, and game-stages mapping.
/// </summary>
public class SceneManagement : MonoBehaviour
{
    [Header("Mini Games and Stages")]
    [Tooltip("Reference to the MiniGamesAndStages ScriptableObject.")]
    public MiniGamesAndStages miniGamesAndStages;

    [Header("Scriptable Objects")]
    [Tooltip("Reference to the TimeData Scriptable Object.")]
    [SerializeField] private TimeData timeData;

    [Tooltip("Reference to the ScoreData Scriptable Object.")]
    [SerializeField] private ScoreData scoreData;

    [Header("Managed Scenes")]
    [Tooltip("Array of scenes that should not destroy their GameObjects.")]
    [SerializeField]
    private string[] managedScenes; // Add the managed scenes here via the Inspector.

    private static SceneManagement instance;

    private void Start()
    {
        if (miniGamesAndStages == null)
        {
            Debug.LogError("[SceneManagement:Start] MiniGamesAndStages is not assigned.");
            return;
        }

        Debug.Log($"[SceneManagement:Start] Loaded MiniGamesAndStages with {miniGamesAndStages.games.Count} games.");
    }

    /// <summary>
    /// Manages the persistence and uniqueness of this GameObject instance across all scenes based on managed scenes.
    /// This method checks if the current scene is one of the managed scenes. If it is, the GameObject is set to not be destroyed or destroy other instances.
    /// If another instance of this type already exists and it is not this instance, it checks if they are of the same type and not the same GameObject.
    /// If these conditions are met, it destroys the newly created instance. Otherwise, it sets this instance as the persistent one across scenes.
    /// It also checks if a necessary ScriptableObject is assigned and logs the number of games it contains.
    /// In Simple Words: makes this an instance that not get destroyed across the game 
    /// </summary>
    private void Awake()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if the current scene is in the managed scenes
        if (Array.Exists(managedScenes, scene => scene == currentSceneName))
        {
            Debug.Log($"[SceneManagement] Current scene '{currentSceneName}' is managed. Preserving this instance.");
            DontDestroyOnLoad(gameObject);
            return;
        }

        if (instance != null && instance != this)
        {
            Debug.Log("[SceneManagement] Another instance exists. Checking for match...");

            if (instance.gameObject != this.gameObject && instance.GetType() == this.GetType())
            {
                Debug.Log($"[SceneManagement] Destroying the new instance for scene: {currentSceneName} as it's not required.");
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SceneManagement] This instance will persist across scenes.");
        }

        if (miniGamesAndStages == null)
        {
            Debug.LogError("[SceneManagement] MiniGamesAndStages ScriptableObject is not assigned!");
        }
        else
        {
            Debug.Log($"[SceneManagement] Awake: Loaded MiniGamesAndStages with {miniGamesAndStages.games.Count} games.");
        }
    }


    public static SceneManagement Instance => instance;

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            Debug.Log("[SceneManagement] Instance destroyed.");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[SceneManagement] Subscribed to sceneLoaded event.");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("[SceneManagement] Unsubscribed from sceneLoaded event.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneManagement] Scene loaded: {scene.name}");

        if (miniGamesAndStages == null)
        {
            Debug.LogError("[SceneManagement] MiniGamesAndStages reference is null after scene load!");
        }
        else
        {
            Debug.Log($"[SceneManagement] MiniGamesAndStages contains {miniGamesAndStages.games.Count} games after loading scene {scene.name}.");
        }
    }

    /// <summary>
    /// Retrieves the game name for the current scene based on the mapping.
    /// </summary>
    public string GetCurrentGameName()
    {
        // Get the name of the currently active scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneManagement - GetCurrentGameName] Active Scene: '{currentSceneName}'.");

        // Loop through all games in MiniGamesAndStages to find the matching game
        foreach (var gameInfo in miniGamesAndStages.games)
        {
            if (gameInfo.stageNames.Contains(currentSceneName))
            {
                Debug.Log($"[SceneManagement - GetCurrentGameName] Found Game: '{gameInfo.gameName}' for Scene: '{currentSceneName}'.");
                return gameInfo.gameName; // Return the name of the mini-game
            }
        }

        // If no match is found, log a warning and return "Unknown Game"
        Debug.LogWarning($"[SceneManagement - GetCurrentGameName] Scene '{currentSceneName}' does not match any known game.");
        return "Unknown Game";
    }

    /// <summary>
    /// Retrieves the stage index for the current scene.
    /// </summary>
    public int GetCurrentStageIndex()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneManagement(163) - GetCurrentGameStage] Current Scene?  as active: '{currentSceneName}'.");

        foreach (var gameInfo in miniGamesAndStages.games)
        {
            int index = gameInfo.stageNames.IndexOf(currentSceneName);
            if (index != -1)
            {
                return index;
            }
        }

        Debug.LogWarning($"[SceneManagement] Scene '{currentSceneName}' does not match any known stage.");
        return -1;
    }

    /// <summary>
    /// Completes the current level reset time & score for stage and loads the next level.
    /// </summary>
    public void CompleteLevel()
    {
        // Retrieve the current game name and stage index using the provided methods
        string currentGameName = GetCurrentGameName();
        int currentStageIndex = GetCurrentStageIndex();

        // Check if the game is valid and if the current stage index is valid
        if (currentGameName != "Unknown Game" && currentStageIndex != -1)
        {
            Debug.Log($"[SceneManagement] Current Game: {currentGameName}, Current Stage Index: {currentStageIndex}");

            // Get the list of stages for the current game
            var currentGame = miniGamesAndStages.games.FirstOrDefault(game => game.gameName == currentGameName);

            if (currentGame != null && currentStageIndex < currentGame.stageNames.Count - 1)
            {
                // Move to the next level
                string nextSceneName = currentGame.stageNames[currentStageIndex + 1];
                Debug.Log($"[SceneManagement] Moving to next scene: {nextSceneName}");
                ResetCurrentLevelData();
                LoadLevelByName(nextSceneName);
            }
            else//if player completed the levels
            {
                ResetCurrentLevelData();
                Debug.Log($"[SceneManagement] All levels completed for game: {currentGameName}");
                LoadLevelByName("KingsLobby");  // Handle the case where all levels in the current game are completed
            }
        }
        else
        {
            Debug.LogWarning("[SceneManagement] Could not determine current game or stage. Falling back to default behavior.");
        }
    }

    /// <summary>
    /// Loads the next level by its name.
    /// </summary>
    private void LoadLevelByName(string levelName)
    {

        Debug.Log($"[SceneManagement] Loading level: {levelName}");
        SceneManager.LoadScene(levelName);
    }


    /// <summary>
    /// Resets the time and score data for the current level if it is part of a game.
    /// </summary>
    /// <param name="currentSceneName">The name of the current scene.</param>
    private void ResetCurrentLevelData()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;



        string gameName = GetCurrentGameName();
        if (gameName == null)
        {
            Debug.LogWarning($"[SceneTransitionManager] No game found for current scene: {currentSceneName}");
            return;
        }

        // Reset time data
        if (timeData != null)
        {
            timeData.ResetStageTime(gameName, currentSceneName);
            Debug.Log($"[SceneTransitionManager] Time reset for stage: {currentSceneName}");
        }
        else
        {
            Debug.LogError("[SceneTransitionManager] TimeData is not assigned.");
        }

        // Reset score data
        if (scoreData != null)
        {
            scoreData.ResetStageScores(gameName, currentSceneName);
            Debug.Log($"[SceneTransitionManager] Score reset for stage: {currentSceneName}");
        }
        else
        {
            Debug.LogError("[SceneTransitionManager] ScoreData is not assigned.");
        }
    }
}
