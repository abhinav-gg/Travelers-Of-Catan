using System;
using System.Collections.Generic;
using System.Numerics;

namespace NEAGame
{


    public interface UI
    {
        float GetTimer();
        void GetUserNodeChoice(Node[] options, Action<Node> method);
        bool GetUserConfirm();
        void CreatePopup(string message);
        void DisplayBoard(Board board);
        void HandleWinner(Player winner);
        void SaveGame();
        void LoadGame(string Save);
        void BeginTurn(float time);
        void UpdatePlayer(Node otherNode);
        void UpdateConnection(Node node1, Node node2);
        void UpdateSettlement(Node otherNode);
        void DisplayPlayers(List<Player> gamePlayers);
    }

    public class TerminalUI { }

}

