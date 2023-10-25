using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEAGame;


namespace App
{


    public interface UI
    {

        Node GetUserNodeChoice(Node[] options);
        string GetUserNameInput(int who);
        bool GetUserConfirm();
        void CreatePopup(string message);
        void DisplayPlayers(Player[] players);
        void UpdateBoard(Board board);
        void UpdateBoardConnections(Board board);
        void HandleWinner(Player winner);

    }

    public class TerminalUI { }

}

