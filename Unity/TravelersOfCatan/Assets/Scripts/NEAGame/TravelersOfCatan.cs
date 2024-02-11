using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace NEAGame
{
    /// <summary>
    /// <c>TravelersOfCatan</c> is the main class for controlling the game. This includes creating the game board, players, and controlling the game loop.
    /// </summary>
    public class TravelersOfCatan
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// The following variables are readonly meaning they can not be changed after first assigned. These control the game rules of Catan. 
        /// These also take the form of dictionaries and nested dictionaries.
        /// Coding Style: Use of Constants
        /// Skill A: Complex Data Structure 
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static readonly IDictionary<string, int> VICTORY_POINT_CONVERTOR = new Dictionary<string, int>()
        {
            {"Road", 0},
            {"Wall", 1},
            {"Village", 3 },
            {"City", 5 }
        };

        private static readonly int MAX_MOVES = 3;

        private static readonly Dictionary<string, Dictionary<Resource, int>> PURCHASE_COST = new Dictionary<string, Dictionary<Resource, int>>()
            {
                {"Road", new Dictionary<Resource, int>()     { { new Resource(1), 1 }, { new Resource(2), 1 }, { new Resource(3), 0 }, { new Resource(4), 1 }, { new Resource(5), 0 } } },
                {"Wall", new Dictionary<Resource, int>()     { { new Resource(1), 2 }, { new Resource(2), 0 }, { new Resource(3), 0 }, { new Resource(4), 2 }, { new Resource(5), 0 } } },
                {"Village", new Dictionary<Resource, int>()  { { new Resource(1), 1 }, { new Resource(2), 2 }, { new Resource(3), 0 }, { new Resource(4), 1 }, { new Resource(5), 1 } } },
                {"City", new Dictionary<Resource, int>()     { { new Resource(1), 1 }, { new Resource(2), 0 }, { new Resource(3), 3 }, { new Resource(4), 1 }, { new Resource(5), 2 } } }
            };

        private static readonly Vector3[] STARTING_COORDS = new Vector3[] {
            new Vector3( 0,  3, -2),
            new Vector3( 1, -2,  3),
            new Vector3( 3, -1, -1),
            new Vector3(-2,  2,  2) 
        };

        public readonly int WinningVictoryPoints;
        public readonly int StartingResourceCount;
        public readonly float TimePerMove;


        private UI UserInterface;
        private int turn = 0;
        private Player currentPlayer;
        public List<Player> gamePlayers = new List<Player>();
        public Board board;

        // Dijkstra's algorithm variables
        private Dictionary<Node, int> distance = new Dictionary<Node, int>();
        private Dictionary<Node, Node> previous = new Dictionary<Node, Node>();
        
        // AI best move search variables
        public bool isAICalculation;
        public Stack<GameAction> actions = new Stack<GameAction>();

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Lots of the variables created above are private and can only be accessed by the methods in this class.
        /// This limits the scope of the variables to only the methods that need them, which is a good example of encapsulation.
        /// Skill A: Complex User-Defined OOP - Encapsulation 
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Constructor for a new game
        /// </summary>
        public TravelersOfCatan(UI ui, int win, int start, float maxtime)
        {

            UserInterface = ui;
            WinningVictoryPoints = win;
            StartingResourceCount = start;
            TimePerMove = maxtime;

        }

        /// <summary>
        /// Constructor for loading a game from a save file
        /// </summary>
        public TravelersOfCatan(UI ui, GameWrapper game)
        {
            UserInterface = ui;
            // retrieve the game settings from the save
            WinningVictoryPoints = game.winVictoryPoints;
            TimePerMove = game.timePerMove;
            // add all of the players back to the game
            foreach (PlayerWrapper pl in game.allPlayers)
            {
                if (pl.isAI)
                    gamePlayers.Add(new AI(pl, this));
                else
                    gamePlayers.Add(new Player(pl));
            }

            board = new Board(game.board);
            foreach (NodeWrapper n in game.board.nodes._Values)
            {
                Node node = new Node(n);
                foreach (Player pdl in gamePlayers)
                {
                    if (node.status.GetOccupant() == pdl.GetID())
                    {
                        pdl.addBuilding(node);
                        break;
                    }
                }
            }
            // load all of the connections made in the game so far
            for (int i = 0; i < game.board.connections._Keys.Count; i++)
            {
                Vector3 pos1 = new Vector3(game.board.connections._Keys[i].x, game.board.connections._Keys[i].y, game.board.connections._Keys[i].z);
                for (int j = 0; j < game.board.connections._Values[i]._Keys.Count; j++)
                {
                    Vector3 pos2 = new Vector3(game.board.connections._Values[i]._Keys[j].x, game.board.connections._Values[i]._Keys[j].y, game.board.connections._Values[i]._Keys[j].z);
                    Connection con = new Connection(game.board.connections._Values[i]._Values[j]);
                    board.UpdateConnection(pos1, pos2, con);
                    UserInterface.UpdateConnection(board.GetNode(pos1), board.GetNode(pos2), con);

                    foreach (Player pdl in gamePlayers)
                    {
                        if (con.GetOccupant() == pdl.GetID())
                        {
                            pdl.addConnection(con);
                            break;
                        }
                    }
                }
            }

            turn = game.turn;
            currentPlayer = gamePlayers[turn];
            if (HasWinner())
            {
                FindWinner(); // If the game is already won, display the winner
            }
            else
            {
                UserInterface.BeginGame(game.timer);
                UserInterface.CreatePopup($"Resuming game from player {turn+1}'s turn");
            }
        }

        // function to add a player to the game
        public void AddPlayer(string Name, string color)
        {
            int i = gamePlayers.Count;
            gamePlayers.Add(new Player(playerNumber: i + 1,playerName: Name, color: color, origin: STARTING_COORDS[i]));
        }

        // function to add an AI to the game
        public void AddAI(string Name, string color)
        {
            int i = gamePlayers.Count;
            gamePlayers.Add(new AI(playerID: i + 1, name: Name, playerColor: color, home: STARTING_COORDS[i], this));
        }

        // AI function to update the current player during the AI calculation
        public void UpdateCurrentPlayer(int id)
        {
            foreach (Player pdl in gamePlayers)
            {
                if (pdl.GetID() == id)
                {
                    currentPlayer = pdl;
                }
            }
            currentPlayer.moves = MAX_MOVES;            
        }

        // get the cost of a structure
        public static Dictionary<Resource, int> GetCostOfUpgrade(string entityName)
        {
            return PURCHASE_COST[entityName];
        }

        // function to start a new created game
        public void startGame()
        {

            board = new Board();

            foreach (Player current in gamePlayers)
            {

                Building capital = new Building("City", current.GetID());
                board.GetNode(current.position).status = capital;
                current.addBuilding(board.GetNode(current.position));

                // give every player their starting resources

                for (int i = 1; i < Resource.RESOURCES.Length; i++)
                {
                    current.addResource(new Resource(i), StartingResourceCount);
                }

            }

            turn = 0;
            currentPlayer = gamePlayers[0];
            currentPlayer.moves = MAX_MOVES;
            UserInterface.BeginGame(-1f);
            UserInterface.CreatePopup(gamePlayers.Count.ToString() + " players have joined the game");

        }

        // function to get the current game time
        public float GetGameTime()
        {
              return UserInterface.GetTimer();
        }

        // function to get the current player
        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }

        // function to start the current player's turn + gather resources. also called to start a loaded game
        public void StartTurn(float timeleft=-1f)
        {
            if (timeleft == -1f)
            {
                gatherResources(currentPlayer.GetID());
                timeleft = TimePerMove;
            }
            // selection based on the class type of the current player
            if (currentPlayer.GetType() == typeof(Player)) 
            {
                isAICalculation = false;   
            }
            else if (currentPlayer.GetType() == typeof(AI))
            {
                // AI has the default time limit because when the new game is loaded, the AI calculation is restarted.
                timeleft = TimePerMove; 
                isAICalculation = true;
            }
            UserInterface.BeginTurn(timeleft);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Display's the AI's chosen moves after calculation
        /// <br/> Skill A: Stack Operations
        /// </summary>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DisplayAIMoves()
        {
            currentPlayer = gamePlayers[turn];
            Stack<GameAction> selectedMoves = ((AI)currentPlayer).selectedMoves;
            Stack<GameAction> firstUndo = ((AI)currentPlayer).currentMove;
            // reverse order of stack
            Stack<GameAction> temp = new Stack<GameAction>();
            while (selectedMoves.Count > 0)
            {
                temp.Push(selectedMoves.Pop());
            }

            while (firstUndo.Count > 0)
            {
                GameAction actToUndo = firstUndo.Pop();
                UpdateCurrentPlayer(actToUndo.playerID);
                UndoAction(actToUndo);
            }

            UpdateCurrentPlayer(gamePlayers[turn].GetID()); // sets turn back to current player!
            isAICalculation = false;
            while (temp.Count > 0)
            {
                DoAction(temp.Pop());
            }
        }
        
        // function to end the current player's turn and continue the game
        public void EndTurn()
        {
            actions.Clear();
            if (HasWinner()) return;
            turn++;
            if (turn >= gamePlayers.Count)
            {
                turn = 0;
            }
            currentPlayer = gamePlayers[turn];
            currentPlayer.moves = MAX_MOVES;
            StartTurn();
        }

        // function to end the game when there is a winner.
        protected void FindWinner()
        {
            foreach (Player p in gamePlayers)
            {
                if (p.getVictoryPoints() >= WinningVictoryPoints)
                {
                    UserInterface.HandleWinner(p);
                    return;
                }
            }
        }

        // Check if there is a winner
        public bool HasWinner()
        {
            return gamePlayers.Any(p => p.getVictoryPoints() >= WinningVictoryPoints);
        }

        // Generates resources around the player with the given ID
        public void gatherResources(int ID)
        {
            Player pdl = gamePlayers.Find(x => x.GetID() == ID);

            foreach (Vector3 u in board.GetNode(pdl.position).GetHexNeighbours())
            {
                if (board.GetHexAtPosition(u) != null)
                {
                    if (board.GetHexAtPosition(u).ToString() == "Empty") continue;
                    pdl.addResource(board.GetHexAtPosition(u));
                    if (!isAICalculation)
                        UserInterface.ShowResource(u, board.GetHexAtPosition(u), Vector3.Zero);
                }
            }
            // also get resources from all cities
            foreach (Node n in pdl.GetBuildings())
            {
                if (n.status.GetStatus() == "City")
                {
                    foreach (Vector3 u in n.GetHexNeighbours())
                    {
                        if (board.GetHexAtPosition(u) != null)
                        {
                            if (board.GetHexAtPosition(u).ToString() == "Empty") continue;
                            pdl.addResource(board.GetHexAtPosition(u));
                            if (!isAICalculation)
                                UserInterface.ShowResource(u, board.GetHexAtPosition(u), Vector3.Zero);
                        }
                    }
                }
            }   
        }
        
        // Undo the resources gathered by the player with the given ID
        public void undoGatherResources(int ID)
        {
            Player pdl = gamePlayers.Find(c => c.GetID() == ID);

            foreach (Vector3 u in board.GetNode(pdl.position).GetHexNeighbours())
            {
                if (board.GetHexAtPosition(u) != null)
                {
                    if (board.GetHexAtPosition(u).ToString() == "Empty") continue;
                    pdl.removeResource(board.GetHexAtPosition(u));
                }
            }

            foreach (Node n in pdl.GetBuildings())
            {
                if (n.status.GetStatus() == "City")
                {
                    foreach (Vector3 u in n.GetHexNeighbours())
                    {
                        if (board.GetHexAtPosition(u) != null)
                        {
                            if (board.GetHexAtPosition(u).ToString() == "Empty") continue;
                            pdl.removeResource(board.GetHexAtPosition(u));
                        }
                    }
                }
            }   
        }

        // complete the trade between the current player and the other player
        public void CompleteTrade(Player otherPlayer, Dictionary<Resource, int> CurrentPlayerChange)
        {
            foreach (var entry in CurrentPlayerChange)
            {
                int change = Math.Abs(entry.Value);
                if (entry.Value > 0)
                {
                    currentPlayer.addResource(entry.Key, change);
                    otherPlayer.removeResource(entry.Key, change);
                    for (int _ = 0; _ < change; _++)
                        UserInterface.ShowResource(otherPlayer.position, entry.Key, currentPlayer.position);
                }
                else if (entry.Value < 0)
                {
                    currentPlayer.removeResource(entry.Key, change);
                    otherPlayer.addResource(entry.Key, change);
                    for (int _ = 0; _ < change; _++)
                        UserInterface.ShowResource(currentPlayer.position, entry.Key, otherPlayer.position);
                }
            }
        }

        // method to deduct the cost of a settlement from the player's inventory
        public void ChargePlayer(string structure)
        {
            Dictionary<Resource, int> cost = GetCostOfUpgrade(structure);
            foreach (var entry in cost)
            {
                currentPlayer.removeResource(entry.Key, entry.Value);
            }
            if (!isAICalculation)
            {
                if (HasWinner())
                {
                    FindWinner();
                }
                else if (!currentPlayer.isPlayerAI())
                {
                    UserInterface.CreatePopup("Purchase Successful");
                }
            }
        }

        // method used in the UI to show the player what they need to buy a settlement
        public Dictionary<Resource, int> GetDifference(string structure) 
        {
            Dictionary<Resource, int> res = new Dictionary<Resource, int>();
            Dictionary<Resource, int> diff = GetCostOfUpgrade(structure);
            foreach (var entry in diff)
            {
                res.Add(entry.Key, Math.Min(entry.Value, currentPlayer.getResources()[entry.Key] - entry.Value));
            }

            return res;
        }

        // method to check if the player can afford a settlement
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

        // method to get possible locations for a player to build a road
        public List<Node> tryPurchaseRoad()
        {
            if (!CheckCosts("Road"))
            {
                return new List<Node>();
            }
            List<Node> viableLocations = new List<Node>();
            Node otherPos;
            bool canconnect = board.GetNode(currentPlayer.position).status.GetOccupant() == currentPlayer.GetID(); // player must be standing on their own settlement to build a road
            bool allSlotsTaken = true;
            foreach (Vector3 vOther in board.GetNode(currentPlayer.position).GetNodeNeighbours() )
            {
                if (board.GetNode(vOther) == null) continue; // this is a null node (out of bounds
                
                Connection con = board.GetConnection(currentPlayer.position, vOther);
                otherPos = board.GetNode(vOther);
                if (con.GetStatus() != "Empty")
                {
                    continue; // can not build a road on existing connections of any sort
                }
                allSlotsTaken = false;
                if ((otherPos.status.GetOccupant() != currentPlayer.GetID()) && (otherPos.status.GetStatus() != "Empty"))
                {
                    continue; // The enemy controls the settlement at the end of this road
                }
                else if (!canconnect && (otherPos.status.GetOccupant() != currentPlayer.GetID()))
                {
                    continue; // player must be connecting to their own settlement
                }
                else
                {
                    viableLocations.Add(otherPos);
                }
            }
            if (!isAICalculation)
            {
                if (viableLocations.Count == 0)
                {
                    if (allSlotsTaken)
                    {
                        UserInterface.CreatePopup("All possible connections have already been made from your current position.");
                    }
                    else
                    {
                        UserInterface.CreatePopup("One end of the road must be connected to your settlement. You can not purchase a road from your current position.");
                    }
                }
                else
                {
                    UserInterface.GetUserNodeChoice(viableLocations.ToArray(), purchaseRoad);
                }
            }
            return viableLocations;
        }

        // method to get possible locations for a player to build a wall
        public List<Node> tryPurchaseWall()
        {
            if (!CheckCosts("Wall"))
            {
                if (!isAICalculation)
                    UserInterface.CreatePopup("You can not afford this");
                return new List<Node>();
            }

            List<Node> viableLocations = new List<Node>();
            Node otherPos;
            // player must be standing on their own settlement to build a road
            bool canconnect = board.GetNode(currentPlayer.position).status.GetOccupant() == currentPlayer.GetID(); 
            foreach (Vector3 vOther in board.GetNode(currentPlayer.position).GetNodeNeighbours())
            {
                if (board.GetNode(vOther) == null) continue; // this is a null node (out of bounds)

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
            if (!isAICalculation)
            {
                if (viableLocations.Count == 0)
                {
                    UserInterface.CreatePopup("All possible connections have already been made from your current position.");
                }
                else
                {
                    UserInterface.GetUserNodeChoice(viableLocations.ToArray(), purchaseWall);
                }
            }
            return viableLocations;

        }

        // method to see if the player can purchase a village
        public Node tryPurchaseVillage()
        {
            if (!CheckCosts("Village"))
            {
                return null;
            }
            Node otherPos;
            Node current = board.GetNode(currentPlayer.position);
            if (current.status.GetStatus() != "Empty")
            {
                if (!isAICalculation)
                    UserInterface.CreatePopup("You can not build a settlement on an existing settlement");
                return null;
            }

            bool DistanceRule = true;
            bool isConnecting = false;
            bool road = false;
            foreach (Vector3 vOther in board.GetNode(currentPlayer.position).GetNodeNeighbours())
            {
                if (board.GetNode(vOther) == null) continue; // this is a null node (out of bounds)

                Connection con = board.GetConnection(currentPlayer.position, vOther);
                otherPos = board.GetNode(vOther);

                if (con.GetOccupant() != currentPlayer.GetID() && con.GetStatus() == "Road")
                {
                    // can not build a settlement if an enemy road connects to this node
                    road = true;
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
            if (isConnecting || DistanceRule)
            {
                if (!isAICalculation)
                    UserInterface.GetUserNodeChoice(new Node[] { current }, purchaseVillage);
                return current;
            }
            else
            {
                if (!isAICalculation)
                {
                    if (road)
                        UserInterface.CreatePopup("You may not build a settlement on the end of an enemy road.");
                    else
                        UserInterface.CreatePopup("You may not build a settlement next to another settlement unless a road connects them.");
                }
                return null;
            }
        }

        // method to see if the player can purchase a city
        public Node tryPurchaseCity()
        {
            if (!CheckCosts("City"))
            {
                return null;
            }
            Node current = board.GetNode(currentPlayer.position);
            if (current.status.GetStatus() == "Village" && current.status.GetOccupant() == currentPlayer.GetID())
            {
                if (!isAICalculation)
                    UserInterface.GetUserNodeChoice(new Node[] { current }, purchaseCity);
                return current;
            }
            else 
            { 
                if (!isAICalculation)
                    UserInterface.CreatePopup("Player must be on their village to upgrade to a city.");
                return null;
            }
        }

        // method to complete the purchase of a road at the given location
        public void purchaseRoad(Node other)
        {
            board.UpdateConnection(currentPlayer.position, other.position, new Connection(status: "Road", occupant: currentPlayer.GetID()));
            Connection con = board.GetConnection(currentPlayer.position, other.position);
            currentPlayer.addConnection(con);
            currentPlayer.addVictoryPoints(VICTORY_POINT_CONVERTOR["Road"]);
            actions.Push(new PlayerPurchase(currentPlayer.GetID(), currentPlayer.position, "Road", other.position));
            ChargePlayer("Road");
            if (!isAICalculation)
            {
                UserInterface.UpdateConnection(other, board.GetNode(currentPlayer.position), con);
            }
        }

        // method to complete the purchase of a wall at the given location
        public void purchaseWall(Node other)
        {
            if (board.GetConnection(currentPlayer.position, other.position).GetStatus() != "Empty")
            {
                return;
            }
            board.UpdateConnection(currentPlayer.position, other.position, new Connection(status: "Wall", occupant: currentPlayer.GetID()));
            Connection con = board.GetConnection(currentPlayer.position, other.position);
            currentPlayer.addConnection(con);
            currentPlayer.addVictoryPoints(VICTORY_POINT_CONVERTOR["Wall"]);
            actions.Push(new PlayerPurchase(currentPlayer.GetID(), currentPlayer.position, "Wall", other.position));
            ChargePlayer("Wall");
            if (!isAICalculation)
                UserInterface.UpdateConnection(other, board.GetNode(currentPlayer.position), con);
        }

        // method to complete the purchase of a village 
        public void purchaseVillage(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status = new Building("Village", currentPlayer.GetID());
            currentPlayer.addBuilding(board.GetNode(currentPlayer.position));
            currentPlayer.addVictoryPoints(VICTORY_POINT_CONVERTOR["Village"]);
            actions.Push(new PlayerPurchase(currentPlayer.GetID(), currentPlayer.position, "Village"));
            ChargePlayer("Village");
            if (!isAICalculation)
                UserInterface.UpdateSettlement(otherPos);
        }

        // method to complete the purchase of a city
        public void purchaseCity(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status.UpgradeVillage();
            currentPlayer.addVictoryPoints(VICTORY_POINT_CONVERTOR["City"]);
            actions.Push(new PlayerPurchase(currentPlayer.GetID(), currentPlayer.position, "City"));
            ChargePlayer("City");
            if (!isAICalculation)
                UserInterface.UpdateSettlement(otherPos);
        }


        // method to get the possible locations for the player to move to
        public List<Node> attemptPlayerMove()
        {
            
            List<Node> viableLocations = new List<Node>();
            Dijkstra(board, currentPlayer.position);
            // use a Linq to only select the nodes that are within the players movement range
            viableLocations = distance.Where(x => x.Value <= currentPlayer.moves).Select(x => x.Key).ToList();
            // sort viable locations by distance descending (AI optimisation)
            viableLocations = viableLocations.OrderByDescending(x => distance[x]).ToList();

            if (viableLocations.Count == 0 && currentPlayer.moves == MAX_MOVES && !isAICalculation)
            {
                UserInterface.CreatePopup("Something went wrong... Sending you to your capital");
                currentPlayer.position = currentPlayer.GetCapital();
                Stack<Node> home = new Stack<Node>();
                home.Push(board.GetNode(currentPlayer.origin));
                UserInterface.UpdatePlayer(home);
                
            }
            if (!isAICalculation)
                UserInterface.GetUserNodeChoice(viableLocations.ToArray(), MovePlayer); // get the user to choose a location
            return viableLocations;
        }

        // method to move the player to the given location
        private void MovePlayer(Node otherpos)
        {
            if (!isAICalculation)
            {
                Dijkstra(board, currentPlayer.position);
                actions.Push(new PlayerMove(currentPlayer.GetID(), currentPlayer.position, otherpos.position));
                Node current = otherpos;
                Stack<Node> path = new Stack<Node>();
                while (current != board.GetNode(currentPlayer.position))
                {
                    path.Push(current);
                    current = previous[current];
                }
                UserInterface.UpdatePlayer(path); // used to animate the player moving along the path
                currentPlayer.moves -= distance[otherpos];
            }
            currentPlayer.position = otherpos.position;

        }

        // method to refund the player for a structure
        public void Refund(string structure)
        {

            Dictionary<Resource, int> cost = GetCostOfUpgrade(structure);
            foreach (var entry in cost)
            {
                currentPlayer.addResource(entry.Key, entry.Value);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Method to calculate the shortest path to all nodes from the passed start position<br/>
        /// Skill A: Graph Traversal
        /// </summary>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dijkstra(Board board, Vector3 start)
        {
            distance = new Dictionary<Node, int>();
            previous = new Dictionary<Node, Node>();

            Node[] gameBoard = board.GetAllNodes();

            List<Node> Q = new List<Node>();
            // Linq to get the nodes of all other players
            List<Node> occupied = gamePlayers.Where(gamePlayers => gamePlayers.GetID() != currentPlayer.GetID()).Select(gamePlayers => board.GetNode(gamePlayers.position)).ToList();

            foreach (Node node in gameBoard)
            {
                distance.Add(node, int.MaxValue);
                previous.Add(node, null);
                Q.Add(node);
            }

            Node current = board.GetNode(start);
            distance[current] = 0;

            while (Q.Count > 0)
            {

                // Linq to sort the list by distance and get the first element (Priority Queue Implementation)
                current = Q.OrderBy(x => distance[x]).First();
                Q.Remove(current);
                if (distance[current] == int.MaxValue) break; // All remaining nodes are inaccessible so we can safely break
                
                foreach (var g in current.GetNodeNeighbours())
                {
                    Node neighbour = board.GetNode(g);
                    if (neighbour == null)
                    {
                        continue;
                    }
                    else if ((neighbour.status.GetOccupant() != currentPlayer.GetID() && neighbour.status.GetStatus() != "Empty") || occupied.Contains(neighbour))
                    {
                        distance[neighbour] = int.MaxValue;
                        previous[neighbour] = null;
                        continue;
                    }
                    
                    Connection con = board.GetConnection(current.position, g);

                    int NewDist = distance[current] + con.GetWalkingCost(currentPlayer);
                    if (NewDist < distance[neighbour])
                    {
                        distance[neighbour] = NewDist;
                        previous[neighbour] = current;
                    }
                }
            }

            // Prevent the player from moving onto their current position
            distance[board.GetNode(start)] = int.MaxValue; 
        }

        // method to undo the last action of the player
        public void UndoPlayerAction()
        {
            GameAction act = actions.Pop();
            UndoAction(act);
        }

        // method to undo the given game action
        public void UndoAction(GameAction a)
        {
            if (a.type == typeof(PlayerMove))
            {
                PlayerMove move = (PlayerMove)a;
                
                if (!isAICalculation)
                {
                    // if the player is undoing their move, animate back down the path taken
                    Node otherpos = board.GetNode(move.position);
                    Dijkstra(board, move.newpos);
                    Node current = otherpos;
                    Stack<Node> path = new Stack<Node>();
                    while (current != board.GetNode(move.newpos))
                    {
                        path.Push(current);
                        current = previous[current];
                    }
                    currentPlayer.moves += distance[otherpos];
                    UserInterface.UpdatePlayer(path);
                }

                currentPlayer.position = move.position;

            }
            else if (a.type == typeof(PlayerPurchase))
            {
                PlayerPurchase purchase = (PlayerPurchase)a;

                if (purchase.status == "Road" || purchase.status == "Wall")
                {
                    // remove the connection and refund the player
                    Connection con = board.GetConnection(purchase.position, purchase.otherpos);

                    currentPlayer.removeConnection(con);
                    currentPlayer.addVictoryPoints(-VICTORY_POINT_CONVERTOR[con.GetStatus()]);
                    board.UpdateConnection(purchase.position, purchase.otherpos, new Connection());
                    Refund(purchase.status);
                    
                    if (!isAICalculation)
                        UserInterface.UpdateConnection(board.GetNode(purchase.position), board.GetNode(purchase.otherpos), board.GetConnection(purchase.position, purchase.otherpos));
                }
                else if (purchase.status == "Village")
                {
                    currentPlayer.removeBuilding(board.GetNode(currentPlayer.position));
                    currentPlayer.addVictoryPoints(-VICTORY_POINT_CONVERTOR["Village"]);
                    board.GetNode(purchase.position).status = new Building();
                    Refund("Village");

                    if (!isAICalculation)
                        UserInterface.UpdateSettlement(board.GetNode(purchase.position));
                }
                else if (purchase.status == "City")
                {
                    board.GetNode(purchase.position).status.DowngradeVillage();
                    currentPlayer.addVictoryPoints(-VICTORY_POINT_CONVERTOR["City"]);
                    Refund("City");

                    if (!isAICalculation)
                        UserInterface.UpdateSettlement(board.GetNode(purchase.position));
                }
            }
            else
            {
                UserInterface.CreatePopup("Unknown Type");
            }
        }

        // method to perform the given game action
        public void DoAction(GameAction a)
        {
            if (a.type == typeof(PlayerMove))
            {
                // move the player
                PlayerMove move = (PlayerMove)a;                    
                MovePlayer(board.GetNode(move.newpos));
            }
            else if (a.type == typeof(PlayerPurchase))
            {
                // make the purchase
                PlayerPurchase purchase = (PlayerPurchase)a;
                if (purchase.status == "Road")
                {
                    purchaseRoad(board.GetNode(purchase.otherpos));
                }
                else if (purchase.status == "Wall")
                {
                    purchaseWall(board.GetNode(purchase.otherpos));
                }
                else if (purchase.status == "Village")
                {
                    purchaseVillage(board.GetNode(purchase.position));
                }
                else if (purchase.status == "City")
                {
                    purchaseCity(board.GetNode(purchase.position));
                }
            }
            else
            {
                UserInterface.CreatePopup("Unknown Type");
            }
        }
    }

    /// <summary>
    /// The <c>GameAction</c> class represents an action that a player can make.
    /// </summary>
    public abstract class GameAction
    {
        public int playerID; 
        public Vector3 position;
        public Type type;
    }

    /// <summary>
    /// The <c>PlayerMove</c> class represents the game action of moving a player to a new node.
    /// </summary>
    public class PlayerMove : GameAction
    {
        public Vector3 newpos;

        // contructor for the PlayerMove class that takes the origin, new position and player moving
        public PlayerMove(int playerID, Vector3 position, Vector3 newpos)
        {
            this.playerID = playerID;
            this.position = position;
            this.newpos = newpos;
            type = GetType();
        }

        // override the ToString method to return the move
        public override string ToString()
        {
            return $"player {playerID} moved from {position} to {newpos}";
        }
    }

    /// <summary>
    /// The <c>PlayerPurchase</c> class represents the game action of a player making a shop purchase.
    /// </summary>
    public class PlayerPurchase : GameAction
    {
        public Vector3 otherpos;
        public string status;

        // constructor for the PlayerPurchase class that takes the playerID, position, status and other position (if needed for purchase)
        public PlayerPurchase(int playerID, Vector3 position, string status, Vector3 otherpos = new Vector3())
        {
            this.playerID = playerID;
            this.position = position;
            this.otherpos = otherpos;
            type = GetType();
            this.status = status;
        }

        // override the ToString method to return the purchase
        public override string ToString()
        {
            if (otherpos == new Vector3())
            {
                return $"player {playerID} Purchasing a {status} at {position}";
            }
            else
            {
                return $"player {playerID} Purchasing a {status} from {position} to {otherpos}";
            }
        }
    }
}
