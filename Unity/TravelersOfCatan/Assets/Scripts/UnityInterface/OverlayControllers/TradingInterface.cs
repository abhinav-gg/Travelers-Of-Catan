using System.Collections.Generic;
using UnityEngine;
using NEAGame;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// <c>TradingInterface</c> is the class that manages the trading overlay. It is used to display each resource along with the quantity to trade.
/// </summary>
public class TradingInterface : MonoBehaviour
{
    public Button accept;
    public Button cancel;
    public Button left;
    public Button right;
    public Sprite[] Cards;

    [Header("Players")]
    public GameObject PName1;
    public GameObject PName2;
    public GameObject PCol1;
    public GameObject PCol2;
    public Button increase;
    public Button decrease;
    public GameObject PTrade1;
    public GameObject PTrade2;
    public GameObject Card;
    public GameObject POverallVal1;
    public GameObject POverallVal2;

    [Header("Animation")]
    public GameObject PanelText;
    
    //Dictionaries used to store the current trades and the max gain/loss for each resource so player's can't trade more than they have
    Dictionary<int, int> MaxGain = new Dictionary<int, int>();
    Dictionary<int, int> MaxLoss = new Dictionary<int, int>();
    Dictionary<int, int> CurrentTrades = new Dictionary<int, int>();
    
    int currentResource = 0;
    int overallVal = 0;
    Player other;

    // Start is called before the first frame update
    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 250;
        cancel.onClick.AddListener(() =>
        {
            AudioManager.i.Play("UIClick");
            CloseGUI();
        });
        accept.onClick.AddListener(AcceptTrade);
        accept.interactable = false;
        left.onClick.AddListener(Left);
        right.onClick.AddListener(Right);
        increase.onClick.AddListener(AddToCurrent);
        decrease.onClick.AddListener(GiveToEnemy);
        
    }

    public void CloseGUI()
    {
        // tween everything out of existance
        Destroy(POverallVal1);
        Destroy(POverallVal2);
        LeanTween.scale(PName2.transform.parent.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.scale(PName1.transform.parent.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.scale(accept.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.scale(cancel.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.moveLocalY(cancel.gameObject, -100, 0.2f).setEase(LeanTweenType.easeOutQuint);
        //LeanTween.rotateAround()
        LeanTween.moveLocalX(left.gameObject, -600f, 0.5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.moveLocalX(right.gameObject, 600f, 0.5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.alphaCanvas(PanelText.GetComponent<CanvasGroup>(), 0f, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(1f).setOnComplete(() => Destroy(gameObject));
    }

    public void Setup(Player current, Player other)
    {
        this.other = other;
        PName1.GetComponent<TextMeshProUGUI>().text = current.playerName;
        PName1.SetActive(true);
        PName2.GetComponent<TextMeshProUGUI>().text = other.playerName;
        PName2.SetActive(true);
        PCol1.GetComponent<Image>().color = UnityUI.textToColor(current.color.ToString());
        PCol2.GetComponent<Image>().color = UnityUI.textToColor(other.color.ToString());
        foreach (var entry in current.getResources())
        {
            CurrentTrades.Add(entry.Key.GetHashCode()-1, 0);
            MaxLoss.Add(entry.Key.GetHashCode()-1, entry.Value);
        }
        foreach (var entry in other.getResources())
        {
            MaxGain.Add(entry.Key.GetHashCode()-1, entry.Value);
        }

        // set Ptrade texts
        UpdateGUI();
        left.interactable = false;

    }

    void UpdateGUI()
    {
        // tween the card around
        Card.GetComponent<Image>().sprite = Cards[currentResource];
        if (CurrentTrades[currentResource] > 0)
        {
            PTrade1.GetComponent<TextMeshProUGUI>().text = "+" + CurrentTrades[currentResource].ToString();
            PTrade2.GetComponent<TextMeshProUGUI>().text = "-" + CurrentTrades[currentResource].ToString();
        }
        else if (CurrentTrades[currentResource] < 0)
        {
            PTrade2.GetComponent<TextMeshProUGUI>().text = "+" + Mathf.Abs(CurrentTrades[currentResource]).ToString();
            PTrade1.GetComponent<TextMeshProUGUI>().text = CurrentTrades[currentResource].ToString();
        }
        else
        {
            PTrade1.GetComponent<TextMeshProUGUI>().text = "0";
            PTrade2.GetComponent<TextMeshProUGUI>().text = "0";
        }

        if (overallVal > 0)
        {
            POverallVal1.GetComponent<TextMeshProUGUI>().text = "+" + overallVal.ToString();
            POverallVal1.GetComponent<TextMeshProUGUI>().color = Color.green;
            POverallVal2.GetComponent<TextMeshProUGUI>().text = "-" + overallVal.ToString();
            POverallVal2.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else if (overallVal < 0)
        {
            POverallVal2.GetComponent<TextMeshProUGUI>().text = "+" + Mathf.Abs(overallVal).ToString();
            POverallVal2.GetComponent<TextMeshProUGUI>().color = Color.green;
            POverallVal1.GetComponent<TextMeshProUGUI>().text = overallVal.ToString();
            POverallVal1.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            POverallVal1.GetComponent<TextMeshProUGUI>().text = "0";
            POverallVal1.GetComponent<TextMeshProUGUI>().color = Color.white;
            POverallVal2.GetComponent<TextMeshProUGUI>().text = "0";
            POverallVal2.GetComponent<TextMeshProUGUI>().color = Color.white;
        }


        
        decrease.interactable = (-CurrentTrades[currentResource] < MaxLoss[currentResource]);
        increase.interactable = (CurrentTrades[currentResource] < MaxGain[currentResource]);
        
        
        
    }


    // referenced by Unity Button
    void AddToCurrent()
    {
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(increase.gameObject))
        {
            LeanTween.scale(increase.gameObject, increase.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        overallVal++;
        CurrentTrades[currentResource] ++ ;
        UpdateGUI();

    }

    void GiveToEnemy()
    {
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(decrease.gameObject))
        {
            LeanTween.scale(decrease.gameObject, decrease.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        overallVal--;
        CurrentTrades[currentResource] -- ;
        UpdateGUI();
    }

    void Left()
    {
        accept.interactable = false;
        right.interactable = true;
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(left.gameObject))
        {
            LeanTween.scale(left.gameObject, left.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        currentResource--; if (currentResource == 0) { left.interactable = false; }
        UpdateGUI();
    }
     void Right()
    {
        left.interactable = true;
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(right.gameObject))
        {
            LeanTween.scale(right.gameObject, right.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        currentResource++; if (currentResource == 4) { right.interactable = false; accept.interactable = true; }
        UpdateGUI();
    }

    void AcceptTrade()
    {
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(accept.gameObject))
        {
            LeanTween.scale(accept.gameObject, accept.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        StartCoroutine(UnityUI.Interface.RegisterTrade(CurrentTrades, other));
        CloseGUI();
    }


}
