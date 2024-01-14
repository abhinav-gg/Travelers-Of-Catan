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
    // Start is called before the first frame update
    void Awake()
    {
        if (Interface == null)
            Interface = this;
        
        DontDestroyOnLoad(gameObject);

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
            if (selectedSave != -1)
            {
            JSON_manager json = new JSON_manager(selectedSave);
                GameWrapper gw = json.LOADGAME();
                game = new TravelersOfCatan(Interface, gw);

            }
            else
            {
                game.startGame();
            }

            
        }
        else if (scene.name == "GameSetup")
        {
            game = new TravelersOfCatan(Interface, 15, 1, 910f);

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
        selectedSave = i;
        CommenceGame();
    }

    public void CreateNewGame(int i)
    {
        selectedSave = i;
        SceneTransition.i.SendToScene("GameSetup");
    }


    public void saveCurrentGame()
    {
        int i = selectedSave;
        JSON_manager saver = new JSON_manager(i);
        saver.SAVEGAME(game);
    }




    public void GoHome()
    {

        SceneTransition.i.SendToScene("Hub");

    }

    void QuitButtonPressed()
    {
        // add confirm here
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
        
    }


}
