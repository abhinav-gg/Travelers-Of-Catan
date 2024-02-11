using System;
using System.Collections.Generic;
using System.Numerics;

namespace NEAGame
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// The <c>UI</c> interface is used to define the methods that the game will use to interact with the user.<br/>
    /// Skill A: Complex OOP - Interface
    /// </summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface UI
    {
        // gets the current time of the game in seconds
        float GetTimer();
        // offers the user a choice of nodes to select and then calls the method passing the selected node as a parameter
        void GetUserNodeChoice(Node[] options, Action<Node> method);
        // displays a message to the user
        void CreatePopup(string message);
        // called when the game is over to display the winner
        void HandleWinner(Player winner);
        // begin the turn of the current player
        void BeginTurn(float time);
        // initialise the game screen
        void BeginGame(float Timer);
        // Updates the connection between two nodes
        void UpdateConnection(Node node1, Node node2, Connection con);
        // Updates the settlement of a node
        void UpdateSettlement(Node otherNode);
        // Updates the player's position over a path
        void UpdatePlayer(Stack<Node> otherNode);
        // Shows the user that a resource has been added to their inventory from a node
        void ShowResource(Vector3 u, Resource resource, Vector3 optional);
        // tests a condition and raises an error if it is not met
        void Assert(bool test);
    }

    /// <summary> 
    /// Discontinued when project became entirely Unity event-based
    /// </summary>
    public class TerminalUI { } 

}

