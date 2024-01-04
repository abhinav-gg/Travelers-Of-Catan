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
        int MaxDepth = 4; // make readonly
        TravelersOfCatan gameRef;
        int Settler;

        public AI(int playerID, string name, string playerColor, Vector3 home, TravelersOfCatan reference) : base(playerNumber: playerID, playerName:name, playerColor: playerColor, origin:home)
        {
            isAI = true;
            gameRef = reference;
        }

        public AI(PlayerWrapper player) : base(player)
        {
            isAI = true;
        }

        public void StartBRSThread(int alpha, int beta)
        {
            Thread t = new Thread(() => BRS(alpha, beta));
            t.Start();
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

        public int BRS(int alpha=int.MinValue, int beta=int.MaxValue, int depth=-1, Turn turn=Turn.Max)
        {
            if (depth == -1)
            {
                depth = MaxDepth;
                Settler = 5 - gameRef.gamePlayers.Count;
            }

            List<GameAction> AllMoves = new List<GameAction>();

            if (depth <= 0)
            {
                return StaticEval();
            }
            if (turn == Turn.Max)
            {
                AllMoves = GenerateMoves(this).ToList();
            }
            else if (turn == Turn.Min)
            {
                GameAction lastMove = currentMove.Peek();
                if (lastMove.type != typeof(PlayerMove) && lastMove.playerID != playerNumber)
                {
                    foreach (Player pdl in gameRef.gamePlayers)
                    {
                        if (pdl.GetID() == lastMove.playerID)
                        {
                            AllMoves.AddRange(GenerateMoves(pdl));
                        }
                    }
                }
                else
                {
                    foreach (Player pdl in gameRef.gamePlayers)
                    {
                        if (pdl.GetID() == playerNumber)
                        {
                            continue;
                        }
                        AllMoves.AddRange(GenerateMoves(pdl));
                    }
                }
            }
            int initAlpha = alpha;
            int initBeta = beta;
            Console.Write(AllMoves);
            foreach (GameAction m in AllMoves)
            {

                gameRef.UpdateCurrentPlayer(m.playerID);
                currentMove.Push(m);
                gameRef.DoAction(m);
                int v = int.MinValue;
                
                if (m.type == typeof(PlayerMove))
                {
                    
                    gameRef.actions.Clear();
                    if (turn == Turn.Max)
                    {
                        // gather resources for all other players
                        foreach (Player pdl in gameRef.gamePlayers)
                        {
                            if (pdl.GetID() != playerNumber)
                            {
                                gameRef.gatherResources(pdl);
                            }
                        }
                        v = BRS(-beta, -alpha, depth-1, Turn.Min);
                        foreach (Player pdl in gameRef.gamePlayers)
                        {
                            if (pdl.GetID() != playerNumber)
                            {
                                gameRef.undoGatherResources(pdl);
                            }
                        }
                    }
                    else if (turn == Turn.Min)
                    {
                        gameRef.UpdateCurrentPlayer(playerNumber);
                        gameRef.gatherResources(this);
                        v = BRS(-beta, -alpha, depth-1, Turn.Max);
                        gameRef.UpdateCurrentPlayer(playerNumber);
                        gameRef.undoGatherResources(this);
                    }

                } 
                else
                {
                    v = BRS(initAlpha, initBeta, depth, turn); // only update the position!
                    // alpha and beta are updated in the recursive call so the original values are preserved for the next iteration
                }
                
                gameRef.UpdateCurrentPlayer(m.playerID);
                gameRef.UndoAction(m);
                currentMove.Pop();
                
                if (v >= beta)
                {
                    return v;
                }
                if (v > alpha)
                {
                    if (depth == MaxDepth && m.type == typeof(PlayerMove))
                    {
                        selectedMoves = Clone(currentMove);
                        selectedMoves.Push(m);
                        Settler--;
                        if (Settler <= 0)
                        {
                            return v; // return the value of the move that was just made if it is the last move of the turn to save time
                        }
                    }
                    alpha = v;
                }
                

            }

            return alpha;


            // find resources required and use that to find the closest position to go to
            // The ai can not strategically place walls but it should be able to place roads and settlements somehow...
        }

        public IEnumerable<GameAction> GenerateMoves(Player pdl)
        {

            // base case move to the same position
            gameRef.UpdateCurrentPlayer(pdl.GetID());
            if (gameRef.tryPurchaseCity() != null)
            {
                yield return new PlayerPurchase(pdl.GetID(), pdl.position, "City");
            }

            foreach (Node n in gameRef.tryPurchaseRoad())
            {
                yield return new PlayerPurchase(pdl.GetID(), pdl.position, "Road", n.position);
            }
            

            if (gameRef.tryPurchaseVillage() != null)
            {
                yield return new PlayerPurchase(pdl.GetID(), pdl.position, "Village");
            }


            foreach (Node n in gameRef.tryPurchaseWall())
            {
                yield return new PlayerPurchase(pdl.GetID(), pdl.position, "Wall", n.position);
            }


            foreach (Node end in gameRef.attemptPlayerMove())
            {
                yield return new PlayerMove(pdl.GetID(), pdl.position, end.position);
            }
            
            yield return new PlayerMove(pdl.GetID(), pdl.position, pdl.position);
        }

        public static Stack<GameAction> Clone(Stack<GameAction> stack)
        {
            Contract.Requires(stack != null);
            return new Stack<GameAction>(new Stack<GameAction>(stack));
        }

    }


}
