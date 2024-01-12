using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Diagnostics.Contracts;

namespace NEAGame
{
    class AI : Player
    {
        public enum Turn
        {
            Max,
            Min
        }
        public Stack<GameAction> selectedMoves = new Stack<GameAction>();
        Stack<GameAction> currentMove = new Stack<GameAction>();
        private readonly int MaxDepth = 3;
        TravelersOfCatan gameRef;

        public AI(int playerID, string name, string playerColor, Vector3 home, TravelersOfCatan reference) : base(playerNumber: playerID, playerName:name, playerColor: playerColor, origin:home)
        {
            isAI = true;
            gameRef = reference;
        }

        public AI(PlayerWrapper player) : base(player)
        {
            isAI = true;
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

        public int BRS(int alpha=int.MinValue + 1, int beta=int.MaxValue, int depth=-1, Turn turn=Turn.Max)
        {
            if (depth == -1)
            {
                depth = MaxDepth;
            }

            List<List<GameAction>> AllMoves = new List<List<GameAction>>();
            
            if (depth == 0)
            {
                return StaticEval();
            }

            /*GameAction lastMove = currentMove.Peek();
            if (lastMove.type != typeof(PlayerMove))
            {
                foreach (Player pdl in gameRef.gamePlayers)
                {
                    if (pdl.GetID() == lastMove.playerID)
                    {
                        AllMoves.AddRange(GenerateMoves(pdl));
                    }
                }
            }
            else*/
            
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
                gameRef.actions.Clear();
                for (int i = 0; i < m.Count; i++)
                {
                    currentMove.Push(m[i]);
                    gameRef.DoAction(m[i]);
                }

                int v = 0;
                if (turn == Turn.Min)
                {
                    // gather resources for all other players
                    foreach (Player pdl in gameRef.gamePlayers)
                    {
                        if (pdl.GetID() != playerNumber)
                        {
                            gameRef.gatherResources(pdl);
                        }
                    }
                    v = -BRS(-beta, -alpha, depth-1, turn);
                    foreach (Player pdl in gameRef.gamePlayers)
                    {
                        if (pdl.GetID() != playerNumber)
                        {
                            gameRef.undoGatherResources(pdl);
                        }
                    }
                }
                else if (turn == Turn.Max)
                {
                    gameRef.UpdateCurrentPlayer(playerNumber);
                    gameRef.gatherResources(this);
                    v = -BRS(-beta, -alpha, depth-1, turn);
                    gameRef.UpdateCurrentPlayer(playerNumber);
                    gameRef.undoGatherResources(this);
                }

                gameRef.UpdateCurrentPlayer(m[0].playerID);
                for (int i = m.Count - 1; i > -1; i--)
                {
                    
                    gameRef.UndoAction(m[i]);
                    currentMove.Pop();
                }
                
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

        public IEnumerable<List<GameAction>> GenerateMoves(Player pdl)
        {
            int playerID = pdl.GetID();
            gameRef.UpdateCurrentPlayer(playerID);
            List<Node> allMoves = gameRef.attemptPlayerMove();
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
        }

        public static Stack<GameAction> Clone(Stack<GameAction> stack)
        {
            Contract.Requires(stack != null);
            return new Stack<GameAction>(new Stack<GameAction>(stack));
        }

    }


}
