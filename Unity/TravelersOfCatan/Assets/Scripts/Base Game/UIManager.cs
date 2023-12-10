using System;
using System.Numerics;

namespace NEAGame
{


    public interface UI
    {

        void GetUserNodeChoice(Node[] options, Action<Node> method);
        bool GetUserConfirm();
        void CreatePopup(string message);
        void DisplayBoard(Board board);
        void HandleWinner(Player winner);
        void SaveGame();
        void LoadGame(string Save);
        void BeginTurn();
        void UpdatePlayer(Node otherNode);
        void UpdateConnection(Node otherNode);
        void UpdateSettlement(Node otherNode);

    }

    public class TerminalUI { }

}

