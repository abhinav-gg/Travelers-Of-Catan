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
        


    }


}
