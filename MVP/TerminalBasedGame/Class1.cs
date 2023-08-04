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

            //

            Console.WriteLine("Welcome to Game! Key [ENTER] to begin...");
            Console.ReadLine();
            GamePlayLoop game = new GamePlayLoop(2);
            game.startGame();
        }

        public static Vector3 GetNextPosition()
        {

            Console.WriteLine("Enter your next position in the form of x,y,z");
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

        public static int ConvertToVictoryPoints(string entityName)
        {

            IDictionary<string, int> victoryPoints = new Dictionary<string, int>()
            {
                {"Road", 3}, 
                {"Wall", 2},
                {"Village", 5 },
                {"City", 10 }
            }; // store as JSON in Unity Version



            return victoryPoints[entityName];


        }

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

            Vector3 inp;
            Console.WriteLine("Player " + turn + "'s turn");
            inp = MainFunc.GetNextPosition();



            turn++;
            turn = turn % MAXplayer;
            

        }

    }



    class Player
    {
        private int playerNumber;
        private int victoryPoints;
        private int[] resources;
        private int[] buildings;
        private int[] connections;

        public Player(int playerNumber)
        {
            this.playerNumber = playerNumber;
            victoryPoints = 0;
            resources = new int[5];
            buildings = new int[4];
            connections = new int[3];
        }

        public void addVictoryPoints(int points)
        {
            victoryPoints += points;
        }

        public void addResource(int resource, int amount)
        {
            resources[resource] += amount;
        }

        public void addBuilding(int building, int amount)
        {
            buildings[building] += amount;
            GamePlayLoop.ConvertToVictoryPoints(building.ToString());//find out where to get string name

        }

        public void addConnection(int connection, int amount)
        {
            connections[connection] += amount;
        }

        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        public int getResource(int resource)
        {
            return resources[resource];
        }

        public int getBuilding(int building)
        {
            return buildings[building];
        }

        public int getConnection(int connection)
        {
            return connections[connection];
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
        }


        public static bool inRange(Vector3 pos)
        {
            // i will probably just hard code all of these values in the game so this isnt needed for the final version

            /// i range -8 - 0
            /// j range 
            /// k range -5 - 0

            // asign constants
            bool inRangei = pos.X > -9 && pos.X < 0;
            bool inRangej = pos.Y > -6 && pos.X < 0;
            bool inRangek = pos.Z > -5 && pos.X < 5;
            return inRangei && inRangej && inRangek;
        }

        public static bool isNeighbours(Node pos1, Node pos2)
        {

            if (pos1.GetNeighbours().Contains(pos2.position))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }


    }

    class HexagonUnit
    {
        public Vector3 position;
        public Resource resource;

        public HexagonUnit(Resource R, int x, int y, int z)
        {
            resource = R;
        }


    }

    class Board
    {

        private HexagonUnit[] board = new HexagonUnit[19];

        public Board()
        {
            int i = 0;
            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    for (int z = -2; z < 3; z++)
                    {
                        if (x + y + z == 0)
                        {
                            HexagonUnit unit = new HexagonUnit(new Resource(), x, y, z);
                            board[i] = unit;
                            i++;
                        }
                    }
                }
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
            return null; // if no unit is found on grid
        }

        public void ShowBoard()
        {
            foreach (HexagonUnit unit in board)
            {
                Console.WriteLine(unit);
                Console.WriteLine(unit.position);
                Console.WriteLine(unit.resource);
            }
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
        private string[] resources = { "Wood", "Brick", "Wheat", "Resource4", "Resource5" };

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
