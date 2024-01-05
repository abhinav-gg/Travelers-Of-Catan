using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;



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

        /// <summary>
        /// Used primarily for the AI interaction
        /// </summary>
        Dictionary<Node, int> distance = new Dictionary<Node, int>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();
        // AI func
        public bool isAICalculation;
        public Stack<GameAction> actions = new Stack<GameAction>();
        /// <summary>
        /// Constructor for a new game
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="win"></param>
        /// <param name="start"></param>
        /// <param name="maxtime"></param>
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
        /// <param name="ui"></param>
        /// <param name="game"></param>
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

            board = new Board(game.board);
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

            turn = game.turn;
            UserInterface.DisplayBoard(board);
            UserInterface.DisplayPlayers(gamePlayers);
            currentPlayer = gamePlayers[turn]; // moves are already saved

        }


        public void AddPlayer(string Name, string color="teal")
        {
            int i = gamePlayers.Count;
            gamePlayers.Add(new Player(playerNumber: i + 1,playerName: Name, playerColor: color, origin: StartingCoords[i]));
        }

        public void AddAI(string Name, string color)
        {
            int i = gamePlayers.Count;
            gamePlayers.Add(new AI(playerID: i + 1, name: Name, playerColor: color, home: StartingCoords[i], this));


        }

        public void UpdateCurrentPlayer(int id)
        {
            foreach (Player pdl in gamePlayers)
            {
                if (pdl.GetID() == id)
                {
                    currentPlayer = pdl;
                }
            }
            currentPlayer.moves = 3;
            UserInterface.Assert(isAICalculation);
            
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

                Building capital = new Building("City", current.GetID());
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
            turn = -1;
            
            EndTurn();

        }

        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public void StartTurn(float timeleft=-1f)
        {
            if (timeleft == -1f)
            {
                gatherResources(currentPlayer);
                timeleft = TimePerMove;
            }     

            if (currentPlayer.GetType() == typeof(Player))
            {
                isAICalculation = false;
                
            }
            else if (currentPlayer.GetType() == typeof(AI))
            {
                isAICalculation = true;
            }
            UserInterface.BeginTurn(timeleft);

        }

        public void DisplayAIMoves()
        {
            currentPlayer = gamePlayers[turn];
            isAICalculation = false;
            Stack<GameAction> selectedMoves = ((AI)currentPlayer).selectedMoves;
            // reverse order of stack
            Stack<GameAction> temp = new Stack<GameAction>();
            while (selectedMoves.Count > 0)
            {
                temp.Push(selectedMoves.Pop());
            }
            while (temp.Count > 0)
            {
                DoAction(temp.Pop());
            }
            
        }


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
            currentPlayer.moves = 3; // Allow this to change as a gamemode
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

        public void gatherResources(Player pdl)
        {
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
        
        public void undoGatherResources(Player pdl)
        {
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

        /// <summary>
        /// Trading Function!
        /// </summary>
        /// <param name="otherPlayer"></param>
        /// <param name="CurrentPlayerChange"></param>

        public void CompleteTrade(Player otherPlayer, Dictionary<Resource, int> CurrentPlayerChange)
        {
            foreach (var entry in CurrentPlayerChange)
            {
                if (entry.Value > 0)
                {
                    currentPlayer.addResource(entry.Key, entry.Value);
                    otherPlayer.removeResource(entry.Key, entry.Value);
                    for (int _ = 0; _ < entry.Value; _++)
                        UserInterface.ShowResource(otherPlayer.position, entry.Key, currentPlayer.position);
                }
                else if (entry.Value < 0)
                {
                    currentPlayer.removeResource(entry.Key, entry.Value);
                    otherPlayer.addResource(entry.Key, entry.Value);
                    for (int _ = 0; _ < entry.Value; _++)
                        UserInterface.ShowResource(currentPlayer.position, entry.Key, otherPlayer.position);
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
            if (!isAICalculation)
            {
                CheckWinner();
                //UserInterface.CreatePopup("Purchase Successful");

            }

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
        public List<Node> tryPurchaseRoad()
        {
            if (!CheckCosts("Road"))
            {
                if (!isAICalculation)
                    UserInterface.CreatePopup("You can not afford this");
                return new List<Node>();
            }
            List<Node> viableLocations = new List<Node>();
            Node otherPos;
            bool canconnect = board.GetNode(currentPlayer.position).status.GetOccupant() == currentPlayer.GetID(); // player must be standing on their own settlement to build a road
            
            foreach (Vector3 vOther in board.GetNode(currentPlayer.position).GetNodeNeighbours() )
            {
                if (board.GetNode(vOther) == null) continue; // this is a null node (out of bounds
                
                Connection con = board.GetConnection(currentPlayer.position, vOther);
                otherPos = board.GetNode(vOther);
                if (con.GetStatus() != "Empty")
                {
                    continue; // can not build a road on existing connections of any sort
                }
                else if ((otherPos.status.GetOccupant() != currentPlayer.GetID()) && (otherPos.status.GetStatus() != "Empty"))
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
                UserInterface.GetUserNodeChoice(viableLocations.ToArray(), purchaseRoad);
            return viableLocations;
        
        }
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
            bool canconnect = board.GetNode(currentPlayer.position).status.GetOccupant() == currentPlayer.GetID(); // player must be standing on their own settlement to build a road

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
            if (!isAICalculation)
                UserInterface.GetUserNodeChoice(viableLocations.ToArray(), purchaseWall);
            return viableLocations;

        }
        public Node tryPurchaseVillage()
        {
            if (!CheckCosts("Village"))
            {
                if (!isAICalculation)
                    UserInterface.CreatePopup("You can not afford this");
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

            if (isConnecting || DistanceRule)
            {
                if (!isAICalculation)
                    UserInterface.GetUserNodeChoice(new Node[] { current }, purchaseVillage);
                return current;
            }
            else
            {
                if (!isAICalculation)
                    UserInterface.CreatePopup($"{DistanceRule} {isConnecting}");
                return null;
            }


        }
        public Node tryPurchaseCity()
        {
            if (!CheckCosts("City"))
            {
                if (!isAICalculation)
                    UserInterface.CreatePopup("You can not afford this");
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
                    UserInterface.CreatePopup("Player must be on their village to upgrade");
                return null;
            }
        }
        public void purchaseRoad(Node other)
        {
            board.UpdateConnection(currentPlayer.position, other.position, new Connection(status: "Road", occupant: currentPlayer.GetID()));
            ChargePlayer("Road");
            Connection con = board.GetConnection(currentPlayer.position, other.position);
            currentPlayer.addConnection(con);
            actions.Push(new PlayerPurchase(currentPlayer.GetID(), currentPlayer.position, "Road", other.position));
            if (!isAICalculation)
            {
                UserInterface.UpdateConnection(other, board.GetNode(currentPlayer.position), con);
            }
        }
        public void purchaseWall(Node other)
        {
            if (board.GetConnection(currentPlayer.position, other.position).GetStatus() != "Empty")
            {
                UserInterface.Assert(isAICalculation);
                return; // issue caused by the AI 
            }
            board.UpdateConnection(currentPlayer.position, other.position, new Connection(status: "Wall", occupant: currentPlayer.GetID()));
            ChargePlayer("Wall");
            Connection con = board.GetConnection(currentPlayer.position, other.position);
            currentPlayer.addConnection(con);
            actions.Push(new PlayerPurchase(currentPlayer.GetID(), currentPlayer.position, "Wall", other.position));
            if (!isAICalculation)
                UserInterface.UpdateConnection(other, board.GetNode(currentPlayer.position), con);

        }
        public void purchaseVillage(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status = new Building("Village", currentPlayer.GetID());
            ChargePlayer("Village");
            currentPlayer.addBuilding(board.GetNode(currentPlayer.position));
            actions.Push(new PlayerPurchase(currentPlayer.GetID(), currentPlayer.position, "Village"));
            if (!isAICalculation)
                UserInterface.UpdateSettlement(otherPos);
        }
        public void purchaseCity(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status.UpgradeVillage();
            ChargePlayer("City");
            currentPlayer.upgradeVillage(board.GetNode(currentPlayer.position));
            actions.Push(new PlayerPurchase(currentPlayer.GetID(), currentPlayer.position, "City"));
            if (!isAICalculation)
                UserInterface.UpdateSettlement(otherPos);
        }


        /// <summary>
        /// Player Movement
        /// </summary>

        public List<Node> attemptPlayerMove()
        {
            
            List<Node> viableLocations = new List<Node>();
            Dijkstra(board, currentPlayer.position);
            // use a Linq to only select the nodes that are within the players movement range
            viableLocations = distance.Where(x => x.Value <= currentPlayer.moves).Select(x => x.Key).ToList();
            // sort viable locations by distance descending (AI optimisation)
            viableLocations = viableLocations.OrderByDescending(x => distance[x]).ToList();

            if (viableLocations.Count == 0 && currentPlayer.moves > 0 && !isAICalculation)
            {
                UserInterface.CreatePopup("Something went wrong... Sending you to your capital");
                currentPlayer.position = currentPlayer.GetCapital();
                Stack<Node> home = new Stack<Node>();
                home.Push(board.GetNode(currentPlayer.origin));
                UserInterface.UpdatePlayer(home);
                
            }
            if (!isAICalculation)
                UserInterface.GetUserNodeChoice(viableLocations.ToArray(), MovePlayer);
            return viableLocations;
        }

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
                UserInterface.UpdatePlayer(path);
                currentPlayer.moves -= distance[otherpos];
            }
            currentPlayer.position = otherpos.position;
            if (currentPlayer.playerName == "test") { currentPlayer.moves = 3; } // for testing purposes


        }

        /// <summary>
        /// AI Functions
        /// </summary>

        public void Refund(string structure)
        {

            Dictionary<Resource, int> cost = GetCostOfUpgrade(structure);
            foreach (var entry in cost)
            {
                currentPlayer.addResource(entry.Key, entry.Value);
            }


        }

        public void Dijkstra(Board board, Vector3 start)
        {

            // Dijkstra's algorithm
            distance = new Dictionary<Node, int>();
            previous = new Dictionary<Node, Node>();


            Node[] gameBoard = board.GetAllNodes();


            List<Node> Q = new List<Node>();
            // use linq to get the nodes of all other players
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

                // Linq to sort the list by distance and get first element (Priority Queue Implementation)
                current = Q.OrderBy(x => distance[x]).First();
                Q.Remove(current);
                if (distance[current] == int.MaxValue) break; // all remaining nodes are inaccessible
                
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

            distance[board.GetNode(start)] = int.MaxValue; // this is to prevent the player from moving back onto their current position

        }

        public void UndoPlayerAction()
        {
            UserInterface.Assert(!currentPlayer.isPlayerAI());
            
            GameAction act = actions.Pop();
            UndoAction(act);

        }

        public void UndoAction(GameAction a)
        {

            if (a.type == typeof(PlayerMove))
            {
                PlayerMove move = (PlayerMove)a;
                
                UserInterface.Assert(currentPlayer.position == move.newpos);


                if (!isAICalculation)
                {
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
                    

                    Connection con = board.GetConnection(purchase.position, purchase.otherpos);
                    UserInterface.Assert(con.GetStatus() == purchase.status);
                    UserInterface.Assert(con.GetOccupant() == currentPlayer.GetID());

                    currentPlayer.removeConnection(con);
                    board.UpdateConnection(purchase.position, purchase.otherpos, new Connection());
                    Refund(purchase.status);
                    
                    if (!isAICalculation)
                        UserInterface.UpdateConnection(board.GetNode(purchase.position), board.GetNode(purchase.otherpos), board.GetConnection(purchase.position, purchase.otherpos));


                }
                else if (purchase.status == "Village")
                {

                    UserInterface.Assert(board.GetNode(purchase.position).status.GetStatus() == "Village");
                    UserInterface.Assert(board.GetNode(purchase.position).status.GetOccupant() == currentPlayer.GetID());

                    currentPlayer.removeBuilding(board.GetNode(currentPlayer.position));
                    board.GetNode(purchase.position).status = new Building();
                    Refund("Village");

                    if (!isAICalculation)
                        UserInterface.UpdateSettlement(board.GetNode(purchase.position));


                }
                else if (purchase.status == "City")
                {

                    UserInterface.Assert(board.GetNode(purchase.position).status.GetStatus() == "City");


                    board.GetNode(purchase.position).status.DowngradeVillage();
                    currentPlayer.undoUpgradeVillage(board.GetNode(currentPlayer.position));
                    UserInterface.UpdateSettlement(board.GetNode(purchase.position));

                    Refund("City");

                }
            }
            else
            {
                UserInterface.CreatePopup("Unknown Type");
            }
        }

        public void DoAction(GameAction a)
        {
           
            if (a.type == typeof(PlayerMove))
            {

                PlayerMove move = (PlayerMove)a;                    
                MovePlayer(board.GetNode(move.newpos));

                
            }
            else if (a.type == typeof(PlayerPurchase))
            {
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
        


        public void ShowActions(Stack<GameAction> actions)
        {
            string allacts = "";
            // go through stack without changing it

            foreach (GameAction a in actions)
            {
                allacts += a.ToString() + "\n";
            }

            UserInterface.CreatePopup(allacts);


        }

    }


    public abstract class GameAction
    {
        public int playerID; 
        public Vector3 position;
        public Type type;

    }

    public class PlayerMove : GameAction
    {
        public Vector3 newpos;

        public PlayerMove(int playerID, Vector3 position, Vector3 newpos)
        {
            this.playerID = playerID;
            this.position = position;
            this.newpos = newpos;
            type = GetType();
        }

        public override string ToString()
        {
            return $"player {playerID} moved from {position} to {newpos}";
        }


    }

    public class PlayerPurchase : GameAction
    {
        public Vector3 otherpos;
        public string status;

        public PlayerPurchase(int playerID, Vector3 position, string status, Vector3 otherpos = new Vector3())
        {
            this.playerID = playerID;
            this.position = position;
            this.otherpos = otherpos;
            type = GetType();
            this.status = status;
        }

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
