using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App;
using System.Collections;
using System.Diagnostics.Contracts;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;


// Path: Class1.cs
namespace NEAGame
{

    class TravelersOfCatan
    {

        public static UI UserInterface;
        public static IDictionary<string, int> victoryPointConvertor = new Dictionary<string, int>()
            {
                {"Road", 2},
                {"Wall", 1},
                {"Village", 4 },
                {"City", 10 }
            };

        private readonly Vector3[] StartingCoords = new Vector3[] {
            new Vector3( 0,  3, -2),
            new Vector3( 1, -2,  3),
            new Vector3( 3, -1, -1),
            new Vector3(-2,  2,  2) 
        };
        private int WinningVictoryPoints = 30; // allow this to be passed into the constructor
        private int StartingResourceCount = 10;

        private int turn = 0;
        private int MAXplayer = 0;
        private Player currentPlayer;
        public int[] victoryPoints;
        public Board board;
        public Player[] gamePlayers;


        public TravelersOfCatan(int MAXplayer, int MAXbot)
        {

            UserInterface = new TerminalUI();

            this.MAXplayer = MAXplayer + MAXbot ;
            victoryPoints = new int[MAXplayer + MAXbot];
            gamePlayers = new Player[MAXplayer + MAXbot];

            foreach (int i in Enumerable.Range(0, MAXplayer))
            {
                gamePlayers[i] = new Player(i, UserInterface.GetUserNameInput(i+1), StartingCoords[i]);
            
            }

            for (int i = 0; i < MAXbot; i++)
            {
                gamePlayers[i + MAXplayer] = new AI(i + MAXplayer, "AI" + (i+1), StartingCoords[i + MAXplayer]);
            }



        }

        public static string GetPurchaseID(int i)
        {
            return new string[] { "Road", "Wall", "Village", "City" }[i];
        }

        public static Dictionary<int, int> GetCostOfUpgrade(string entityName)
        {

            Dictionary<string, Dictionary<int, int>> purchaseCost = new Dictionary<string, Dictionary<int, int>>()
            {
                {"Road", new Dictionary<int, int>()     { { 1, 1 }, { 2, 1 }, { 3, 0 }, { 4, 1 }, { 5, 0 } } },
                {"Wall", new Dictionary<int, int>()     { { 1, 0 }, { 2, 5 }, { 3, 0 }, { 4, 0 }, { 5, 0 } } },
                {"Village", new Dictionary<int, int>()  { { 1, 3 }, { 2, 1 }, { 3, 3 }, { 4, 3 }, { 5, 2 } } },
                {"City", new Dictionary<int, int>()     { { 1, 1 }, { 2, 3 }, { 3, 1 }, { 4, 0 }, { 5, 4 } } }  
            };
            // store as JSON in Unity Version
           
            return purchaseCost[entityName];

        }


        public void startGame()
        {



            DisplayPlayers();
            bool gameOngoing = true;

            //TravelersOfCatan.UserInterface.CreatePopup("Loarding Board...");

            board = new Board();

            foreach (Player current in gamePlayers)
            {

                Building capital = new Building("City", current);
                board.GetNode(current.position).status = capital;
                current.addBuilding(board.GetNode(current.position));

                // give every player their starting resources
                for (int i = 1; i < Resource.resources.Length; i++)
                {

                    current.addResource(new Resource(i), StartingResourceCount);
                    if (current.playerName == "test")
                    {
                        current.addResource(new Resource(i), 1000);
                    }
                }

            }

            // TODO add highwayman 

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

                    TravelersOfCatan.UserInterface.CreatePopup("Player " + p.playerName + " has won the game");
                    Console.ReadLine();
                    Environment.Exit(0);
                    return;
                }
            }
        }

        public void takeTurn() // Terminal version specific
        {

            currentPlayer.moves = 2; // initial moves per turn

            TravelersOfCatan.UserInterface.CreatePopup($"{currentPlayer.playerName}'s turn");
            while (true)
            {


                TravelersOfCatan.UserInterface.CreatePopup(@"Enter Option:
                    a) Show Current Position
                    b) Move Player
                    c) Show Victory Points
                    d) Check Inventory
                    e) Show Costs
                    f) Make Purchase
                    g) Show Board
                    h) Show Board Connections
                    i) Enter Trading
                    j) End Turn");


                // add more options and make it easier to read

                int response = UserInterface.GetUserLetterInput(10); // this will get rewritten in Unity version

                switch (response) // TODO: remove switch case into seperate functions as Unity does not support switch case
                {
                    case 1:
                        TravelersOfCatan.UserInterface.CreatePopup($"You are currently at {currentPlayer.position}");
                        break;

                    case 2:
                        if (currentPlayer.moves == 0)
                        {
                            TravelersOfCatan.UserInterface.CreatePopup("You have already moved this turn");
                            break;
                        }
                        movePlayer();
                        break;

                    case 3:
                        TravelersOfCatan.UserInterface.CreatePopup("You have " + currentPlayer.getVictoryPoints() + " victory points");
                        break;


                    case 4:
                        foreach (var entry in currentPlayer.getResources())
                        {
                            TravelersOfCatan.UserInterface.CreatePopup($"You have {entry.Value} {entry.Key}");
                        }
                        break;

                    case 5:

                        // show cost of all items
                        foreach (int i in Enumerable.Range(0, 4))
                        {
                            TravelersOfCatan.UserInterface.CreatePopup(GetPurchaseID(i) + ":");
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
                        TravelersOfCatan.UserInterface.CreatePopup("Entering Trades");
                        break; // not a mandatory feature so may be implemented at a later stage

                    default:
                        turn++;
                        turn = turn % MAXplayer;
                        TravelersOfCatan.UserInterface.CreatePopup("Ending Turn");
                        return;
                }


            }

        }

        public void printCostOfItem(string item)
        {
            Dictionary<int, int> cost = GetCostOfUpgrade(item);
            foreach (var entry in cost)
            {
                if (entry.Value > 0)
                {
                    TravelersOfCatan.UserInterface.CreatePopup($"You need {entry.Value} {Resource.resources[entry.Key]}");
                }
            }
        }

        public void gatherResources()
        {
            foreach (Vector3 u in board.GetNode(currentPlayer.position).GetHexNeighbours())
            {
                if (board.GetHexAtPosition(u) != null)
                {
                    currentPlayer.addResource(board.GetHexAtPosition(u).resource);
                }
            }

        }

        public void makePurchase()
        {
            TravelersOfCatan.UserInterface.CreatePopup("What would you like to purchase?\n a) Road\n b) Wall\n c) Village\n d) City");

            int purchase = UserInterface.GetUserLetterInput(4) - 1;
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
                TravelersOfCatan.UserInterface.CreatePopup("You cannot afford this purchase");
                return;
            }

            bool success = false;

            switch (GetPurchaseID(purchase))
            {
                case "Road":
                    success = purchaseRoad();
                    break;
                case "Wall":
                    success = purchaseWall();
                    break;
                case "Village":
                    success = purchaseVillage();
                    break;
                case "City":
                    success = purchaseCity();
                    break;

            }
            if (success)
            {

                foreach (var entry in cost)
                {
                    currentPlayer.removeResource(new Resource(entry.Key), entry.Value);
                }
                TravelersOfCatan.UserInterface.CreatePopup("Purchase Successful");
                CheckWinner();

            }
            else
            {
                TravelersOfCatan.UserInterface.CreatePopup("Purchase Failed");
            }

        }

        public bool purchaseRoad()
        {
            List<Node> viableLocations = new List<Node>();
            bool canconnect = board.GetNode(currentPlayer.position).status.GetOccupant() == currentPlayer; // player must be standing on their own settlement to build a road
            foreach (Connection con in board.GetNode(currentPlayer.position).GetConnections())
            {
                if (con.start.position != currentPlayer.position)
                {
                    Console.WriteLine("Something went wrong..."); // this should never happen
                }
                if (con.GetOccupant() != null)
                {
                    continue; // can not build a road on existing connections of any sort
                }
                else if ((con.end.status.GetOccupant() != currentPlayer) && (con.end.status.GetOccupant() != null))
                {
                    continue; // The enemy controls the settlement at the end of this road
                }
                else
                {
                    viableLocations.Add(con.end);
                }

            }

            Node otherpos;

            if (viableLocations.Count > 1 && canconnect)
            {
                TravelersOfCatan.UserInterface.CreatePopup("Where would you like this road to connect?");
                int index = UserInterface.GetUserChoice(Array.ConvertAll(viableLocations.ToArray(), x => (object)x)) - 1;
                otherpos = viableLocations[index];
                
            }
            else if (viableLocations.Count == 1 && canconnect)
            {
                otherpos = viableLocations[0];
                TravelersOfCatan.UserInterface.CreatePopup($"You are purchasing a Road at {otherpos}");
            }
            else
            {
                TravelersOfCatan.UserInterface.CreatePopup("You cannot purchase a road here");
                return false;
            }

            TravelersOfCatan.UserInterface.CreatePopup("You are purchasing a Road which costs the following:");
            printCostOfItem("Road");
            TravelersOfCatan.UserInterface.CreatePopup("Are you sure you want to purchase this?");
            if (!UserInterface.GetUserConfirm()) { return false; }

            board.UpdateConnection(currentPlayer.position, otherpos.position, "Road", currentPlayer);
            currentPlayer.addConnection(board.GetConnection(currentPlayer.position, otherpos.position));

            return true;

        }

        public bool purchaseWall()
        {
            List<Node> viableLocations = new List<Node>();
            foreach (Connection con in board.GetNode(currentPlayer.position).GetConnections())
            {
                if (con.start.position != currentPlayer.position)
                {
                    Console.WriteLine("Something went wrong..."); // this should never happen
                }
                if (con.GetOccupant() != null)
                {
                    continue; // can not build a wall on existing connections of any sort
                }
                else // do not care who owns the settlement at the end of this wall
                {
                    viableLocations.Add(con.end);
                }
            }

            Node otherpos;

            if (viableLocations.Count > 1)
            {
                TravelersOfCatan.UserInterface.CreatePopup("Where would you like this wall to connect?");
                int index = UserInterface.GetUserChoice(Array.ConvertAll(viableLocations.ToArray(), x => (object)x)) - 1;
                otherpos = viableLocations[index];

            }
            else if (viableLocations.Count == 1)
            {
                otherpos = viableLocations[0];
                TravelersOfCatan.UserInterface.CreatePopup($"You are purchasing a Wall at {otherpos}");
            }
            else
            {
                TravelersOfCatan.UserInterface.CreatePopup("You cannot purchase a wall here");
                return false;
            }

            TravelersOfCatan.UserInterface.CreatePopup("You are purchasing a permanent Wall which costs the following:");
            printCostOfItem("Wall");
            TravelersOfCatan.UserInterface.CreatePopup("Are you sure you want to purchase this?");
            if (!UserInterface.GetUserConfirm()) { return false; }

            board.UpdateConnection(currentPlayer.position, otherpos.position, "Wall", currentPlayer);
            currentPlayer.addConnection(board.GetConnection(currentPlayer.position, otherpos.position));

            return true;

        }

        public bool purchaseVillage()
        {
            if (!board.GetNode(currentPlayer.position).isEmpty())
            {
                UserInterface.CreatePopup("You cannot place a village here as there is already an establishment on this Node");
                return false;
            }

            bool isConnected = false;
            foreach (Vector3 nPos in board.GetNode(currentPlayer.position).GetNodeNeighbours())
            {
                Connection con = board.GetConnection(currentPlayer.position, nPos);
                if (con.GetOccupant() == currentPlayer && con.GetStatus() == "Road")
                {
                    isConnected = true;
                }

            }
            if (!isConnected)
            {

                TravelersOfCatan.UserInterface.CreatePopup("You cannot place a village here as there is no road connecting to this Node");
                return false;
            }

            TravelersOfCatan.UserInterface.CreatePopup("You are purchasing a Village which costs the following:");
            printCostOfItem("Village");
            TravelersOfCatan.UserInterface.CreatePopup("Are you sure you want to purchase this?");
            if (!UserInterface.GetUserConfirm()) { return false; }

            board.GetNode(currentPlayer.position).status = new Building("Village", currentPlayer);
            currentPlayer.addBuilding(board.GetNode(currentPlayer.position));

            return true;

        }

        public bool purchaseCity()
        {

            if (!(board.GetNode(currentPlayer.position).status.ToString() == "Village"))
            { 

                TravelersOfCatan.UserInterface.CreatePopup("You cannot place a city here as you do not own a village on this Node");
                return false;
                
            }

            TravelersOfCatan.UserInterface.CreatePopup("You are purchasing a City which costs the following:");
            printCostOfItem("City");
            TravelersOfCatan.UserInterface.CreatePopup("Are you sure you want to purchase this?");
            if (!UserInterface.GetUserConfirm()) { return false; }

            board.GetNode(currentPlayer.position).status.UpgradeVillage();
            currentPlayer.addBuilding(board.GetNode(currentPlayer.position));

            return true;
        }

        public void movePlayer()
        {

            List<Node> viableLocations = new List<Node>();

            foreach (Connection con in board.GetNode(currentPlayer.position).GetConnections())
            {
                if ((con.GetOccupant() != currentPlayer) && (con.GetOccupant() != null)) 
                {
                    continue; // the enemy controls this road or wall
                }
                else if ((con.end.status.GetOccupant() != currentPlayer) && (con.end.status.GetOccupant() != null))
                {
                    continue; // The enemy controls the settlement at the end of this road
                }

                viableLocations.Add(con.end);
                    
            }

            if (viableLocations.Count == 0)
            {
                TravelersOfCatan.UserInterface.CreatePopup("Something went wrong... Sending you to your capital");
                currentPlayer.position = currentPlayer.GetCapital().position;
                return; // this should not happen unless you get boxed in by the clever opponent!
            }

            TravelersOfCatan.UserInterface.CreatePopup("Where would you like to move?");
            int index = UserInterface.GetUserChoice(Array.ConvertAll(viableLocations.ToArray(), x => (object)x)) - 1;
            TravelersOfCatan.UserInterface.CreatePopup("Confirm this position?");
            if (!UserInterface.GetUserConfirm()) { return; }
            currentPlayer.position = viableLocations[index].position;
            currentPlayer.moves -= 1; // update to accounts for travelling costs

            if (currentPlayer.playerName == "test") { currentPlayer.moves = 3; } // for testing purposes
        }
            
        public void DisplayPlayers()
        {
            foreach (Player p in gamePlayers)
            {
                TravelersOfCatan.UserInterface.CreatePopup(p.playerName);
            }
        }
    }

   

    class Board // A graph of nodes
    {

        private HexagonUnit[] board = new HexagonUnit[19];
        Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>(); 
        // array of nodes and hexagon centers in graph. Fixed length means no need to resize
        public Dictionary<Vector3, Dictionary<Vector3, Connection>> connections = new Dictionary<Vector3, Dictionary<Vector3, Connection>>(); 
        // nested dictionary for the connections between nodes in the board with a default state of new Connection() which can be updated as the game progresses
        // this acts as an adjacency matrix of the graph of nodes but omits all the null values

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
                            HexagonUnit unit = new HexagonUnit(Resource.GetRandom(), x, y, z);
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

                            // register a list of all existing connections for the AI to use



                            nodes.Add(new Vector3(x, y, z), n);
                            i++;
                        }
                    }
                }
            }

            foreach (Node n in nodes.Values)
            {
                n.RegisterConnections(this);
                foreach (Connection con in n.GetConnections())
                {
                    if (!connections.ContainsKey(con.start.position))
                    {
                        connections.Add(con.start.position, new Dictionary<Vector3, Connection>());
                    }
                    connections[con.start.position].Add(con.end.position, con);
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

        public Node GetNode(Vector3 pos)
        {
            
            if (nodes.ContainsKey(pos))
                return nodes[pos];
            else
                return null;
        }

        public Node[] GetAllNodes()
        {
            return nodes.Values.ToArray();
        }

        public void ShowBoard() 
        {

            TravelersOfCatan.UserInterface.CreatePopup("Hexes:");

            foreach (HexagonUnit unit in board)
            {
                TravelersOfCatan.UserInterface.CreatePopup(unit.ToString());
            }

            TravelersOfCatan.UserInterface.CreatePopup("Nodes:");

            foreach (Node u in nodes.Values)
            {
                TravelersOfCatan.UserInterface.CreatePopup(u.ToString());
            }
        }

        public void ShowBoardConnections()
        {
            foreach (var i in connections)
            {
                TravelersOfCatan.UserInterface.CreatePopup(i.Key.ToString());
                foreach (var j in i.Value)
                {
                    TravelersOfCatan.UserInterface.CreatePopup("\t" + j.Value.ToString());
                }
            }
        }
            

        public Connection GetConnection(Vector3 v1, Vector3 v2)
        {
            var x = connections[v1];
            if (x == null)
            {
                return null;
            }
            else
            {
                if (x.ContainsKey(v2))
                {
                    return x[v2];
                }
                else
                {
                    return null;
                }
            }
        }


        public void UpdateConnection(Vector3 v1, Vector3 v2, string status, Player currentPlayer) // weakest function in entire project
        {
            Connection x = connections[v1][v2];
            x.SetOccupant(currentPlayer);
            x.SetStatus(status);
        }

    }

    class Player
    {
        private int victoryPoints;
        private Dictionary<Resource, int> resources = new Dictionary<Resource, int>() {
            { new Resource(1), 0 },
            { new Resource(2), 0 },
            { new Resource(3), 0 },
            { new Resource(4), 0 },
            { new Resource(5), 0 }
        };

        /// <summary>
        private List<Node> buildings = new List<Node>(); //
        private List<Connection> connections = new List<Connection>(); //
        /// Unused for now however may be useful for future features
        /// </summary>

        protected int playerNumber; // useful as a UID for the player allowing the same name in testing
        public string playerName;

        public int moves;
        public Vector3 position;
        protected bool isAI = false; // gets changed by child AI class

        public Player(int playerNumber, string playerName, Vector3 origin)
        {
            this.playerNumber = playerNumber;
            this.playerName = playerName;
            victoryPoints = 0;
            position = origin;
        }

        private void addVictoryPoints(int points) // required for future features
        {
            victoryPoints += points;
        }


        public void addBuilding(Node building)
        {
            buildings.Add(building);
            addVictoryPoints(TravelersOfCatan.victoryPointConvertor[building.status.GetStatus()]);

        }

        public Node GetCapital()
        {
            return buildings[0];
        }

        public void addConnection(Connection connection)
        {
            connections.Add(connection);
            addVictoryPoints(TravelersOfCatan.victoryPointConvertor[connection.GetStatus()]);
        }

        public int getVictoryPoints()
        {
            return victoryPoints;
        }

        public void addResource(Resource resource, int amount = 1)
        {
            resources[resource] += amount;
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

        public void Trade()
        {

        }

    }


    class Node
    {
        public Vector3 position;
        public Building status = new Building();
        public Dictionary<Vector3, Connection> connections = new Dictionary<Vector3, Connection>();

        public Node(int x, int y, int z)
        {
            position = new Vector3(x, y, z);

            
        }


        public void RegisterConnections(Board gameBoard)
        {
            foreach(Vector3 v in GetNodeNeighbours())
            {
                if (gameBoard.GetNode(v) != null)
                {
                    Console.WriteLine("Adding connection" + this + v);
                    connections.Add(v, new Connection(this, gameBoard.GetNode(v)));
                }
            }
        }

        public List<Connection> GetConnections()
        {
            return Enumerable.ToList<Connection>(connections.Values);
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
            if (!status.IsEmpty())
            {
                return $"{status} at {position}";

            }
            else
            {
                return position.ToString();
            }
        }


    }
    

    class Connection
    {
        public static readonly string[] statuses = { "Empty", "Road", "Wall" };
        public Node start;
        public Node end;
        private int i = 0;
        private Player occupant;


        public Connection(Node Start, Node End, int i = 0, string status = "", Player occupant = null)
        {
            this.i = i;
            if (status != "")
            {
                this.i = Array.IndexOf(statuses, status);
            }
            this.occupant = occupant;
            this.start = Start;
            this.end = End;
        }

        public static bool operator ==(Connection c1, Connection c2)
        {
            if (c1 is null)
            {
                if (c2 is null)
                {
                    return true;
                }
                return false;
            }
            return c1.Equals(c2);
        }

        public static bool operator !=(Connection c1, Connection c2)
        {
            if (c1 is null)
            {
                if (c2 is null)
                {
                    return false;
                }
                return true;
            }
            return c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Connection))
            {
                return false;
            }
            Connection otherConnection = obj as Connection;
            if (otherConnection.start == start && otherConnection.end == end) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public Player GetOccupant()
        {
            return occupant;
        }

        public void SetOccupant(Player p)
        {
            occupant = p;
        }

        public string GetStatus()
        {
            return statuses[i];
        }

        public void SetStatus(string status)
        {
            i = Array.IndexOf(statuses, status);
        }

        public int GetWalkingCost(Player otherPlayer)
        {
            if (this.i == 0)
            {
                return 1;
            }
            if (otherPlayer == occupant)
            {
                if (this.i == 1)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else if (otherPlayer != occupant)
            {
                return int.MaxValue;
            }
            return int.MinValue; // should never happen
        }


        public override string ToString()
        {
            return $"{statuses[i]} Owned by {occupant} Which connects {start} to {end}";
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

    class Building
    {
        public int i;
        private string[] statuses = { "Empty", "Village", "City", "Highway Man" };
        private Player occupant;



        public Building(string i = "Empty", Player o = null)
        {
            this.i = Array.IndexOf(statuses, i);
            occupant = o;

        }

        public void UpgradeVillage()
        {
            if (i == 1)
            {
                i++;
            }
            else
            {
                TravelersOfCatan.UserInterface.CreatePopup(@"You can't upgrade a {this.ToString()}");
            }
        }

        public bool IsEmpty()
        {
            return i == 0;
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

        public string GetStatus()
        {
            return statuses[i];
        }

        public Player GetOccupant()
        {
            return occupant;
        }

    }

    class Resource
    {
        public static readonly string[] resources = { "Empty", "Wood", "Brick", "Wheat", "Sheep", "Ore" };
        private static readonly Random rng = new Random();
        private int i;


        public Resource(int i = 0)
        {
            this.i = i;
        }

        public override string ToString()
        {
            return resources[i];
        }


        public void CreateRandomResource()
        {
            i = rng.Next(1, resources.Length);
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
        
        public static Resource GetRandom()
        {
            Resource i = new Resource();
            i.CreateRandomResource();
            return i;
        }


    }

}
