using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIOverlay : MonoBehaviour
{


    public Button MoveInput;
    public Button ShopInput;
    public Button TradeInput;
    public Button EndTurnInput;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI PlayerName;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingLayerID = 2;
        GetComponent<Canvas>().sortingOrder = 200;

    }

    // Update is called once per frame
    void Update()
    {
        TimerText.text = UnityUI.GameInterface.GetTime();
    }







}
