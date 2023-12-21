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
                {"Road", new Dictionary<Resource, int>()     { { new Resource(1), 1 }, { new Resource(2), 1 }, { new Resource(3), 0 }, { new Resource(4), 1 }, { new Resource(5), 0 } } },
                {"Wall", new Dictionary<Resource, int>()     { { new Resource(1), 0 }, { new Resource(2), 5 }, { new Resource(3), 0 }, { new Resource(4), 0 }, { new Resource(5), 0 } } },
                {"Village", new Dictionary<Resource, int>()  { { new Resource(1), 3 }, { new Resource(2), 1 }, { new Resource(3), 3 }, { new Resource(4), 3 }, { new Resource(5), 2 } } },
                {"City", new Dictionary<Resource, int>()     { { new Resource(1), 1 }, { new Resource(2), 3 }, { new Resource(3), 1 }, { new Resource(4), 0 }, { new Resource(5), 4 } } }
            };

        private static readonly Vector3[] StartingCoords = new Vector3[] {
            new Vector3( 0,  3, -2),
            new Vector3( 1, -2,  3),
            new Vector3( 3, -1, -1),
            new Vector3(-2,  2,  2) 
        };

        public UI UserInterface;

        public readonly int WinningVictoryPoints; // allow this to be passed into the constructor
        public readonly int StartingResourceCount;
        public readonly float TimePerMove;

        private int turn = 0;
        //private int MAXplayer = 0;
        private Player currentPlayer;
        public List<Player> gamePlayers = new List<Player>();
        public Board board;

        public TravelersOfCatan(UI ui, int win, int start, float maxtime)
        {

            UserInterface = ui;
            WinningVictoryPoints = win;
            StartingResourceCount = start;
            TimePerMove = maxtime;

        }


        public TravelersOfCatan(UI ui, GameWrapper game)
        {
            UserInterface = ui;
            WinningVictoryPoints = game.winVictoryPoints;
            TimePerMove = game.timePerMove;
            foreach (PlayerWrapper pl in game.allPlayers)
            {
                if (pl.isAI)
                    gamePlayers.Add(new AI(pl));
                else
                    gamePlayers.Add(new Player(pl));
            }

            board = Board.SoftDeSerialize(game.board);
            turn = game.turn;
            UserInterface.DisplayBoard(board);
            UserInterface.DisplayPlayers(gamePlayers);


        }


        public void AddPlayer(string Name)
        {
            int i = gamePlayers.Count;
            gamePlayers.Add(new Player(i + 1, Name, StartingCoords[i]));
        }

        public void AddAI()
        {
            int i = gamePlayers.Count;
            gamePlayers.Add(new AI(i + 1, "AI" + (i + 1), StartingCoords[i]));


        }

        public static Dictionary<Resource, int> GetCostOfUpgrade(string entityName)
        {

            return purchaseCost[entityName];

        }


        public void startGame()
        {
            

            UserInterface.CreatePopup(gamePlayers.Count.ToString() + " players have joined the game");
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


            UserInterface.DisplayBoard(board);
            UserInterface.DisplayPlayers(gamePlayers);
            StartTurn();
            
        }

        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public void StartTurn(float timeleft=-1f)
        {
            if (timeleft == -1f)
            {
                timeleft = TimePerMove;
            }
            currentPlayer = gamePlayers[turn];
            currentPlayer.moves = 3; // Allow this to change as a gamemode
            gatherResources();

            if (currentPlayer.GetType() == typeof(Player))
            {
                UserInterface.BeginTurn(timeleft);
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
            if (HasWinner()) return;

            turn++;
            if (turn >= gamePlayers.Count)
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

        public bool HasWinner()
        {
            return gamePlayers.Any(p => p.getVictoryPoints() >= WinningVictoryPoints);
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

            foreach (Node n in currentPlayer.GetBuildings())
            {
                if (n.status.GetStatus() == "City")
                {
                    foreach (Vector3 u in n.GetHexNeighbours())
                    {
                        if (board.GetHexAtPosition(u) != null)
                        {
                            if (board.GetHexAtPosition(u).resource.ToString() == "Empty") continue;
                            currentPlayer.addResource(board.GetHexAtPosition(u).resource);
                        }
                    }
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
        public Dictionary<Resource, int> GetDifference(string structure) // can be used in the UI to show the player what they need to buy
        {
            Dictionary<Resource, int> res = new Dictionary<Resource, int>();
            Dictionary<Resource, int> diff = GetCostOfUpgrade(structure);
            foreach (var entry in diff)
            {
                res.Add(entry.Key, Math.Min(entry.Value, currentPlayer.getResources()[entry.Key] - entry.Value));
            }

            return res;
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
            return canAfford;
        }
        public void tryPurchaseRoad()
        {

            List<Node> viableLocations = new List<Node>();
            Node otherPos;
            bool canconnect = board.GetNode(currentPlayer.position).status.GetOccupant() == currentPlayer.getNumber(); // player must be standing on their own settlement to build a road
            
            foreach (Vector3 vOther in board.GetNode(currentPlayer.position).GetNodeNeighbours() )
            {
                if (board.GetNode(vOther) == null) continue; // this is a null node (out of bounds
                
                Connection con = board.GetConnection(currentPlayer.position, vOther);
                otherPos = board.GetNode(vOther);
                if (con.GetStatus() != "Empty")
                {
                    continue; // can not build a road on existing connections of any sort
                }
                else if ((otherPos.status.GetOccupant() != currentPlayer.getNumber()) && (otherPos.status.GetStatus() != "Empty"))
                {
                    continue; // The enemy controls the settlement at the end of this road
                }
                else if (!canconnect && (otherPos.status.GetOccupant() != currentPlayer.getNumber()))
                {
                    continue; // player must be connecting to their own settlement
                }
                else
                {
                    viableLocations.Add(otherPos);
                }

            }

            UserInterface.GetUserNodeChoice(viableLocations.ToArray(), purchaseRoad);
        
        
        }
        public void tryPurchaseWall()
        {


            List<Node> viableLocations = new List<Node>();
            Node otherPos;
            bool canconnect = board.GetNode(currentPlayer.position).status.GetOccupant() == currentPlayer.getNumber(); // player must be standing on their own settlement to build a road

            foreach (Vector3 vOther in board.GetNode(currentPlayer.position).GetNodeNeighbours())
            {
                if (board.GetNode(vOther) == null) continue; // this is a null node (out of bounds

                Connection con = board.GetConnection(currentPlayer.position, vOther);
                otherPos = board.GetNode(vOther);
                if (con.GetStatus() != "Empty")
                {
                    continue; // can not build a road on existing connections of any sort
                }
                else
                {
                    viableLocations.Add(otherPos);
                }

            }

            UserInterface.GetUserNodeChoice(viableLocations.ToArray(), purchaseWall);


        }
        public void tryPurchaseVillage()
        {

            Node otherPos;
            Node current = board.GetNode(currentPlayer.position);
            if (current.status.GetStatus() != "Empty")
            {
                UserInterface.CreatePopup("You can not build a settlement on an existing settlement");
                return;
            }


            bool DistanceRule = true;
            bool isConnecting = false;
            
            foreach (Vector3 vOther in board.GetNode(currentPlayer.position).GetNodeNeighbours())
            {
                if (board.GetNode(vOther) == null) continue; // this is a null node (out of bounds

                Connection con = board.GetConnection(currentPlayer.position, vOther);
                otherPos = board.GetNode(vOther);

                if (con.GetOccupant() != currentPlayer.GetID() && con.GetStatus() == "Road")
                {
                    // can not build a settlement if an enemy road connects to this node
                    
                    isConnecting = false;
                    DistanceRule = false;
                    break;
                }
                else if (con.GetOccupant() == currentPlayer.GetID() && con.GetStatus() == "Road") 
                {
                    // node is connected to a road owned by the player
                    isConnecting = true;
                }

                if (otherPos.status.GetStatus() != "Empty")
                {
                    // breaches distance rule but may still be valid
                    DistanceRule = false;
                }


            }
            UserInterface.CreatePopup(DistanceRule.ToString());
            UserInterface.CreatePopup(isConnecting.ToString());

            if (isConnecting || DistanceRule)
            {
                purchaseVillage(current);

            }


        }
        public void tryPurchaseCity()
        {

            Node current = board.GetNode(currentPlayer.position);
            if (current.status.GetStatus() == "Village" && current.status.GetOccupant() == currentPlayer.GetID())
            {
                //purchaseCity(current);
                UserInterface.GetUserNodeChoice(new Node[] { current }, purchaseCity);

            }
            else 
            { 
                UserInterface.CreatePopup("Player must be on their village to upgrade");
            }

        }
        public void purchaseRoad(Node other)
        {
            board.UpdateConnection(currentPlayer.position, other.position, "Road", currentPlayer);
            ChargePlayer("Road");
            currentPlayer.addConnection(board.GetConnection(currentPlayer.position, other.position));
            UserInterface.UpdateConnection(other, board.GetNode(currentPlayer.position));
        }
        public void purchaseWall(Node other)
        {
            board.UpdateConnection(currentPlayer.position, other.position, "Wall", currentPlayer);
            ChargePlayer("Wall");
            currentPlayer.addConnection(board.GetConnection(currentPlayer.position, other.position));
            UserInterface.UpdateConnection(other, board.GetNode(currentPlayer.position));

        }
        public void purchaseVillage(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status = new Building("Village", currentPlayer.getNumber());
            ChargePlayer("Village");
            currentPlayer.addBuilding(board.GetNode(currentPlayer.position));
            UserInterface.UpdateSettlement(otherPos);
        }
        public void purchaseCity(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status.UpgradeVillage();
            ChargePlayer("City");
            currentPlayer.upgradeCillage(board.GetNode(currentPlayer.position));
            UserInterface.UpdateSettlement(otherPos);

        }

        /// <summary>
        /// Player Movement
        /// </summary>

        public void attemptPlayerMove()
        {
            
            Connection con;
            Node target;
            List<Node> viableLocations = new List<Node>();
            bool possible = false;
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
                possible = possible || valid;
                // Check that the player has enough movement points to move to this location
                if (currentPlayer.moves < con.GetWalkingCost(currentPlayer))
                {
                    valid = false;
                }

                if (valid)
                    viableLocations.Add(target);
                    
            }

            if (viableLocations.Count == 0 && !possible)
            {
                UserInterface.CreatePopup("Something went wrong... Sending you to your capital");
                currentPlayer.position = currentPlayer.GetCapital();
                return;
            }
            UserInterface.GetUserNodeChoice(viableLocations.ToArray(), MovePlayer);

        }

        private void MovePlayer(Node otherpos)
        {
            currentPlayer.moves -= board.GetConnection(currentPlayer.position, otherpos.position).GetWalkingCost(currentPlayer); 
            currentPlayer.position = otherpos.position;

            if (currentPlayer.playerName == "test") { currentPlayer.moves = 3; } // for testing purposes

            UserInterface.UpdatePlayer(otherpos);


        }

    }


    /// <summary>
    /// Minor classes for the game that are not worth their own file and are all serializable
    /// </summary>

  

    public class HexagonUnit
    {
        public Vector3 position;
        public Resource resource;

        public HexagonUnit(Resource R, int x, int y, int z)
        {
            resource = R;
            position = new Vector3(x, y, z);
        }

        public HexagonUnit(HexagonUnitWrapper hex)
        {
            resource = new Resource(hex.resource);
            position = new Vector3(hex.position.x, hex.position.y, hex.position.z);
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
    }

    public class Building : Settlement
    { 

        public Building(string i = "Empty", int o = -1)
        {
            statuses = new string[] { "Empty", "Village", "City" };
            id = Array.IndexOf(statuses, i);
            occupantID = o;

        }

        public Building(SettlementWrapper sw)
        {
            statuses = new string[] { "Empty", "Village", "City" };
            occupantID = sw.occupantID;
            id = Array.IndexOf(statuses, sw.status);
        }


        public void UpgradeVillage()
        {
            if (id == 1)
            {
                id++;
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

    public class Resource
    {
        public static readonly string[] resources = { "Empty", "Brick", "Sheep", "Ore", "Wood", "Wheat" };
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

        public Resource(ResourceWrapper res)
        {
            this.id = res.id;
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
