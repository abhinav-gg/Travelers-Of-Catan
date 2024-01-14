using System;
using System.Collections.Generic;
using System.Numerics;

namespace NEAGame
{


    public interface UI
    {
        float GetTimer();
        void GetUserNodeChoice(Node[] options, Action<Node> method);
        void CreatePopup(string message);
        void HandleWinner(Player winner);
        void BeginTurn(float time);
        void BeginGame(bool wasGameLoaded, float Timer);
        void UpdateConnection(Node node1, Node node2, Connection con);
        void UpdateSettlement(Node otherNode);
        void UpdatePlayer(Stack<Node> otherNode);
        void ShowResource(Vector3 u, Resource resource, Vector3 optional);
        void Assert(bool test);
    }

    public class TerminalUI { }

}

