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

        public static string GetPurchaseID(int i)
        {
            return new string[] { "Road", "Wall", "Village", "City" }[i];
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

            Console.WriteLine("Loarding Board...");

            board = new Board();

            foreach (Player current in gamePlayers)
            {
                // IMPORTANT: players start with resources and buildings

                BuildingStatus capital = new BuildingStatus("City", current);
                board.GetNodeAtPosition(current.position).status = capital;

                foreach (int j in Enumerable.Range(0, 5))
                {
                    current.addResource(new Resource(j), 10);
                }


            }


            Console.WriteLine("Game Started");
            board.ShowBoard();


            while (gameOngoing)
            {
                currentPlayer = gamePlayers[turn];
                gatherResources();
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
            while (true)
            {


                Console.WriteLine("Enter Option:\n a) Show Current Position\n b) Move Player\n c) Show Victory Points\n d) Check Inventory\n e) Show Costs\n f) Make Purchase\n g) Show Board\n h) Show Board Connections\n i) Enter Trading\n j) End Turn");


                // add more options and make it easier to read

                int response = TerminalGame.GetUserLetterInput(9);

                switch (response) // TODO: remove switch case into seperate functions as Unity does not support switch case
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

                        // show cost of all items

                        foreach (int i in Enumerable.Range(0, 4))
                        {
                            Console.WriteLine(GetPurchaseID(i) + ":");
                            printCostOfItem(GetPurchaseID(i));
                        }


                        break;


                    case 6:

                        makePurchase();
                        break;


                    case 7:
                        board.ShowBoard();
                        break;

                    case 8:
                        board.ShowBoardConnections();
                        break;

                    case 9:
                        Console.WriteLine("Entering Trades");
                        break; // not a mandatory feature so may be implemented at a later stage

                    default:
                        turn++;
                        turn = turn % MAXplayer;
                        Console.WriteLine("Ending Turn");
                        return;
                }


            }

        }

        public void printCostOfItem(string item)
        {
            Dictionary<int, int> cost = GetCostOfUpgrade(item);
            foreach (var entry in cost)
            {
                Console.WriteLine($"You need {entry.Value} {entry.Key.ToString()}");
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

        public void makePurchase()
        {
            Console.WriteLine("What would you like to purchase?\n a) Road\n b) Wall\n c) Village\n d) City");

            int purchase = TerminalGame.GetUserLetterInput(4) - 1;
            Dictionary<int, int> cost = GetCostOfUpgrade(GetPurchaseID(purchase));
            bool canAfford = true;
            foreach (var entry in cost)
            {
                if (currentPlayer.getResources()[new Resource(entry.Key)] < entry.Value)
                {
                    canAfford = false;
                }
            }
            if (!canAfford)
            {
                Console.WriteLine("You cannot afford this purchase");
            }
            else
            {

                if (!checkIfCanPurchase(GetPurchaseID(purchase)))
                {
                    Console.WriteLine("You cannot purchase this item from your current position");
                    return;
                }

                Vector3 otherpos = new Vector3();
                if (GetPurchaseID(purchase) == "Road" || GetPurchaseID(purchase) == "Wall") // if the purchase is a road or wall, ask for a position to place it
                {
                    bool valid = false;
                    while (!valid)
                    {
                        Console.WriteLine("Where would you like this item to connect to?");
                        otherpos = TerminalGame.GetUserPositionInput();
                        if (board.GetNodeAtPosition(otherpos).GetNodeNeighbours().Contains(currentPlayer.position))
                        {
                            valid = true;
                        }
                        else
                        {
                            Console.WriteLine("You cannot place a road here");
                        }
                        // TODO: give only the valid options

                    }
                }
                else
                {
                    otherpos = currentPlayer.position;
                }

                Console.WriteLine("You are purchasing a " + GetPurchaseID(purchase) + " for:");
                printCostOfItem(GetPurchaseID(purchase));
                if (!TerminalGame.GetUserConfirm()) { return; }

                foreach (var entry in cost)
                {
                    currentPlayer.removeResource(new Resource(entry.Key), entry.Value);
                }

                switch (GetPurchaseID(purchase))
                {
                    case "Road":
                        board.SetConnection(currentPlayer.position, otherpos, "Road", currentPlayer);
                        currentPlayer.addConnection(board.GetConnection(currentPlayer.position, otherpos));
                        break;
                    case "Wall":
                        board.SetConnection(currentPlayer.position, otherpos, "Wall", currentPlayer);
                        currentPlayer.addConnection(board.GetConnection(currentPlayer.position, otherpos));
                        break;
                    case "Village":
                        board.GetNodeAtPosition(currentPlayer.position).status = new BuildingStatus("Village", currentPlayer);
                        currentPlayer.addBuilding(board.GetNodeAtPosition(currentPlayer.position));
                        break;
                    case "City":
                        board.GetNodeAtPosition(currentPlayer.position).status = new BuildingStatus("City", currentPlayer);
                        currentPlayer.addBuilding(board.GetNodeAtPosition(currentPlayer.position));
                        break;
                }

                Console.WriteLine("Purchase Successful");
            }
            CheckWinner();
        }

        public bool checkIfCanPurchase(string item)
        {


            if (item == "Road")
            {
                foreach (Vector3 nPos in board.GetNodeAtPosition(currentPlayer.position).GetNodeNeighbours())
                {
                    if (board.GetNodeAtPosition(nPos) == null) { continue; }
                    if (board.GetNodeAtPosition(nPos).status.occupant == currentPlayer)
                    {
                        if (board.GetConnection(currentPlayer.position, nPos) == null)
                        {
                            return true;
                        }
                        
                    }
                }
            }
            else if (item == "Wall")
            {
                foreach (Vector3 nPos in board.GetNodeAtPosition(currentPlayer.position).GetNodeNeighbours())
                {
                    if (board.GetNodeAtPosition(nPos) == null) { continue; }
                    if (board.GetConnection(currentPlayer.position, nPos) == null)
                    {
                            return true;
                    }
                }
            }


            /// THEORETICALLY group wall and road together and check if there is a connection between the two nodes as this code is common


            else if (item == "Village")
            {

                if (!board.GetNodeAtPosition(currentPlayer.position).isEmpty())
                {
                    Console.WriteLine("You cannot place a village here as there is already an establishment on this Node");
                    return false;
                }
                foreach (Vector3 nPos in board.GetNodeAtPosition(currentPlayer.position).GetNodeNeighbours())
                {
                    Connection con = board.GetConnection(currentPlayer.position, nPos);
                    if (con.GetOccupant() == currentPlayer && con.GetStatus() == "Road")
                    {
                        return true;
                    }

                }

                Console.WriteLine("You cannot place a village here as there is no road connecting to this Node");

            }
            else if (item == "City")
            {
                if ((board.GetNodeAtPosition(currentPlayer.position).status.occupant == currentPlayer) && (board.GetNodeAtPosition(currentPlayer.position).status.ToString() == "Village"))
                { 
                    return true;
                }
            }

            return false;
        }

        public void movePlayer()
        {
            bool valid = false;
            Vector3 inp;
            while (!valid)
            {
                Console.WriteLine("Where would you like to move?");
                inp = TerminalGame.GetUserPositionInput();

                if (inp.X == 1000) { return; } // Exit Code passed from CLI

                if ((board.GetNodeAtPosition(currentPlayer.position).GetNodeNeighbours().ToList().Contains(inp) && board.GetNodeAtPosition(inp) != null) || currentPlayer.playerName == "test") // test is for testing purposes only, remove when live
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
        private Dictionary<Resource, int> resources = new Dictionary<Resource, int>() {
            { new Resource(0), 0 },
            { new Resource(1), 0 },
            { new Resource(2), 0 },
            { new Resource(3), 0 },
            { new Resource(4), 0 }
        };

        /// <summary>
        private List<Node> buildings = new List<Node>(); //
        private List<Connection> connections; //
        /// Unused for now however may be useful for future features
        /// </summary>

        private int playerNumber; // useful as a UID for the player allowing the same name in testing
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

        private void addVictoryPoints(int points)
        {
            victoryPoints += points;
        }


        public void addBuilding(Node building)
        {
            buildings.Add(building);
            addVictoryPoints(TravelersOfCatan.ConvertToVictoryPoints(building.status.ToString()));

        }


        public void addConnection(Connection connection)
        {
            connections.Add(connection);
            addVictoryPoints(TravelersOfCatan.ConvertToVictoryPoints(connection.GetStatus()));
        }

        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        public void addResource(Resource resource, int count = 1)
        {
            resources[resource] += count;
        }

        public void removeResource(Resource resource, int amount = 1)
        {
            resources[resource] -= amount;
        }

        public Dictionary<Resource, int> getResources()
        {
            return resources;
        }

        public override string ToString()
        {
            return playerName;
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

        public bool isEmpty()
        {
            return status.ToString() == "Empty";
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

        public void ShowBoardConnections()
        {
            foreach (KeyValuePair<string, Connection> entry in connections)
            {
                Console.WriteLine(entry.Key + ": " + entry.Value);
            }
        }


        public Connection GetConnection(Vector3 v1, Vector3 v2)
        {
            string key = Connection.Hash(v1, v2);
            if (connections.ContainsKey(key))
            {
                return connections[key];
            }
            else
            {
                return null;
            }
        }


        public void SetConnection(Vector3 v1, Vector3 v2, string status, Player currentPlayer) // weakest function in entire project
        {
            
            string key = Connection.Hash(v1, v2);
            connections.Add(key, new Connection(Connection.GetID(status), currentPlayer));

        }

    }

    class BuildingStatus
    {
        public int i;
        private string[] statuses = { "Empty", "Village", "City", "Highway Man" };
        public Player occupant;



        public BuildingStatus(string i = "Empty", Player o = null)
        {
            this.i = Array.IndexOf(statuses, i);
            occupant = o;
        
        }

        public void UpgradeVillage()
        {
            if (i == 1)
            {
                i ++;
            }
            else
            {
                Console.WriteLine("You can't upgrade a city");
            }
        }


        public override string ToString()
        {
            if (occupant != null)
            {
                return $"{statuses[i]} owned by {occupant}";
            }
            else
            {
                return $"{statuses[i]}";
            }
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

        public static int GetID(string status)
        {
            return Array.IndexOf(statuses, status);
        }

        public Connection(int i, Player occupant)
        {
            this.i = i;
            this.occupant = occupant;
        }

        public Connection()
        {

        }

        public Player GetOccupant()
        {
            return occupant;
        }
        public string GetStatus()
        {
            return statuses[i];
        }


        public override string ToString()
        {
            return $"{statuses[i]} Owmed by {occupant.ToString()}";
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

        public override bool Equals(System.Object otherItem)
        {
            if (otherItem == null)
            {
                return false;
            }

            Resource otherResource = otherItem as Resource;

            return (i == otherResource.i);
        }

        public override int GetHashCode()
        {
            return i;
        }


    }

}
