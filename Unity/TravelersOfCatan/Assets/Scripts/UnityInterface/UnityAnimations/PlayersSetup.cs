using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersSetup : MonoBehaviour
{
    public TMPro.TMP_Dropdown colorChoice;
    public GameObject bgcol;


    // Start is called before the first frame update
    void Start()
    {
        AlterColor(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    


    public void AlterColor(int choice)
    {
        // get string from dropdown menu and choice
        string color = colorChoice.options[choice].text;
        bgcol.GetComponent<Image>().color = UnityUI.textToColor(color);
    }


}
