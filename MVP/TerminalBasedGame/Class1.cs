using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEAGame;


namespace App
{
    public static class TerminalGame
    {

        public static void Main(string[] args)
        {

            //Console.WriteLine(Connection.Hash(new Vector3(1, 0, -1), new Vector3(-1, 0, 1)));
            //


            // change to accept player count
            Console.WriteLine("Welcome to Game! Key [ENTER] to begin...");
            Console.ReadLine();
            TravelersOfCatan game = new TravelersOfCatan(2); // 2 player game



            game.startGame();
        }

        public static Vector3 GetUserPositionInput()
        {
            string[] pos;
            Vector3 position;
            Console.WriteLine("Enter The Position x,y,z or -1 to exit:");
            string playerPos =  Console.ReadLine();
            if (playerPos == "-1")
            {
                return new Vector3(-1000,0,0);
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


        public static int GetUserPlayer(int MaxPlayer)
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
                return GetUserPlayer(MaxPlayer);
            }
            return player;
        }

        public static int GetUserLetterInput(int options)
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

        public static string GetUserNameInput(int who)
        {
            string input;
            Console.WriteLine($"Enter Player {who}'s Name: ");
            input = Console.ReadLine();
            return input;
        }



        public static bool GetUserConfirm() {             
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

    }

}

