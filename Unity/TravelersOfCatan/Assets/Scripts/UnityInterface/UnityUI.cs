using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NEAGame;

public partial class UnityUI : MonoBehaviour, UI // This is the tip of the Unity interface
{




    public static UnityUI Interface { get; private set; }

    [Header("UI overlay prefabs")] 
    public GameObject NameGetOverlay;



    [Header("Serialized Game View")] 
    public TravelersOfCatan game;

    private string LoadFile = "";
    // Start is called before the first frame update
    void Awake()
    {
        if (Interface == null)
            Interface = this;
        
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        SceneManager.sceneLoaded += NewScene;
    }   

    void NewScene(Scene scene, LoadSceneMode mode)
    {
        
        if (scene.name == "Game")
        {
            SetupGameScene();
            if (LoadFile != "")
            {
                GameWrapper gw = JSON_manager.LOADGAME(LoadFile);
                game = new TravelersOfCatan(Interface, gw);
                
                game.StartTurn(gw.timer);
            }
            else
            {
                game.startGame();
            }

            
        }
        else if (scene.name == "GameSetup")
        {
            game = new TravelersOfCatan(Interface, 130, 1, 910f);

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
        SceneManager.LoadScene("Game");
        
    }


    public void StartGameButton()
    {
        SceneManager.LoadScene("GameSetup");
    }

    public void LoadGameButton()
    {
        LoadFile = "TESTFINDME";
        SceneManager.LoadScene("Game");

    }

    public void GoHome()
    {

        // add scene animation here


        SceneManager.LoadScene("Hub");
    }

    void QuitButtonPressed()
    {
        // add confirm here
        Application.Quit();
    }
    bool UI.GetUserConfirm()
    {
        return true;
    }

    void UI.CreatePopup(string message)
    {
        Debug.Log(message);
    }

    //void UI.DisplayPlayers(List<Player> players)
    //{
    //    //throw new System.NotImplementedException();
    //}


    

    void UI.SaveGame()
    {
        //throw new System.NotImplementedException();
    }

    void UI.LoadGame(string Save)
    {
        //throw new System.NotImplementedException();
    }


}
