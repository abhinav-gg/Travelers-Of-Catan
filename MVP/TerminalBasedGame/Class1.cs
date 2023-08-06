using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalBasedGame;
using App;

namespace App
{
    public static class MainFunc
    {

        public static void Main(string[] args)
        {

            //Console.WriteLine(Connection.Hash(new Vector3(1, 0, -1), new Vector3(-1, 0, 1)));
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
        public Board board;

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
            board = new Board();
            board.ShowBoard();
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
        private Node[] buildings; // Node
        private Connection[] connections;

        public Player(int playerNumber)
        {
            this.playerNumber = playerNumber;
            victoryPoints = 0;
            resources = new int[5];
        }

        public void addVictoryPoints(int points)
        {
            victoryPoints += points;
        }

        public void addResource(int resource, int amount)
        {
            resources[resource] += amount;
        }

        public void addBuilding(Node building, int amount)
        {
            //buildings[building] += amount;
            GamePlayLoop.ConvertToVictoryPoints(building.ToString());//find out where to get string name

        }

        public void addConnection(int connection, int amount)
        {
            //connections[connection] += amount;
        }

        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        public int getResource(int resource)
        {
            return resources[resource];
        }

    }


}


// Path: Class1.cs
namespace TerminalBasedGame
{



    class Node
    {
        public Vector3 position;
        public BuildingStatus status = new BuildingStatus();

        public Node(int x, int y, int z)
        {
            position = new Vector3(x, y, z);
        }


        public IEnumerable<Vector3> GetNeighbours()
        {

            // determine parity of position
            int sum = (int)(position.X + position.Y + position.Z);
            if (sum % 2 == 0)
            {
                
                yield return position + new Vector3(1, 0, 0);
                yield return position + new Vector3(0, 1, 0);
                yield return position + new Vector3(0, 0, 1);

            }
            else
            {
                yield return position + new Vector3(-1, 0, 0);
                yield return position + new Vector3(0, -1, 0);
                yield return position + new Vector3(0, 0, -1);

            }
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

        public override string ToString()
        {
            return $"{status} at {position}";
        }


    }

    class HexagonUnit
    {
        public Vector3 position;
        public Resource resource;

        public HexagonUnit(Resource R, int x, int y, int z)
        {
            resource = R;
            position = new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return $"{resource} at {position}";
        }

    }

    class Board
    {

        private HexagonUnit[] board = new HexagonUnit[19];
        private Node[] nodes = new Node[54];


        // adjacenecy list for the connections between nodes in the board with a default state of new Connection() which can be updated as the game progresses
        public Dictionary<string, Connection> connections = new Dictionary<string, Connection>();
        // string key will be the return from the Connection.Hash() function

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

            i = 0;
            for (int x = -2; x < 4; x++)
            {
                for (int y = -2; y < 4; y++)
                {
                    for (int z = -2; z < 4; z++)
                    {
                        if ((x + y + z == 1) || (x + y + z == 2))
                        {
                            Node n = new Node(x, y, z);
                            nodes[i] = n;
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
            return null;
        }

        public void ShowBoard()
        {

            Console.WriteLine("Hexes:");

            foreach (HexagonUnit unit in board)
            {
                Console.WriteLine(unit);
            }

            Console.WriteLine("Nodes:");

            foreach (Node u in nodes)
            {
                Console.WriteLine(u);
            }
        }

    }

    class BuildingStatus
    {
        private int i;
        private string[] statuses = { "Empty", "City", "Village", "Highway Man" };
        private Player occupant;



        public BuildingStatus(int i = 0)
        {
            this.i = i;
        }


        public override string ToString()
        {
            return statuses[i];
        }

    }

    class Connection
    {
        public static readonly string[] statuses = { "Empty", "Road", "Wall" };
        private int i = 0;
        private Player occupant;


        public static string Hash(Vector3 v1, Vector3 v2)
        {
            return $"{Math.Min(v1.X, v2.X)}{Math.Max(v1.X, v2.X)},{Math.Min(v1.Y, v2.Y)}{Math.Max(v1.Y, v2.Y)},{Math.Min(v1.Z, v2.Z)}{Math.Max(v1.Z, v2.Z)}";
        }

        public Connection(int i, Player occupant)
        {
            this.i = i;
            this.occupant = occupant;
        }


    }



    class Resource
    {
        public static readonly string[] resources = { "Wood", "Brick", "Wheat", "Resource4", "Resource5" };
        private static readonly Random rng = new Random();
        private int i;


        public Resource(int i)
        {
            this.i = i;
        }

        public Resource()
        {
            CreateRandomResource();
        }

        public override string ToString() 
        { 
            return resources[i];
        }


        public void CreateRandomResource() // turn this into a static generator
        { 
            // Console.WriteLine(rnd.Next(0, resources.Length));
            i = rng.Next(0, resources.Length);
        }

    }

}
