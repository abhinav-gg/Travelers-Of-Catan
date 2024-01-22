using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerUIOverlay : MonoBehaviour
{


    public Button MoveInput;
    public Button ShopInput;
    public Button InventoryInput;
    public Button TradeInput;
    public Button EndTurnInput;
    public Button PauseInput;
    public Button ZoomInput;
    public Button UndoInput;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI PlayerScore;
    public TextMeshProUGUI PlayerMoves;
    public Image ColorMe;
    public bool isAI = false;

    public Sprite CancelImage;

    
    public bool isZoomed = true;
    float animationTimer;
    float zoomCD = 0.0f;
    float moveCD = 0.0f;
    float turnEndCD = 5f;
    bool isTryingToMove = false;
    //bool isTryingBuy = false; REMOVE SELECT FROM SHOP BUTTON
    Sprite buffer;

    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 200;
        ZoomInput.onClick.AddListener(ZoomButton);
        MoveInput.onClick.AddListener(MoveButton);
        EndTurnInput.onClick.AddListener(EndTurnButton);
        InventoryInput.onClick.AddListener(OnInventory);
        TradeInput.onClick.AddListener(OnTrade);
    }

    // Update is called once per frame
    void Update()
    {

        TimerText.text = UnityUI.Interface.GetTime();
        if (!isAI)
        {
            PlayerMoves.text = UnityUI.Interface.game.GetCurrentPlayer().getMovesLeft().ToString();
            PlayerScore.text = UnityUI.Interface.game.GetCurrentPlayer().getVictoryPoints().ToString();
            UndoInput.interactable = UnityUI.Interface.game.actions.Count > 0;
        }
        animationTimer += Time.deltaTime;
        zoomCD = Mathf.Clamp(zoomCD - Time.deltaTime, -5f, 5f);
        moveCD = Mathf.Clamp(moveCD - Time.deltaTime, -3f, 3f);
        turnEndCD = Mathf.Clamp(turnEndCD - Time.deltaTime, -3f, 3f);
        
    }



    public void SetAI()
    {
        isAI = true;
        Destroy(MoveInput.gameObject);
        InventoryInput.interactable = false;
        Destroy(EndTurnInput.gameObject);
        Destroy(ShopInput.gameObject);
        Destroy(UndoInput.gameObject);
        Destroy(TradeInput.gameObject);

    }


    public void MoveButton()
    {
        
        AudioManager.i.Play("UIClick");
        if (LeanTween.isTweening(MoveInput.gameObject))
        {
            LeanTween.scale(MoveInput.gameObject, MoveInput.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        if (moveCD > 0.0f)
        {
            return;
        }
        isTryingToMove = !isTryingToMove;
        if (isTryingToMove)
        {
            moveCD = 0.5f;
            UnityUI.Interface.OnPlayerMove();
            buffer = MoveInput.image.sprite;
            MoveInput.image.sprite = CancelImage;
            AlterAllGameButton(false);
            MoveInput.gameObject.SetActive(true);
            PlayerMoves.gameObject.SetActive(false);
        }
        else
        {

            FinishMove();
        }
    }


    public void EndTurnButton()
    {
        if (turnEndCD > 0.0f)
        {
            return;
        }
        turnEndCD = 5f;
        AudioManager.i.Play("UIClick");
        if (!EndTurnInput.gameObject.IsDestroyed())
        {
            LeanTween.moveLocalY(EndTurnInput.gameObject, 10, 0.5f).setEase(LeanTweenType.easeInBack);
            LeanTween.scale(EndTurnInput.gameObject, new Vector3(0f, 0f, 0f), 0.5f).setEase(LeanTweenType.easeInOutBounce).setOnComplete(() => {
                Destroy(FindObjectOfType<PlayerUIOverlay>().gameObject);

                UnityUI.Interface.EndTurn();

            });
        }
    }



    public void FinishMove()
    {
        if (!isAI)
        {
            if (buffer != null)
            {
                MoveInput.image.sprite = buffer;
                buffer = null;
            }
            UnityUI.Interface.StopAllPlayerCoroutines();

            AlterAllGameButton(true);
            PlayerMoves.gameObject.SetActive(true);
        }
    }
    


    public void AlterAllGameButton( bool state, bool changeZoom = false ) // only called by player movement and shopping
    {
        MoveInput.gameObject.SetActive(state);
        ShopInput.gameObject.SetActive(state);
        InventoryInput.interactable = state;
        TradeInput.gameObject.SetActive(state);
        EndTurnInput.gameObject.SetActive(state);
        UndoInput.gameObject.SetActive(state);
        if (changeZoom)
        {
            // Zooming is normally allowed when moving, but not when trading
            ZoomInput.gameObject.SetActive(state);
        }
    }

    public void ZoomButton()
    {
        AudioManager.i.Play("UIClick");
        // make button smaller and larger
        if (!LeanTween.isTweening(ZoomInput.gameObject))
        {
            LeanTween.scale(ZoomInput.gameObject, ZoomInput.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        
        
        if (zoomCD > 0.0f)
        {
            return;
        }
        float inDist = 150.0f;
        float outDist = 300.0f;
        isZoomed = !isZoomed;
        zoomCD = 0.85f;
        animationTimer = 0.0f;
        if (isZoomed)
        {
            StartCoroutine(ZoomLerp(inDist));
        }
        else
        {
            StartCoroutine(ZoomLerp(outDist));
        }

    }

    IEnumerator ZoomLerp(float endDist)
    {
        float totalTime = 0.5f;
        float startDist = Camera.main.orthographicSize;
        float t = 0.0f;
        while (animationTimer < totalTime)
        {
            t = animationTimer / totalTime;
            Camera.main.orthographicSize = Mathf.Lerp(startDist, endDist, t);
            yield return 0;
        }


        // make this more rigid
    }

    public void OnInventory()
    {
        SceneTransition.i.PlayAnimation();
        if (FindObjectOfType<InventoryPopup>() != null)
        {
            return;
        }
        AudioManager.i.Play("UIClick");
        LeanTween.scale(InventoryInput.gameObject, InventoryInput.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        UnityUI.Interface.OpenInventory();
    }

    public void OnTrade()
    {
        if (FindObjectOfType<TradingInterface>() != null) // might need to be changed to a bool
        {
            return;
        }
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(TradeInput.gameObject))
        {
            LeanTween.scale(TradeInput.gameObject, TradeInput.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }

        StartCoroutine(UnityUI.Interface.OpenTrade());
        
    }



}
