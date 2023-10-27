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


    [Header("Serialized Game View")] public TravelersOfCatan game;


    // Start is called before the first frame update
    void Awake()
    {
        if (Interface == null)
            Interface = this;
        
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        GameInterface = FindObjectOfType<GameUI>();
        GameInterface.StartGame();
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



    /// <summary>
    /// Functions to convert between Unity and System.Numerics vectors
    /// </summary>


    void UI.BeginTurn()
    {
        GameInterface.BeginTurn();
    }


    Node UI.GetUserNodeChoice(Node[] options)
    {
        return GameInterface.GetUserNodeChoice(options);
    }

    string UI.GetUserNameInput(int who)
    {
        return "AAAA";
    }

    bool UI.GetUserConfirm()
    {
        return true;
    }

    void UI.CreatePopup(string message)
    {
        Debug.Log(message);
    }

    void UI.DisplayPlayers(Player[] players)
    {
        GameInterface.DisplayPlayers(players);
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
