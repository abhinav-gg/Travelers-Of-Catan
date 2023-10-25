using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;

namespace App
{

    /*public class UnityUIManager : UI
    {
        public int GetUserLetterInput(int options)
        {
            return 1;
        }

        public int GetUserChoice(object[] options)
        {
            return 1;
        }

        public string GetUserNameInput(int who)
        {
            return "A";
        }

        public bool GetUserConfirm()
        {
            return true;
        }

        public void CreatePopup(string message)
        {
            Debug.Log(message);
        }
    }
    */
    
    public class UnityUI : MonoBehaviour
    {

        public TravelersOfCatan game;
        // Start is called before the first frame update
        void Start()
        {
            //game = new TravelersOfCatan(new UnityUIManager(), 2, 2);
            //game.startGame();
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
