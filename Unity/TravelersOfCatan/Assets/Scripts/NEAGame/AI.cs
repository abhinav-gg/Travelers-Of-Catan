using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace NEAGame
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// The <c>AI</c> class inherits from Player and is used to represent an AI player in the game. This contains all of the AI's logic and decision making. <br/>
    /// Skill A: Complex User-Defined OOP - Inheritance
    /// </summary>
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class AI : Player
    {
        public enum Turn
        {
            Max,
            Min
        }
        public Stack<GameAction> selectedMoves = new Stack<GameAction>();
        public Stack<GameAction> currentMove = new Stack<GameAction>();
        private readonly int MAX_DEPTH = 4;
        private TravelersOfCatan gameRef;

        // Constructor for AI that creates a new player
        public AI(int playerID, string name, string playerColor, Vector3 home, TravelersOfCatan reference) : base(playerNumber: playerID, playerName:name, color: playerColor, origin:home)
        {
            isAI = true;
            gameRef = reference;
        }

        // Constructor for AI that loads a player from a save file
        public AI(PlayerWrapper player, TravelersOfCatan reference) : base(player)
        {
            isAI = true;
            gameRef = reference;
        }

        // Method to get the static evaluation of the current game position based on player's wealth and victory points
        public int StaticEval()
        {
            int score = 0;

            foreach (Player pdl in gameRef.gamePlayers)
            {
                if (pdl.GetID() == playerNumber)
                {
                    if (pdl.getVictoryPoints() > gameRef.WinningVictoryPoints)
                    {
                        score += 1000;
                    }
                    score += pdl.GetWealth();
                }
                else
                {
                    if (pdl.getVictoryPoints() > gameRef.WinningVictoryPoints)
                    {
                        score -= 1000;
                    }
                    score -= pdl.GetWealth();
                }   
            }
            return score;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Runs the Best Reply Search algorithm on the current game position to attempt to determine the best move to make.
        /// <br/>Skill A: Recursive Algorithm
        /// <br/>Skill A: Tree traversal
        /// <br/>Skill A: Complex User-Defined Algorithm
        /// <br/>Skill A: Stack Operations
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int BRS(int alpha=int.MinValue + 1, int beta=int.MaxValue, int depth=-1, Turn turn=Turn.Max)
        {
            if (depth == -1)
            {
                depth = MAX_DEPTH;
            }

            List<List<GameAction>> AllMoves = new List<List<GameAction>>();
            
            if (depth == 0)
            {
                // this line was altered from the original code to fix a bug where the AI returns the negative of the static evaluation on all even depths
                return (int)Math.Pow(-1, MAX_DEPTH) * StaticEval();
            }

            if (turn == Turn.Max)
            {
                // if it is the AI's turn, generate all possible moves the AI has
                AllMoves = GenerateMoves(this).ToList();
                turn = Turn.Min;
            }
            else if (turn == Turn.Min)
            {
                // if it is the player's turn, generate all possible moves that any player has
                foreach (Player pdl in gameRef.gamePlayers)
                {
                    if (pdl.GetID() == playerNumber)
                    {
                        continue;
                    }
                    AllMoves.AddRange(GenerateMoves(pdl));
                }
                turn = Turn.Max;
            }

            // sort the moves by their length to optimise alpha beta pruning
            // this means that the moves that are most likely to be good are evaluated first
            AllMoves.Sort((x, y) => y.Count.CompareTo(x.Count));
            
            foreach (var m in AllMoves)
            {

                gameRef.UpdateCurrentPlayer(m[0].playerID);
                gameRef.gatherResources(m[0].playerID);
                gameRef.actions.Clear();
                for (int i = 0; i < m.Count; i++)
                {
                    currentMove.Push(m[i]);
                    gameRef.DoAction(m[i]);
                }

                int v = -BRS(-beta, -alpha, depth-1, turn);
                // recursively calls the BRS algorithm to move to the next depth
                

                gameRef.UpdateCurrentPlayer(m[0].playerID);
                for (int i = m.Count - 1; i > -1; i--)
                {
                    
                    gameRef.UndoAction(m[i]);
                    currentMove.Pop();
                }
                // undoes the action to return to the previous game state
                gameRef.undoGatherResources(m[0].playerID);
                
                if (v >= beta)
                {
                    return v;
                }
                if (v > alpha)
                {
                    if (depth == MAX_DEPTH)
                    {
                        // if the move is the best move found so far, store it in the selectedMoves stack
                        selectedMoves = new Stack<GameAction>(m);
                    }
                    alpha = v;
                }
            }
            return alpha; // returns the evaluation of the position
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Method to generate possible good moves on the passed player's turn
        /// <br/>Skill A: Dynamic generation of class objects
        /// <br/>Skill A: Stack Operations
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public IEnumerable<List<GameAction>> GenerateMoves(Player pdl)
        {
            // get all combinations of making a purchase followed by moving to a new node
            int playerID = pdl.GetID();
            gameRef.UpdateCurrentPlayer(playerID);
            gameRef.gatherResources(playerID);
            List <Node> allMoves = gameRef.attemptPlayerMove();
            allMoves.Add(gameRef.board.GetNode(pdl.position));
            if (gameRef.tryPurchaseCity() != null)
            {
                foreach (Node end in allMoves)
                {
                    yield return new List<GameAction> {
                        new PlayerPurchase(playerID, pdl.position, "City"),
                        new PlayerMove(playerID, pdl.position, end.position)
                    };
                }
            }

            foreach (Node n in gameRef.tryPurchaseRoad())
            {
                foreach (Node end in allMoves)
                {
                    yield return new List<GameAction> {
                        new PlayerPurchase(playerID, pdl.position, "Road", n.position),
                        new PlayerMove(playerID, pdl.position, end.position)
                    };
                }
            }

            if (gameRef.tryPurchaseVillage() != null)
            {
                foreach (Node end in allMoves)
                {
                    yield return new List<GameAction> {
                        new PlayerPurchase(playerID, pdl.position, "Village"),
                        new PlayerMove(playerID, pdl.position, end.position)
                    };
                }
            }

            foreach (Node n in gameRef.tryPurchaseWall())
            {
                foreach (Node end in allMoves)
                {
                    yield return new List<GameAction> {
                        new PlayerPurchase(playerID, pdl.position, "Wall", n.position),
                        new PlayerMove(playerID, pdl.position, end.position)
                    };
                }
            }

            foreach (Node end in allMoves)
            {
                yield return new List<GameAction> {
                        new PlayerMove(playerID, pdl.position, end.position)
                    };
            }
            gameRef.undoGatherResources(playerID);
        }
    }
}
