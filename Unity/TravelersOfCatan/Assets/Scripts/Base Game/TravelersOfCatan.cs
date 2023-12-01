using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;



namespace NEAGame
{
    [System.Serializable]
    /// <summary>
    /// Class <c>NEAGame.TravelersOfCatan</c> that controls the game and all of its mechanics. 
    /// This is the main class that should be called from the UI.
    /// </summary>
    public class TravelersOfCatan
    {

        public static readonly IDictionary<string, int> victoryPointConvertor = new Dictionary<string, int>()
        {
            {"Road", 2},
            {"Wall", 1},
            {"Village", 4 },
            {"City", 10 }
        };

        private static readonly Dictionary<string, Dictionary<Resource, int>> purchaseCost = new Dictionary<string, Dictionary<Resource, int>>()
            {
                {"Road", new Dictionary<Resource, int>()     { { new Resource("Wood"), 1 }, { new Resource(2), 1 }, { new Resource(3), 0 }, { new Resource(4), 1 }, { new Resource(5), 0 } } },
                {"Wall", new Dictionary<Resource, int>()     { { new Resource("Wood"), 0 }, { new Resource(2), 5 }, { new Resource(3), 0 }, { new Resource(4), 0 }, { new Resource(5), 0 } } },
                {"Village", new Dictionary<Resource, int>()  { { new Resource("Wood"), 3 }, { new Resource(2), 1 }, { new Resource(3), 3 }, { new Resource(4), 3 }, { new Resource(5), 2 } } },
                {"City", new Dictionary<Resource, int>()     { { new Resource("Wood"), 1 }, { new Resource(2), 3 }, { new Resource(3), 1 }, { new Resource(4), 0 }, { new Resource(5), 4 } } }
            };

        private static readonly Vector3[] StartingCoords = new Vector3[] {
            new Vector3( 0,  3, -2),
            new Vector3( 1, -2,  3),
            new Vector3( 3, -1, -1),
            new Vector3(-2,  2,  2) 
        };

        public static UI UserInterface;




        private int WinningVictoryPoints = 30; // allow this to be passed into the constructor
        private int StartingResourceCount = 10;

        private int turn = 0;
        private int MAXplayer = 0;
        private Player currentPlayer;
        public List<int> victoryPoints = new List<int>();
        public List<Player> gamePlayers = new List<Player>();
        public Board board;


        public TravelersOfCatan(UI ui)
        {

            UserInterface = ui;

        }


        public void AddPlayer(string Name)
        {
            MAXplayer++;
            victoryPoints.Add(0);
            int i = gamePlayers.Count;
            gamePlayers.Add(new Player(i + 1, Name, StartingCoords[i]));
        }

        public void AddAI()
        {
            MAXplayer++;
            victoryPoints.Add(0);
            int i = gamePlayers.Count;
            gamePlayers.Add(new AI(i + 1 + MAXplayer, "AI" + (i + 1), StartingCoords[i + MAXplayer]));


        }

        public static Dictionary<Resource, int> GetCostOfUpgrade(string entityName)
        {

            return purchaseCost[entityName];

        }


        public void startGame()
        {

            board = new Board();

            foreach (Player current in gamePlayers)
            {

                Building capital = new Building("City", current.getNumber());
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


            UserInterface.UpdateBoard(board);

            StartTurn();
            
        }

        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }

        private void StartTurn()
        {
            currentPlayer = gamePlayers[turn];
            currentPlayer.moves = 3; // Allow this to change as a gamemode
            gatherResources();

            if (currentPlayer.GetType() == typeof(Player))
            {
                UserInterface.BeginTurn();
            }
            else if (currentPlayer.GetType() == typeof(AI))
            {
                //UserInterface.;
                //((AI)currentPlayer).TakeTurn();
                // AI will take its turn here and the UI must be updated with their moves.
            }

        }
        public void EndTurn()
        {
            turn++;
            if (turn >= MAXplayer)
            {
                turn = 0;
            }
            StartTurn();
        }

        public void CheckWinner()
        {
            foreach (Player p in gamePlayers)
            {
                if (p.getVictoryPoints() >= WinningVictoryPoints)
                {

                    UserInterface.CreatePopup("Player " + p.playerName + " has won the game");
                    UserInterface.HandleWinner(p);
                    return;
                }
            }
        }

        
        /*public void printCostOfItem(string item)
        {
            Dictionary<int, int> cost = GetCostOfUpgrade(item);
            foreach (var entry in cost)
            {
                if (entry.Value > 0)
                {
                    UserInterface.CreatePopup($"You need {entry.Value} {Resource.resources[entry.Key]}");
                }
            }
        }*/

        public void gatherResources()
        {
            foreach (Vector3 u in board.GetNode(currentPlayer.position).GetHexNeighbours())
            {
                if (board.GetHexAtPosition(u) != null)
                {
                    if (board.GetHexAtPosition(u).resource.ToString() == "Empty") continue;
                    currentPlayer.addResource(board.GetHexAtPosition(u).resource);
                }
            }

        }


        /// <summary>
        /// Shopping Methods
        /// </summary>


        public void ChargePlayer(string structure)
        {

            Dictionary<Resource, int> cost = GetCostOfUpgrade(structure);
            foreach (var entry in cost)
            {
                currentPlayer.removeResource(entry.Key, entry.Value);
            }
            UserInterface.CreatePopup("Purchase Successful");
            CheckWinner();

        }

        public bool CheckCosts(string structure)
        {
            Dictionary<Resource, int> cost = GetCostOfUpgrade(structure);
            bool canAfford = true;
            foreach (var entry in cost)
            {
                if (currentPlayer.getResources()[entry.Key] < entry.Value)
                {
                    canAfford = false;
                }
            }
            if (!canAfford)
            {
                UserInterface.CreatePopup("You cannot afford this purchase");
            }
            return canAfford;
        }

        public Node[] canPurchaseRoad()
        {

            if (!CheckCosts("Road")) return new Node[0];


            List<Node> viableLocations = new List<Node>();
            Node otherPos;
            bool canconnect = board.GetNode(currentPlayer.position).status.GetOccupant() == currentPlayer.getNumber(); // player must be standing on their own settlement to build a road
            if (!canconnect)
            {
                UserInterface.CreatePopup("You must be standing on your own settlement to build a road");
                return new Node[0];
            }
            foreach (Vector3 vOther in board.GetNode(currentPlayer.position).GetNodeNeighbours() )
            {
                if (board.GetNode(vOther) == null) continue; // this is a null node (out of bounds
                
                Connection con = board.GetConnection(currentPlayer.position, vOther);
                otherPos = board.GetNode(vOther);
                if (con.GetOccupant() > 0)
                {
                    continue; // can not build a road on existing connections of any sort
                }
                else if ((otherPos.status.GetOccupant() != currentPlayer.getNumber()) && (otherPos.status.GetOccupant() > 0))
                {
                    continue; // The enemy controls the settlement at the end of this road
                }
                else
                {
                    viableLocations.Add(otherPos);
                }

            }

            return viableLocations.ToArray();           

        }

        public void tryPurchaseRoad()
        {
            UserInterface.GetUserNodeChoice(canPurchaseRoad(), purchaseRoad);
        } // these functions will be needed as a canPurchase func is mandatory for the UI

        public bool canPurchaseVillage()
        {

            if (!CheckCosts("Village")) return false;


            if (!board.GetNode(currentPlayer.position).isEmpty())
            {
                UserInterface.CreatePopup("You cannot place a village here as there is already an establishment on this Node");
                return false;
            }

            bool isConnected = false;
            foreach (Vector3 nPos in board.GetNode(currentPlayer.position).GetNodeNeighbours())
            {
                Connection con = board.GetConnection(currentPlayer.position, nPos);
                if (con.GetOccupant() == currentPlayer.getNumber() && con.GetStatus() == "Road")
                {
                    isConnected = true;
                }

            }
            if (!isConnected)
            {

                UserInterface.CreatePopup("You cannot place a village here as there is no road connecting to this Node");
                return false;
            }

            return true;

        }


        public void purchaseRoad(Node other)
        {
            board.UpdateConnection(currentPlayer.position, other.position, "Road", currentPlayer);
            ChargePlayer("Road");
            currentPlayer.addConnection(board.GetConnection(currentPlayer.position, other.position));
        }

        public void purchaseWall(Node other)
        {
            board.UpdateConnection(currentPlayer.position, other.position, "Wall", currentPlayer);
            currentPlayer.addConnection(board.GetConnection(currentPlayer.position, other.position));

        }

        public void purchaseVillage(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status = new Building("Village", currentPlayer.getNumber());
            currentPlayer.addBuilding(board.GetNode(currentPlayer.position));
        }

        public void purchaseCity(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status = new Building("City", currentPlayer.getNumber());
            currentPlayer.upgradeCillage(board.GetNode(currentPlayer.position));
        }

        /// <summary>
        /// Player Movement
        /// </summary>

        public void attemptPlayerMove()
        {
            Connection con;
            Node target;
            List<Node> viableLocations = new List<Node>();

            bool valid;
            foreach (Vector3 pos in board.GetNode(currentPlayer.position).GetNodeNeighbours())
            {
                valid = true;
                con = board.GetConnection(pos, currentPlayer.position);
                if (con == null) continue;
                target = board.GetNode(pos);
                if ((con.GetOccupant() != currentPlayer.getNumber()) && (con.GetOccupant() > 0)) 
                {
                    valid = false; // the enemy controls this road or wall
                }
                else if ((target.status.GetOccupant() != currentPlayer.getNumber()) && (target.status.GetOccupant() > 0))
                {
                    valid = false; // The enemy controls the settlement at the end of this road
                }
                // Check that none of the other plauers are at this location
                foreach (Player p in gamePlayers)
                {
                    if (p.position == target.position)
                    {
                        valid = false ;
                    }
                }

                if (valid)
                    viableLocations.Add(target);
                    
            }

            if (viableLocations.Count == 0)
            {
                UserInterface.CreatePopup("Something went wrong... Sending you to your capital");
                currentPlayer.position = currentPlayer.GetCapital().position;
                return;
            }
            UserInterface.GetUserNodeChoice(viableLocations.ToArray(), MovePlayer);

        }

        private void MovePlayer(Node otherpos)
        {
            currentPlayer.position = otherpos.position;
            currentPlayer.moves -= 1; // update to accounts for travelling costs

            if (currentPlayer.playerName == "test") { currentPlayer.moves = 3; } // for testing purposes

            UserInterface.UpdatePlayer(otherpos);


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

    public abstract class Settlement
    {
        protected int id { get; set; }
        protected string[] statuses { get; set; }
        protected int occupantID { get; set; }
        public static int convertToVictoryPoints(string status) { return 0;  }
    }

    [System.Serializable]
    public class Building : Settlement
    { 

        public Building(string i = "Empty", int o = -1)
        {
            statuses = new string[] { "Empty", "Village", "City" };
            id = Array.IndexOf(statuses, i);
            occupantID = o;

        }

        public void UpgradeVillage()
        {
            if (id == 1)
            {
                id++;
            }
            else
            {
                TravelersOfCatan.UserInterface.CreatePopup(@"You can't upgrade a {this.ToString()}");
            }
        }

        public bool IsEmpty()
        {
            return id == 0;
        }
        public override string ToString()
        {
            if (occupantID != -1)
            {
                return $"{statuses[id]} owned by Player {occupantID}";
            }
            else
            {
                return $"{statuses[id]}";
            }
        }

        public string GetStatus()
        {
            return statuses[id];
        }

        public int GetOccupant()
        {
            return occupantID;
        }

    }

    [System.Serializable]
    public class Resource
    {
        public static readonly string[] resources = { "Empty", "Wood", "Brick", "Wheat", "Sheep", "Ore" };
        private int id;
        private static readonly Random rng = new Random();

        

        public Resource(int i = 0)
        {
            id = i;
        }

        public Resource(string name)
        {
            id = Array.IndexOf(resources, name);
        }

        public override string ToString()
        {
            return resources[id];
        }


        public void CreateRandomResource()
        {
            id = rng.Next(1, resources.Length);
        }

        public override bool Equals(System.Object otherItem)
        {
            if (otherItem == null)
            {
                return false;
            }

            Resource otherResource = otherItem as Resource;

            return (id == otherResource.id);
        }

        public override int GetHashCode()
        {
            return id;
        }
        
        public static Resource GetRandom()
        {
            Resource temp = new Resource();
            temp.CreateRandomResource();
            return temp;
        }


    }

}
