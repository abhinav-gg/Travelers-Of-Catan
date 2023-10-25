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

        int GetUserLetterInput(int options); // work to decomition
        int GetUserChoice(Object[] options); 
        string GetUserNameInput(int who);
        bool GetUserConfirm();
        void CreatePopup(string message);
        void DisplayPlayers(Player[] players);
        void UpdateBoard(Board board);
        void UpdateBoardConnections(Board board);
        void HandleWinner(Player winner);

    }




    public class TerminalUI //: UI
    {

        /*
        public static Vector3 GetUserPositionInput()
        {
            string[] pos;
            Vector3 position;
            Console.WriteLine("Enter The Position x,y,z or -1 to exit:");
            string playerPos =  Console.ReadLine();
            if (playerPos == "-1")
            {
                return new Vector3(-1000,0,0); // exit code
            }
            try
            {
                pos = playerPos.Split(',');
                position = new Vector3(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(pos[2]));
            }
            catch
            {
                Console.WriteLine("Invalid input, please try again");
                return GetUserPositionInput();
            }
            return position;
        }


        int GetUserPlayerInput(int MaxPlayer)
        {
            int player;
            Console.WriteLine($"Enter The Player 1-{MaxPlayer}");
            string p = Console.ReadLine();
            try
            {
                player = int.Parse(p);
            }
            catch
            {
                Console.WriteLine("Invalid input, please try again");
                return GetUserPlayerInput(MaxPlayer);
            }
            return player;
        }

        public int GetUserLetterInput(int options)
        {
            string letter;
            Console.WriteLine($"Enter The Letter A-{(char)(options + 64)}");
            letter = Console.ReadLine();
            try
            {
                int i = letter.ToUpper()[0] - 64;
                if (letter.Length > 1 || i<0 || i>options)
                {
                    throw new ArgumentOutOfRangeException("User Input Out Of Range");
                }
                return i;
            }
            catch
            {
                Console.WriteLine("Invalid input, please try again");
                return GetUserLetterInput(options);
            }
            
        }


        public int GetUserChoice(Object[] options)
        {

            string letter;
            int i = 1;
            foreach (Object option in options)
            {
                Console.WriteLine($" {(char)(i + 64)}) {option}");
                i++;
            }
            Console.WriteLine($"Enter The Letter A-{(char)(options.Length + 64)}");
            letter = Console.ReadLine();
            try
            {
                i = letter.ToUpper()[0] - 64;
                if (letter.Length > 1 || i < 0 || i > options.Length)
                {
                    throw new ArgumentOutOfRangeException("User Input Out Of Range");
                }
                return i;
            }
            catch
            {
                Console.WriteLine("Invalid input, please try again");
                return GetUserChoice(options);
            }

        }

        public string GetUserNameInput(int who)
        {
            string input;
            Console.WriteLine($"Enter Player {who}'s Name: ");
            input = Console.ReadLine();
            return input;
        }



        public bool GetUserConfirm() {             
            string input;
            Console.WriteLine($"Enter Y/N:");
            input = Console.ReadLine();
            if (input.ToUpper() == "Y")
            {
                return true;
            }
            else if (input.ToUpper() == "N")
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid input, please try again");
                return GetUserConfirm();
            }
        }

        public void CreatePopup(string message)
        {
            Console.WriteLine(message);
        }

    */
    }
}

