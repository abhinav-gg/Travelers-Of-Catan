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



        public Vector4 Eval(TravelersOfCatan game)
        {


            return new Vector4(0, 0, 0, 0);
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
        }


        public Vector3 Dijkstra(Board board, Vector3 start, Vector3 end)
        {
            // player is this





            return new Vector3();
        }


    }
}
