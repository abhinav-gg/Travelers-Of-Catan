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

    void OnEnable()
    {
        SceneManager.sceneLoaded += NewScene;
    }   

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
            selectedSave = -1;
        }
        else if (scene.name == "About+Rules")
        {
            GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>().onClick.AddListener(() => {
                AudioManager.i.Play("UIClick");
                AudioManager.i.Stop("Write");
                LeanTween.scale(GameObject.FindGameObjectWithTag("HomeButton"), new Vector3(1.2f, 1.2f, 1.2f), 0.2f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1);
                GoHome();
            });
        }
        else if (scene.name == "Victory")
        {
            AudioManager.i.Stop("Write");
            GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>().onClick.AddListener(() => {
                AudioManager.i.Play("UIClick");
                AudioManager.i.Stop("Write");
                LeanTween.scale(GameObject.FindGameObjectWithTag("HomeButton"), new Vector3(1.2f, 1.2f, 1.2f), 0.2f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1);
                GoHome();
            });
            string[] namesInVictoryOrder = game.gamePlayers.OrderByDescending(p => p.getVictoryPoints()).Select(p => p.playerName).ToArray();
            FindObjectOfType<VictoryManager>().Setup(namesInVictoryOrder);
            saveCurrentGame();
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= NewScene;
    }

    void UI.Assert(bool test)
    {
        if (!test)
        {
            Debug.LogError("Assertion failed");
            Debug.Break();
        }
    }
  
    public void CommenceGame()
    {
        SceneTransition.i.SendToScene("Game");
    }

    public void HomeScreenSettingsButton()
    {
        GamePauseOverlay overlay = Instantiate(PauseSettings).GetComponent<GamePauseOverlay>();
        overlay.RemoveSaveButton();
    }

    public void LoadGameButton()
    {
        game = null;
        StartCoroutine(DisplayLoadStates());
    }


    IEnumerator DisplayLoadStates()
    {
        SceneTransition.i.PlayAnimation();
        yield return new WaitForSeconds(0.75f);
        SaveSelector overlay = Instantiate(GameSavePopup).GetComponent<SaveSelector>();
        overlay.Setup();
    }

    public void SelectGameToLoad(int i)
    {
        isLoading = true;
        selectedSave = i;
        CommenceGame();
    }

    public void CreateNewGame(int i)
    {
        isLoading = false;
        selectedSave = i;
        SceneTransition.i.SendToScene("GameSetup");
    }

    IEnumerator LoadGame()
    {
        try
        {
            JSON_manager json = new JSON_manager(selectedSave);
            GameWrapper gw = json.LoadGame();
            game = new TravelersOfCatan(Interface, gw);

        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            // save has been corrupted
            LoadGameButton();
            CreatePopup("Save has been corrupted");
        }
        yield return null;
    }

    void saveCurrentGame()
    {
        int i = selectedSave;
        JSON_manager saver = new JSON_manager(i);
        saver.SaveGame(game);
    }

    public void SaveAndExit()
    {
        saveCurrentGame();
        GoHome();
    }


    public void GoHome()
    {

        SceneTransition.i.SendToScene("Hub");

    }

    public void AboutButton()
    {
        SceneTransition.i.SendToScene("About+Rules");
    }



    public void QuitButton()
    {
        Application.Quit();
    }


    public void CreatePopup(string message)
    {

        Debug.Log(message);


        PopupController overlay = Instantiate(AlertPopup).GetComponent<PopupController>();
        overlay.Setup(message);
    }

    void UI.CreatePopup(string message)
    {
        CreatePopup(message);
    }


    public void OnApplicationQuit()
    {
        if (game != null && selectedSave != -1)
        {
            saveCurrentGame();
        }
    }


}
