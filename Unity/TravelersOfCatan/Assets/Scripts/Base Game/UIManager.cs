
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
        void SaveGame();
        void LoadGame(string Save);

    }

    public class TerminalUI { }

}

