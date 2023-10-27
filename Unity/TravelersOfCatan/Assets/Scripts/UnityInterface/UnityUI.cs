using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NEAGame;
using App;

public class UnityUI : MonoBehaviour, UI // This is the tip of the Unity interface
{




    public static UnityUI Interface { get; private set; }
    public static GameUI GameInterface { get; private set; }

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
        game = new TravelersOfCatan(Interface);
    }

    void Start()
    {
        GameInterface = FindObjectOfType<GameUI>();
        
        StartCoroutine(GetNewPlayer(2));
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            game.ShowBoard();
        }*/
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


    void UI.BeginTurn()
    {
        GameInterface.BeginTurn();
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


    void UI.GetUserNodeChoice(Node[] options)
    {
        GameInterface.GetUserNodeChoice(options);
    }

    bool UI.GetUserConfirm()
    {
        return true;
    }

    void UI.CreatePopup(string message)
    {
        Debug.Log(message);
    }

    void UI.DisplayPlayers(List<Player> players)
    {
        //throw new System.NotImplementedException();
    }

    void UI.UpdateBoard(Board board)
    {
        GameInterface.UpdateBoard(board);
        //throw new System.NotImplementedException();
    }

    void UI.UpdateBoardConnections(Board board)
    {
        //throw new System.NotImplementedException();
    }

    void UI.HandleWinner(Player winner)
    {
        //throw new System.NotImplementedException();
    }

    void UI.SaveGame()
    {
        //throw new System.NotImplementedException();
    }

    void UI.LoadGame(string Save)
    {
        //throw new System.NotImplementedException();
    }

    void UI.ShowCost(string ID)
    {
        //throw new System.NotImplementedException();
    }
}
