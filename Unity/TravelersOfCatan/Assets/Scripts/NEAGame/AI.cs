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

        public void BRS(TravelersOfCatan State, int Turn, int DepthLeft, Stack<Move> MovesMade)
        {
            if (DepthLeft == 0)
            {
                //return StaticEval(State);
            }


            // First consider all purchases that can be made with the current resources
            // Then consider all possible moves that can be made with the current moves
            // Then move on to the next position
            
            
            return;


            // find resources required and use that to find the closest position to go to
            // The ai can not strategically place walls but it should be able to place roads and settlements somehow...
        }
        

        public void Dijkstra(Board board, Vector3 start)
        {

            // Dijkstra's algorithm
            
            Dictionary<Node, int> distance = new Dictionary<Node, int>();
            Dictionary<Node, Node> previous = new Dictionary<Node, Node>();
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
                    //Connection con = board.GetConnection(current.position, g);
                    //if ()
                    //{
                    //    int alt = distance[current] + con.GetWalkingCost(this);
                    //    if (alt < distance[con.end])
                    //    {
                    //        distance[con.end] = alt;
                    //        previous[current] = current;
                    //    }
                    //}
                }

            }

            
            //Use output and distances for determining positions to go with the AI
            
            //return output;
        }


    }

    class Move
    {
        public Vector3 currentPosition;
        public Vector3 position;
        public string type;
        public string status;

        public Move(Vector3 current, Vector3 position)
        {
            this.currentPosition = current;
            this.type = "Move";
            this.position = position;
        }

        public Move(Vector3 current, Vector3 position, string status)
        {
            this.currentPosition = current;
            this.type = "Build";
            this.position = position;
            this.status = status;
        }


    }

}
