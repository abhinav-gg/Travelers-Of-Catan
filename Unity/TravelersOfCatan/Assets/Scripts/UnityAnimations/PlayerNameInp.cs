using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNameInp : MonoBehaviour
{


    public string FinalName;
    public TMP_InputField inputField;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingLayerID = 2;
        GetComponent<Canvas>().sortingOrder = 1000;
        button.onClick.AddListener(NameSubmit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TextChange()
    {
        string name = inputField.text;
        if (name.Length > 0 && name.Length < 16)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    public void NameSubmit()
    {
        FinalName = inputField.text;

    }


}
