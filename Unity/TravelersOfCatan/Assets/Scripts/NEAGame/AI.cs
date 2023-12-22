using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics.Contracts;

namespace NEAGame
{
    class AI : Player
    {
        Dictionary<Node, int> distance = new Dictionary<Node, int>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();
        public AI(int playerNumber, string name, Vector3 home) : base(playerNumber, name, home)
        {
            isAI = true;
        }

        public AI(PlayerWrapper player) : base(player)
        {
            isAI = true;
        }



        public int StaticEval(TravelersOfCatan game)
        {
            int score = 0;


            foreach (Player pdl in game.gamePlayers)
            {
                if (pdl.GetID() == playerNumber)
                {
                    score += pdl.GetWealth();
                }
                else
                {
                    score -= pdl.GetWealth();
                }
            }


            return score;
        }

        public void BRS(TravelersOfCatan State, int Turn, int DepthLeft, int alpha, int beta)
        {
            if (DepthLeft <= 0)
            {
                //return StaticEval(State);
            }
            if (Turn == playerNumber)
            {

            }
            else
            {

            }



            /*foreach (Move m in AllMoves)
            {
                DoMove();
                int v = -BRS(State, Turn, DepthLeft - 1, MovesMade, -beta, -alpha);
                UndoMove();
                if (v >= beta)
                {
                    return v;
                }
                alpha = Math.Max(alpha, v);
            }*/

            // figure out how to do a move and then UNDO that move

            // First consider all purchases that can be made with the current resources
            // Then consider all possible moves that can be made with the current moves
            // Then move on to the next position
            
            
            return;


            // find resources required and use that to find the closest position to go to
            // The ai can not strategically place walls but it should be able to place roads and settlements somehow...
        }

        public void GenerateMoves()
        {

        }


        public void DoMove(Action act)
        {

        }

        public void UndoMove(Action act)
        {
            // remove from stack
            // if its a build move
            // if its a road or wall then update the board connection, remove from players list of connections and add victory points and resources back
            // if its a settlement then update the board node, remove from players list of nodes and add victory points and resources back
            // if its a player move
            // move the player back to the previous position


        }
        

        public void Dijkstra(Board board, Vector3 start)
        {

            // Dijkstra's algorithm
            distance = new Dictionary<Node, int>();
            previous = new Dictionary<Node, Node>();


            Node[] gameBoard = board.GetAllNodes();


            List<Node> Q = new List<Node>();

            foreach( Node node in gameBoard)
            {
                distance.Add(node, int.MaxValue);
                previous.Add(node, null);
                Q.Add(node);
            }
            
            Node current = board.GetNode(start);
            distance[current] = 0;

            while ( Q.Count > 0 ) 
            {

                // Linq to sort the list by distance and get first element (Priority Queue Implementation)
                current = Q.OrderBy(x => distance[x]).First();
                Q.Remove(current);

                foreach (var g in current.GetNodeNeighbours())
                {
                    Node neighbour = board.GetNode(g);
                    if (neighbour == null) continue;
                    Connection con = board.GetConnection(current.position, g);
                    int NewDist = distance[current] + con.GetWalkingCost(this);
                    if (NewDist < distance[neighbour])
                    {
                        distance[neighbour] = NewDist;
                        previous[neighbour] = current;
                    }
                }

            }
        }


    }


}
