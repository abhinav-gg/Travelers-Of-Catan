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



        public static int Eval(TravelersOfCatan game)
        {


            return 0;
        }

        public void Minimax()
        {

            // abcd pruning
            // a = alpha
            // b = beta
            // c = delta
            // d = gamma
            // requires MaxDepth which will make this function a heuristic

            
            return;


            // find resources required and use that to find the closest position to go to
            // The ai can not strategically place walls but it should be able to place roads and settlements somehow...
        }
        

        public void Dijkstra(Board board, Vector3 start)
        {

            // Dijkstra's algorithm
            int min_d;
            Dictionary<Node, int> distance = new Dictionary<Node, int>();
            Dictionary<Node, Node> previous = new Dictionary<Node, Node>();
            Dictionary<Node, Node> output = new Dictionary<Node, Node>();
            Dictionary<Node, bool> visited = new Dictionary<Node, bool>();
            Node[] gameBoard = board.GetAllNodes();
            foreach( Node node in gameBoard)
            {
                distance.Add(node, int.MaxValue);
                previous.Add(node, null);
                visited.Add(node, false);
            }
            
            Node current = board.GetNode(start);
            visited[current] = true;
            distance[current] = 0;

            while (!Enumerable.All(visited, x => x.Value==true))
            {

                //foreach (Connection con in current.GetConnections())
                //{
                //    if (visited[con.end])
                //    {
                //        continue;
                //    } else
                //    {
                //        int alt = distance[current] + con.GetWalkingCost(this);
                //        if (alt < distance[con.end])
                //        {
                //            distance[con.end] = alt;
                //            previous[current] = current;
                //        }
                //    }
                //}
                min_d = int.MaxValue;
                foreach (KeyValuePair<Node, int> node in distance)
                {
                    if (node.Value < min_d && !visited[node.Key])
                    {
                        min_d = node.Value;
                        current = node.Key;
                    }
                }


            }

            foreach(KeyValuePair<Node, Node> entry in previous)
            {
                output.Add(entry.Value, entry.Key); // swap the key and value to get the previous node so it is easier to assemble a path.
            }

            //Use output and distances for determining positions to go with the AI
            
            //return output;
        }


    }
}
