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
        private Stack<GameAction> actions = new Stack<GameAction>();
        Dictionary<Node, int> distance = new Dictionary<Node, int>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

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

            this.board = new Board(game.board);
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
                timeleft = TimePerMove;
            }     

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
            actions.Clear();
            if (HasWinner()) return;
            if (turn != -1)
            {

                gatherResources();
            }
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
            board.UpdateConnection(currentPlayer.position, other.position, new Connection(status: "Road", occupant: currentPlayer.GetID()));
            ChargePlayer("Road");
            Connection con = board.GetConnection(currentPlayer.position, other.position);
            currentPlayer.addConnection(con);
            UserInterface.UpdateConnection(other, board.GetNode(currentPlayer.position), con);
            actions.Push(new PlayerPurchase(currentPlayer.getNumber(), currentPlayer.position, "Road", other.position));
        }
        public void purchaseWall(Node other)
        {
            board.UpdateConnection(currentPlayer.position, other.position, new Connection(status: "Wall", occupant: currentPlayer.GetID()));
            ChargePlayer("Wall");
            Connection con = board.GetConnection(currentPlayer.position, other.position);
            currentPlayer.addConnection(con);
            UserInterface.UpdateConnection(other, board.GetNode(currentPlayer.position), con);
            actions.Push(new PlayerPurchase(currentPlayer.getNumber(), currentPlayer.position, "Wall", other.position));

        }
        public void purchaseVillage(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status = new Building("Village", currentPlayer.getNumber());
            ChargePlayer("Village");
            currentPlayer.addBuilding(board.GetNode(currentPlayer.position));
            UserInterface.UpdateSettlement(otherPos);
            actions.Push(new PlayerPurchase(currentPlayer.getNumber(), currentPlayer.position, "Village"));
        }
        public void purchaseCity(Node otherPos)
        {
            board.GetNode(currentPlayer.position).status.UpgradeVillage();
            ChargePlayer("City");
            currentPlayer.upgradeVillage(board.GetNode(currentPlayer.position));
            UserInterface.UpdateSettlement(otherPos);
            actions.Push(new PlayerPurchase(currentPlayer.getNumber(), currentPlayer.position, "City"));

        }

        /// <summary>
        /// Player Movement
        /// </summary>

        public void attemptPlayerMove()
        {
            
            List<Node> viableLocations = new List<Node>();
            /*Connection con;
            Node target;
            bool valid;
            foreach (Vector3 pos in board.GetNode(currentPlayer.position).GetNodeNeighbours())
            {
                valid = true;
                con = board.GetConnection(pos, currentPlayer.position);
                if (board.GetNode(pos) == null) continue;
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
            */
            Dijkstra(board, currentPlayer.position);
            // use a Linq to only select the nodes that are within the players movement range
            viableLocations = distance.Where(x => x.Value <= currentPlayer.moves).Select(x => x.Key).ToList();

            if (viableLocations.Count == 0 && currentPlayer.moves > 0)
            {
                UserInterface.CreatePopup("Something went wrong... Sending you to your capital");
                currentPlayer.position = currentPlayer.GetCapital();
                Stack<Node> home = new Stack<Node>();
                home.Push(board.GetNode(currentPlayer.origin));
                UserInterface.UpdatePlayer(home);
                return;
            }
            UserInterface.GetUserNodeChoice(viableLocations.ToArray(), MovePlayer);

        }

        private void MovePlayer(Node otherpos)
        {
            actions.Push(new PlayerMove(currentPlayer.getNumber(), currentPlayer.position, otherpos.position));

            Node current = otherpos;
            Stack<Node> path = new Stack<Node>();
            while (current != board.GetNode(currentPlayer.position))
            {
                path.Push(current);
                current = previous[current];
                //actions.Push(new PlayerMove(currentPlayer.getNumber(), current.position, previous[current].position));
            }

            currentPlayer.moves -= distance[otherpos];
            currentPlayer.position = otherpos.position;
            if (currentPlayer.playerName == "test") { currentPlayer.moves = 3; } // for testing purposes

            UserInterface.UpdatePlayer(path);

        }

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
                if ((current.status.GetOccupant() != currentPlayer.GetID() && current.status.GetStatus() != "Empty") || occupied.Contains(current))
                {
                    // can not move through enemy settlements
                    distance[current] = int.MaxValue;
                    previous[current] = null;
                    continue;
                }
                foreach (var g in current.GetNodeNeighbours())
                {
                    Node neighbour = board.GetNode(g);
                    if (neighbour == null) continue;
                    
                    
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

        public void DoAction(GameAction a)
        {

        }

        public void UndoAction()
        {
            if (actions.Count == 0) return;
            ShowActions();

            GameAction a = actions.Pop();
            if (a.type == typeof(PlayerMove))
            {
                PlayerMove move = (PlayerMove)a;
                UserInterface.Assert(currentPlayer.position == move.newpos);
                Dijkstra(board, move.newpos);
                Node otherpos = board.GetNode(move.position);
                Node current = otherpos;
                Stack<Node> path = new Stack<Node>();
                while (current != board.GetNode(currentPlayer.position))
                {
                    path.Push(current);
                    current = previous[current];
                }

                currentPlayer.moves += distance[otherpos];
                currentPlayer.position = otherpos.position;

                UserInterface.UpdatePlayer(path);


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
                    UserInterface.UpdateConnection(board.GetNode(purchase.position), board.GetNode(purchase.otherpos), board.GetConnection(purchase.position, purchase.otherpos));

                    Refund(purchase.status);

                }
                else if (purchase.status == "Village")
                {

                    UserInterface.Assert(board.GetNode(purchase.position).status.GetStatus() == "Village");
                    UserInterface.Assert(board.GetNode(purchase.position).status.GetOccupant() == currentPlayer.GetID());

                    currentPlayer.removeBuilding(board.GetNode(currentPlayer.position));
                    board.GetNode(purchase.position).status = new Building();
                    UserInterface.UpdateSettlement(board.GetNode(purchase.position));

                    Refund("Village");


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
        }

        public void ShowActions()
        {

            // go through stack without changing it

            foreach (GameAction a in actions)
            {
                UserInterface.CreatePopup(a.ToString());
            }


        }

    }


    public abstract class GameAction
    {
        public int playerID; // for verification
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
            return $"player {playerID} Purchasing a {status} from {position}";
        }



    }



}
