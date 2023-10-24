using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;

namespace App
{

    public class UnityInterfacer : UI 
    {
        
        public int GetUserLetterInput(int options)
        {
            //return 0;
            throw new System.NotImplementedException();
        }

        public int GetUserChoice(object[] options)
        {
            //return 0;
            throw new System.NotImplementedException();
        }

        public string GetUserNameInput(int who)
        {
            return "A";
            throw new System.NotImplementedException();
        }

        public bool GetUserConfirm()
        {
            return true;
            throw new System.NotImplementedException();
        }

        public void CreatePopup(string message)
        {
            Debug.Log(message);
        }
    }



    public class UnityUI : MonoBehaviour
    {

        public TravelersOfCatan game;
        // Start is called before the first frame update
        void Start()
        {
            game = new TravelersOfCatan(2, 2);
            game.startGame();
        }

        // Update is called once per frame
        void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.Return))
            {
                game.ShowBoard();
            }*/
        }
    }

}
