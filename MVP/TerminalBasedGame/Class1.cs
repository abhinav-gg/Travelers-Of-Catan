using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App
{
    public static class MainFunc
    {

        public static void Main(string[] args)
        {


            TerminalBasedGame.HexagonUnit i = new TerminalBasedGame.HexagonUnit(new TerminalBasedGame.Resource());
            i.position = new Vector3(0, 0, 0);
            Console.WriteLine(i.GetNeighbours());

            //

            Console.WriteLine("Welcome to Game! Key [ENTER] to begin...");
            Console.ReadLine();
            GamePlayLoop game = new GamePlayLoop(2);
            game.startGame();
        }


        public static Vector3 GetNextPosition()
        {
            string playerPos =  Console.ReadLine();
            string[] pos = playerPos.Split(',');
            return new Vector3(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(pos[2]));
        }

    }


    class GamePlayLoop
    {
        int turn = 0;
        int MAXplayer = 0;
        public int[] victoryPoints;

        public GamePlayLoop(int MAXplayer)
        {
            this.MAXplayer = MAXplayer;
            victoryPoints = new int[MAXplayer];
        }


        public void CreateGrid() 
        {


        }

        public void startGame()
        {

            Console.WriteLine("Game Started");
            while (true)
            {
                
                takeTurn();
                
            }
        }

        public void takeTurn()
        {
            string input = "";
            turn++;
            turn = turn % MAXplayer;
            Console.WriteLine("Player " + turn + "'s turn");
            input = Console.ReadLine();

        }


    }

}


// Path: Class1.cs
namespace TerminalBasedGame
{



    class Node
    {
        public Vector3 position;
        private BuildingStatus status;

        public Node(BuildingStatus status)
        {
            this.status = status;
        }


        public IEnumerable<Vector3> GetNeighbours()
        {

            // determine parity of position
            int sum = (int)(position.X + position.Y + position.Z);
            if (sum % 2 == 0)
            {
                // even
                
                
                yield return position + new Vector3(-1, 0, 0);
                yield return position + new Vector3(0, 1, 0);
                yield return position + new Vector3(0, 0, -1);

            }
            else
            {
                yield return position + new Vector3(1, 0, 0);
                yield return position + new Vector3(0, -1, 0);
                yield return position + new Vector3(0, 0, 1);

            }
            yield return new Vector3();
        }


        public static bool inRange(Vector3 pos)
        {
            // asign constants
            bool inRangei = pos.X > -9 && pos.X < 0;
            bool inRangej = pos.Y > -6 && pos.X < 0;
            bool inRangek = pos.Z > -5 && pos.X < 5;
            return inRangei && inRangej && inRangek;
        }


    }

    class HexagonUnit
    {
        public Vector3 position;
        private Resource m_resource;
        private Node[] nodes = new Node[6];

        public HexagonUnit(Resource R)
        {
            m_resource = R;


            for (int i = 0; i < 6; i++)
            {
                nodes[i] = new Node(new BuildingStatus(0));
            }

        }

        public IEnumerable<Vector3> GetNeighbours()
        {
            yield return position + new Vector3(1, 0, 0);
            yield return position + new Vector3(0, 1, 0);
            yield return position + new Vector3(0, 0, 1);
            yield return position + new Vector3(-1, 0, 0);
            yield return position + new Vector3(0, -1, 0);
            yield return position + new Vector3(0, 0, -1);

        }  

       
    }

    class Board
    {

        private HexagonUnit[] board;

        public Board()
        {
            for (int i = 0; i < 25; i++)
            {
                board[i] = new HexagonUnit(new Resource(0));

                //board[i].position = new Vector3(i % 5, i / 5, 0);

            }
        }

        public HexagonUnit GetUnitAtPosition(Vector3 pos)
        {
            foreach (HexagonUnit unit in board)
            {
                if (unit.position == pos)
                {
                    return unit;
                }
            }
            return null;
        }



    }

    class BuildingStatus
    {
        private int i;
        private string[] statuses = { "Empty", "City", "Village", "Highway Man" };

        public BuildingStatus(int i)
        {
            this.i = i;
        }

    }

    class ConnectionStatus
    {
        private int i;
        private string[] statuses = { "Empty", "Road", "Wall" };

        public ConnectionStatus(int i)
        {
            this.i = i;
        }

    }



    class Resource
    {
        private int i;
        private string[] resources = { "Wood", "Brick", "Wheat", "Sheep", "Ore" };

        public Resource(int i)
        {
            this.i = i;
        }

        public Resource()
        {
            CreateRandomResource();
        }

        public string getResourceString()
        {
            return resources[i];
        }

        public void CreateRandomResource() 
        { 
            Random rnd = new Random();
            i = rnd.Next(0, resources.Length);
        }

    }




}
