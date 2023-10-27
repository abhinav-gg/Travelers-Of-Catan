
using System.Collections.Generic;
using NEAGame;


namespace App
{


    public interface UI
    {

        Node GetUserNodeChoice(Node[] options);
        bool GetUserConfirm();
        void CreatePopup(string message);
        void DisplayPlayers(List<Player> players);
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

