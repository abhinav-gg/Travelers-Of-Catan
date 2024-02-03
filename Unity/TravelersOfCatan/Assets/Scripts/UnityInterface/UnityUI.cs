using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NEAGame;
using System.Linq;

/// <summary>
/// Class to interface between the game and the Unity UI
/// </summary>
public partial class UnityUI : MonoBehaviour, UI
{

    /// <summary>
    /// Singleton instance of the UnityUI class so that it can be accessed from other UI scripts
    /// </summary>
    public static UnityUI Interface { get; private set; }

    [Header("Serialized Game View")] 
    public TravelersOfCatan game;

    private int selectedSave = -1;
    private bool isLoading = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Interface == null)
            Interface = this;
        
        DontDestroyOnLoad(gameObject);
        game = null;
    }

    // OnEnable and OnDisable are used to subscribe and unsubscribe from the sceneLoaded event
    void OnEnable()
    {
        SceneManager.sceneLoaded += NewScene;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= NewScene;
    }

    // Method to prepare for a new scene being loaded
    void NewScene(Scene scene, LoadSceneMode mode)
    {
        
        if (scene.name == "Game")
        {
            SetupGameScene();
            if (isLoading)
            {
                StartCoroutine(LoadGame());
            }
            else
            {
                game.startGame();
            }

        }
        else if (scene.name == "Hub")
        {
            selectedSave = -1; // deselect the save slot when returning to the hub
        }
        else if (scene.name == "About+Rules")
        {
            GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>().onClick.AddListener(() => {
                AudioManager.i.Play("UIClick");
                AudioManager.i.Stop("Write");
                LeanTween.scale(GameObject.FindGameObjectWithTag("HomeButton"), new Vector3(1.2f, 1.2f, 1.2f), 0.2f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1);
                GoHome();
            }); // add a listener to the home button to go back to the hub from the about+rules page
        }
        else if (scene.name == "Victory")
        {
            AudioManager.i.Stop("Write");
            GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>().onClick.AddListener(() => {
                AudioManager.i.StopAll();
                AudioManager.i.Play("UIClick");
                LeanTween.scale(GameObject.FindGameObjectWithTag("HomeButton"), new Vector3(1.2f, 1.2f, 1.2f), 0.2f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1);
                GoHome();
            }); // add a listener to the home button to go back to the hub from the victory page
            
            string[] namesInVictoryOrder = game.gamePlayers.OrderByDescending(p => p.getVictoryPoints()).Select(p => p.playerName).ToArray();
            FindObjectOfType<VictoryManager>().Setup(namesInVictoryOrder);
            // order the players by victory points and pass the names to the victory manager
            saveCurrentGame();
        }
    }

    // Used in testing and debugging to assert that a condition is true
    void UI.Assert(bool test)
    {
        if (!test)
        {
            Debug.LogError("Assertion failed");
            Debug.Break();
        }
    }
  
    // Method to go to the game scene
    public void CommenceGame()
    {
        SceneTransition.i.SendToScene("Game");
    }

    // Called when the settings button is clicked on the home screen
    public void HomeScreenSettingsButton()
    {
        GamePauseOverlay overlay = Instantiate(PauseSettings).GetComponent<GamePauseOverlay>();
        overlay.RemoveSaveButton();
    }

    // Called when the start game button is clicked on the home screen
    public void LoadGameButton()
    {
        game = null;
        StartCoroutine(DisplayLoadStates());
    }

    // Method to display the save states
    IEnumerator DisplayLoadStates()
    {
        SceneTransition.i.PlayAnimation();
        yield return new WaitForSeconds(0.75f);
        SaveSelector overlay = Instantiate(GameSavePopup).GetComponent<SaveSelector>();
        overlay.Setup();
    }

    // Method to select the game to load from save slot i
    public void SelectGameToLoad(int i)
    {
        isLoading = true;
        selectedSave = i;
        CommenceGame();
    }

    // Method to create a new game in save slot i
    public void CreateNewGame(int i)
    {
        isLoading = false;
        selectedSave = i;
        SceneTransition.i.SendToScene("GameSetup");
    }

    // Method to load the game from the selected save slot
    IEnumerator LoadGame()
    {
        try
        {
            JSON_manager json = new JSON_manager(selectedSave);
            GameWrapper gw = json.LoadGame();
            game = new TravelersOfCatan(Interface, gw);
        }
        catch (System.Exception e) // handle any potential exceptions in serialization or null references when loading the game
        {
            Debug.Log(e); // save has been corrupted
            LoadGameButton();
            CreatePopup("Save has been corrupted");
        }
        yield return null;
    }

    // Save the current game
    void saveCurrentGame()
    {
        int i = selectedSave;
        JSON_manager saver = new JSON_manager(i);
        saver.SaveGame(game);
    }

    // Called by the pause menu to save the game and return to the menu page
    public void SaveAndExit()
    {
        saveCurrentGame();
        GoHome();
    }

    // Method to go to the menu page
    public void GoHome()
    {
        SceneTransition.i.SendToScene("Hub");
    }

    // onclick method for the about button
    public void AboutButton()
    {
        SceneTransition.i.SendToScene("About+Rules");
    }

    // onclick method for the quit button
    public void QuitButton()
    {
        Application.Quit();
    }

    // create a popup with the given message
    public void CreatePopup(string message)
    {
        Debug.Log(message); // put message in unity console for debugging
        PopupController overlay = Instantiate(AlertPopup).GetComponent<PopupController>();
        overlay.Setup(message);
    }

    // interface method to create a popup
    void UI.CreatePopup(string message)
    {
        CreatePopup(message);
    }

    // Called when the program is closed by the user to attempt to save the current game.
    public void OnApplicationQuit()
    {
        if (game != null && selectedSave != -1) // check if the user is in a game.
        {
            saveCurrentGame();
        }
    }
}
