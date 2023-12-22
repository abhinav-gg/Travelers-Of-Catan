using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIOverlay : MonoBehaviour
{


    public Button MoveInput;
    public Button ShopInput;
    public Button InventoryInput;
    public Button TradeInput;
    public Button EndTurnInput;
    public Button PauseInput;
    public Button ZoomInput;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI PlayerScore;
    public TextMeshProUGUI PlayerMoves;

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

        // Moves these to main GUi class

        TimerText.text = UnityUI.Interface.GetTime();
        PlayerMoves.text = UnityUI.Interface.game.GetCurrentPlayer().getMovesLeft().ToString();
        PlayerScore.text = UnityUI.Interface.game.GetCurrentPlayer().getVictoryPoints().ToString();
    }







}
