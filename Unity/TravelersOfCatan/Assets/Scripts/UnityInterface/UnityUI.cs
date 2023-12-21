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
            game = new TravelersOfCatan(Interface, 130, 40);
            StartCoroutine(GetNewPlayer(2));

            SetupGameScene();

            
        }
    }


    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= NewScene;
    }

    // Update is called once per frame
    void Update()
    {

        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            game.ShowBoard();
        }*/
        //Debug.Log(JsonUtility.ToJson(game));

    }

    public void StartGameButton()
    {
        SceneManager.LoadScene("Game");
    }


    void MenuButtonPressed()
    {
        // add confirm here
        SceneManager.LoadScene("Menu");
    }

    void QuitButtonPressed()
    {
        // add confirm here
        Application.Quit();
    }

    IEnumerator GetNewPlayer(int playersLeft)
    {

        if (playersLeft > 0)
        {
            Instantiate(Interface.NameGetOverlay);
            
            while (FindObjectOfType<PlayerNameInp>().FinalName == "")
            {
                yield return null;
            }

            string name = FindObjectOfType<PlayerNameInp>().FinalName;
            game.AddPlayer(name);
            Destroy(FindObjectOfType<PlayerNameInp>().gameObject);
            playersLeft--;
            yield return StartCoroutine(GetNewPlayer(playersLeft));
        }
        else
        {
            game.startGame();

        }
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
