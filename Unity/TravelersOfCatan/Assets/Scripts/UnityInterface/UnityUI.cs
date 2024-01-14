using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NEAGame;
using Unity.VisualScripting;

public partial class UnityUI : MonoBehaviour, UI // This is the tip of the Unity interface
{

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
            GameWrapper gw = json.LOADGAME();
            game = new TravelersOfCatan(Interface, gw);

        }
        catch
        {
            Debug.Log("ERROR: Save file corrupted");
            // save has been corrupted
        }
        yield return null;
    }

    void saveCurrentGame()
    {
        int i = selectedSave;
        JSON_manager saver = new JSON_manager(i);
        saver.SAVEGAME(game);
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
