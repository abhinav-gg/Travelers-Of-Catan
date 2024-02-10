using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// <c>PlayerUIOverlay</c> is the class that manages the overlay UI for the player. This includes the buttons for game actions and pausing along with elements to display the player's name, color, score, and time left.
/// </summary>
public class PlayerUIOverlay : MonoBehaviour
{
    public bool isAI = false;
    public bool isZoomed = true;
    private float animationTimer;
    private float zoomCD = 0.0f;
    private float moveCD = 0.0f;
    private float turnEndCD = 5f;
    private bool isTryingToMove = false;
    private Sprite buffer;

    [Header("UI Elements")]
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
    public Sprite CancelImage;

    // Start is called before the first frame update
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

    // called by the AI to set the UI to AI mode. this disables all buttons except for zooming and pausing
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

    // method to handle a click on the move button
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

    // method to handle a click on the end turn button
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

    // method to handle finishing or cancelling a move
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

    // method to alter the state of all game buttons (used to disable all buttons when a player is moving etc.)
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
            ZoomInput.gameObject.SetActive(state);
        }
    }

    // method to handle the zoom button click
    public void ZoomButton()
    {
        AudioManager.i.Play("UIClick");
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

    // method to smoothly lerp the camera zoom
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
    }

    // method to handle the inventory button click
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

    // method to handle the trade button click
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
