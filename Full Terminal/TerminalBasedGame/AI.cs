using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App;
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



        public Vector3 GetMaclaurinDistance() 
        { 
            
            
            return new Vector3(0, 0, 0);
        
        
        }

        public Vector4 Eval(TravelersOfCatan game)
        {


            return new Vector4(0, 0, 0, 0);
        }

        public Vector4 Minimax()
        {
            return new Vector4();
        }





    }
}
