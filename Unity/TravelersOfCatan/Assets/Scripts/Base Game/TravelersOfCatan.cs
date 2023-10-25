using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App;
using System.Collections;


// Path: Class1.cs
namespace NEAGame
{
    [System.Serializable]
    /// <summary>
    /// Class <c>NEAGame.TravelersOfCatan</c> that controls the game and all of its mechanics. This is the main class that should be called from the UI.
    /// </summary>
    public class TravelersOfCatan
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


        public TravelersOfCatan(UI ui, int MAXplayer, int MAXbot)
        {

            UserInterface = ui;

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

            // ADD HIGHWAYMAN MECHANICS

            TravelersOfCatan.UserInterface.DisplayPlayers(gamePlayers);
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

            TravelersOfCatan.UserInterface.UpdateBoard(board);


            while (gameOngoing)
            {
                currentPlayer = gamePlayers[turn];
                currentPlayer.moves = 3; // Allow this to change as a gamemode
                gatherResources();
                if (TravelersOfCatan.UserInterface.GetType() == typeof(TerminalUI))
                {
                    //takeTurn(); // this must now be event based with a timer
                }
                else if (TravelersOfCatan.UserInterface.GetType() == typeof(UnityUI))
                {
                    TravelersOfCatan.UserInterface.CreatePopup("It is " + currentPlayer.playerName + "'s turn");
                    return;
                }

            }
        }

        public void CheckWinner()
        {
            foreach (Player p in gamePlayers)
            {
                if (p.getVictoryPoints() >= WinningVictoryPoints)
                {

                    TravelersOfCatan.UserInterface.CreatePopup("Player " + p.playerName + " has won the game");
                    TravelersOfCatan.UserInterface.HandleWinner(p);
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

        public void makePurchase(string structure)
        {

            
            Dictionary<int, int> cost = GetCostOfUpgrade(structure);
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

            switch (structure)
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
                UserInterface.CreatePopup("Purchase Successful");
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
                    TravelersOfCatan.UserInterface.CreatePopup("Something went wrong..."); // this should never happen
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
                
                otherpos = UserInterface.GetUserNodeChoice(viableLocations.ToArray());
                
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
                    TravelersOfCatan.UserInterface.CreatePopup("Something went wrong..."); // this should never happen
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

                otherpos = UserInterface.GetUserNodeChoice(viableLocations.ToArray());

            }
            else if (viableLocations.Count == 1)
            {
                otherpos = viableLocations[0];
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
            Node otherpos = UserInterface.GetUserNodeChoice(viableLocations.ToArray());

            if (!UserInterface.GetUserConfirm()) { return; }
            currentPlayer.position = otherpos.position;
            currentPlayer.moves -= 1; // update to accounts for travelling costs

            if (currentPlayer.playerName == "test") { currentPlayer.moves = 3; } // for testing purposes
        }
            
    }

    /// <summary>
    /// Minor classes for the game that are not worth their own file and are all serializable
    /// </summary>

  

    [System.Serializable]
    public class HexagonUnit
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

    [System.Serializable]
    public class Building
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

    [System.Serializable]
    public class Resource
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
