using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPage : MonoBehaviour
{
    public Button start;
    // Start is called before the first frame update
    void Start()
    {
        start.onClick.AddListener(UnityUI.Interface.StartGameButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}