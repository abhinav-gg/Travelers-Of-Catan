
using System;
using System.Collections.Generic;


namespace NEAGame
{


    public interface UI
    {

        void GetUserNodeChoice(Node[] options, Action<Node> method);
        bool GetUserConfirm();
        void CreatePopup(string message);
        //void DisplayPlayers(List<Player> players);
        void UpdateBoard(Board board);
        void UpdateBoardConnections(Board board);
        void HandleWinner(Player winner);
        void SaveGame();
        void LoadGame(string Save);
        void BeginTurn();
        void ShowCost(string ID);
    }

    public class TerminalUI { }

}

