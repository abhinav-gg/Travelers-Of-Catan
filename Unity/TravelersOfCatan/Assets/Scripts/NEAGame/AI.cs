using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Diagnostics.Contracts;

namespace NEAGame
{

    /// <summary>
    /// Class that inherits from Player and adds AI functionality
    /// Skill A: Complex OOP - Inheritance
    /// </summary>
    class AI : Player
    {
        public enum Turn
        {
            Max,
            Min
        }
        public Stack<GameAction> selectedMoves = new Stack<GameAction>();
        public Stack<GameAction> currentMove = new Stack<GameAction>();
        private readonly int MaxDepth = 4;
        TravelersOfCatan gameRef;

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
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int BRS(int alpha=int.MinValue + 1, int beta=int.MaxValue, int depth=-1, Turn turn=Turn.Max)
        {
            if (depth == -1)
            {
                depth = MaxDepth;
            }

            List<List<GameAction>> AllMoves = new List<List<GameAction>>();
            
            if (depth == 0)
            {
                return (int)Math.Pow(-1, MaxDepth) * StaticEval();
            }

            if (turn == Turn.Max)
            {
                AllMoves = GenerateMoves(this).ToList();
                turn = Turn.Min;
            }
            else if (turn == Turn.Min)
            {
                
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

            // sort allmoves by count to optimise alpha beta pruning
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
                

                gameRef.UpdateCurrentPlayer(m[0].playerID);
                for (int i = m.Count - 1; i > -1; i--)
                {
                    
                    gameRef.UndoAction(m[i]);
                    currentMove.Pop();
                }
                gameRef.undoGatherResources(m[0].playerID);
                
                if (v >= beta)
                {
                    return v;
                }
                if (v > alpha)
                {
                    if (depth == MaxDepth)
                    {
                        selectedMoves = new Stack<GameAction>(m);
                    }
                    alpha = v;
                }
                

            }

            return alpha;

        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Method to generate possible good moves on the passed player's turn
        /// <br/>Skill A: Dynamic generation of class objects
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public IEnumerable<List<GameAction>> GenerateMoves(Player pdl)
        {
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

        public static Stack<GameAction> Clone(Stack<GameAction> stack)
        {
            Contract.Requires(stack != null);
            return new Stack<GameAction>(new Stack<GameAction>(stack));
        }
    }
}
