using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App;
using System.Collections;
using System.Diagnostics.Contracts;


// Path: Class1.cs
namespace NEAGame
{

    class TravelersOfCatan
    {

        private readonly Vector3[] StartingCoords = new Vector3[] {
            new Vector3( 0,  3, -2),
            new Vector3( 1, -2,  3),
            new Vector3( 3, -1, -1),
            new Vector3(-2,  2,  2) 
        };
        private readonly int WinningVictoryPoints = 10;

        private Board board;
        private int turn = 0;
        private int MAXplayer = 0;
        private int[] victoryPoints;
        private Player[] gamePlayers;
        private Player currentPlayer;

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

        public static Dictionary<int, int> GetCostOfUpgrade(string entityName)
        {
            //        public static readonly string[] resources = { "Wood", "Brick", "Wheat", "Sheep", "Ore" };

            Dictionary<string, Dictionary<int, int>> purchaseCost = new Dictionary<string, Dictionary<int, int>>()
            {
                {"Road", new Dictionary<int, int>()     { { 0, 1 }, { 1, 1 }, { 2, 0 }, { 3, 1 }, { 4, 0 } } },
                {"Wall", new Dictionary<int, int>()     { { 0, 0 }, { 1, 5 }, { 2, 0 }, { 3, 0 }, { 4, 0 } } },
                {"Village", new Dictionary<int, int>()  { { 0, 3 }, { 1, 1 }, { 2, 3 }, { 3, 3 }, { 4, 2 } } },
                {"City", new Dictionary<int, int>()     { { 0, 1 }, { 1, 3 }, { 2, 1 }, { 3, 0 }, { 4, 4 } } }  
            };
            // store as JSON in Unity Version
           
            return purchaseCost[entityName];

        }

        public TravelersOfCatan(int MAXplayer)
        {
            this.MAXplayer = MAXplayer;
            victoryPoints = new int[MAXplayer];
            gamePlayers = new Player[MAXplayer];

            foreach (int i in Enumerable.Range(0, MAXplayer))
            {
                gamePlayers[i] = new Player(i, TerminalGame.GetUserNameInput(i+1), StartingCoords[i]);
            }

        }


        public void startGame()
        {

            bool gameOngoing = true;

            board = new Board();
            board.ShowBoard();
            Console.WriteLine("Game Started");
            while (gameOngoing)
            {
                currentPlayer = gamePlayers[turn];

                takeTurn();

            }
        }

        public void CheckWinner()
        {
            foreach (Player p in gamePlayers)
            {
                if (p.getVictoryPoints() >= WinningVictoryPoints)
                {
                    Console.WriteLine("Player " + p.playerName + " has won the game");
                    return;
                }
            }
        }

        public void takeTurn()
        {

            currentPlayer.hasMoved = false;
            Console.WriteLine($"{currentPlayer.playerName}'s turn");

            gatherResources();


            while (true)
            {
                Console.WriteLine("Enter Option:\n a) Show Position\n b) Move Player\n c) Show Victory Points\n d) Check Inventory\n e) Make Purchase\n f) Offer Trade\n g) End Turn");
                int response = TerminalGame.GetUserLetterInput(7);

                switch (response)
                {
                    case 1:
                        Console.WriteLine($"You are currently at {currentPlayer.position}");
                        break;



                    case 2:
                        if (currentPlayer.hasMoved)
                        {
                            Console.WriteLine("You have already moved this turn");
                            break;
                        }
                        movePlayer();
                        break;

                    case 3:
                        Console.WriteLine("You have " + currentPlayer.getVictoryPoints() + " victory points");
                        break;


                    case 4:
                        foreach (var entry in currentPlayer.getResources())
                        {
                            Console.WriteLine($"You have {entry.Value} {entry.Key}");
                        }
                        break;

                    case 5:

                        // hard part


                        CheckWinner();
                        break;


                    case 6:
                        break; // not a mandatory feature so may be implemented at a later stage

                    case 7:
                        turn++;
                        turn = turn % MAXplayer;
                        Console.WriteLine("Ending Turn");
                        return;
                }


            }

        }

        public void gatherResources()
        {
            foreach (Vector3 u in board.GetNodeAtPosition(currentPlayer.position).GetHexNeighbours())
            {
                if (board.GetHexAtPosition(u) != null)
                {
                    currentPlayer.addResource(board.GetHexAtPosition(u).resource);
                }
            }

        }

        public void movePlayer()
        {
            bool valid = false;
            Vector3 inp;
            while (!valid)
            {
                Console.WriteLine("Where would you like to move?");
                inp = TerminalGame.GetUserPositionInput();

                if (inp.X == 1000) { return; } // Exit Code

                if (board.GetNodeAtPosition(currentPlayer.position).GetNodeNeighbours().ToList().Contains(inp) && board.GetNodeAtPosition(inp) != null)
                {
                    currentPlayer.position = inp;
                    Console.WriteLine("You have moved to " + currentPlayer.position);
                    valid = true;
                }

                else
                {
                    Console.WriteLine("That position is not valid");
                }
            }
            currentPlayer.hasMoved = true;
        }
            
    }

    class Player
    {
        private int victoryPoints;
        private Dictionary<Resource, int> resources = new Dictionary<Resource, int>();
        private List<Node> buildings = new List<Node>(); // Node
        private List<Connection> connections;
        private int playerNumber;
        public string playerName;
        public bool hasMoved = false;

        public Vector3 position;

        public Player(int playerNumber, string playerName, Vector3 origin)
        {
            this.playerNumber = playerNumber;
            this.playerName = playerName;
            victoryPoints = 0;
            position = origin;
        }

        public void addVictoryPoints(int points)
        {
            victoryPoints += points;
        }


        public void addBuilding(Node building)
        {

            TravelersOfCatan.ConvertToVictoryPoints(building.ToString());
            

        }

        public void addConnection(int connection, int amount)
        {
            //connections[connection] += amount;
        }

        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        public void makePurchase() { } //requires parameter of a map and shoudl deduct necessary resources from player
        public void addResource(Resource resource)
        {
            if (!resources.ContainsKey(resource))
            {
                resources.Add(resource, 0);
            }
            resources[resource] += 1;
        }

        public Dictionary<Resource, int> getResources()
        {
            return resources;
        }

    }


    class Node
    {
        public Vector3 position;
        public BuildingStatus status = new BuildingStatus();

        public Node(int x, int y, int z)
        {
            position = new Vector3(x, y, z);
        }


        public IEnumerable<Vector3> GetNodeNeighbours()
        {

            // determine parity of position
            int sum = (int)(position.X + position.Y + position.Z);
            if (sum % 2 == 1)
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

        public IEnumerable<Vector3> GetHexNeighbours()
        {
            int sum = (int)(position.X + position.Y + position.Z);
            if (sum % 2 == 1)
            {

                yield return position + new Vector3(-1, 0, 0);
                yield return position + new Vector3(0, -1, 0);
                yield return position + new Vector3(0, 0, -1);

            }
            else
            {
                yield return position + new Vector3(-1, -1, 0);
                yield return position + new Vector3(0, -1, -1);
                yield return position + new Vector3(-1, 0, -1);

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

        public HexagonUnit GetHexAtPosition(Vector3 pos)
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

        public Node GetNodeAtPosition(Vector3 pos) 
        {             
            foreach (Node node in nodes)
            {
                if (node.position == pos)
                {
                    return node;
                }
            }
            return null;
        }

        public void ShowBoard() // move function to class1.cs
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
        public static readonly string[] resources = { "Wood", "Brick", "Wheat", "Sheep", "Ore" };
        private static readonly Random rng = new Random();
        private int i;


        public Resource(int i)
        {
            this.i = i;
        }
        public Resource(string i)
        {
            this.i = Array.IndexOf(resources, i);
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
