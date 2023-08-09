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
            Console.WriteLine("Enter The Position x,y,z");
            string playerPos =  Console.ReadLine();
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


    }

}

