using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAGame;
using UnityEngine.UI;
using TMPro;

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

    [Header("Animation")]
    public GameObject PanelText;
    
    Dictionary<int, int> MaxGain = new Dictionary<int, int>();
    Dictionary<int, int> MaxLoss = new Dictionary<int, int>();
    Dictionary<int, int> CurrentTrades = new Dictionary<int, int>();
    int currentResource = 1;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    //void Setup(int otherPlayer)

    public void CloseGUI()
    {
        // tween everything out of existance
        
        LeanTween.scale(PName2.transform.parent.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.scale(PName1.transform.parent.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.scale(accept.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.scale(cancel.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.moveLocalY(cancel.gameObject, -100, 0.2f).setEase(LeanTweenType.easeOutQuint);
        //LeanTween.rotateAround()
        LeanTween.moveLocalX(left.gameObject, -600f, 0.5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.moveLocalX(right.gameObject, 600f, 0.5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 2f).setEase(LeanTweenType.easeInOutElastic).setDelay(1f).setOnComplete(() => Destroy(gameObject));
    }


    public void Setup(Player current, Player other)
    {
        PName1.GetComponent<TextMeshProUGUI>().text = current.playerName;
        PName1.SetActive(true);
        PName2.GetComponent<TextMeshProUGUI>().text = other.playerName;
        PName2.SetActive(true);
        PCol1.GetComponent<Image>().color = UnityUI.textToColor(current.color.ToString());
        PCol2.GetComponent<Image>().color = UnityUI.textToColor(other.color.ToString());
        foreach (var entry in current.getResources())
        {
            CurrentTrades.Add(entry.Key.GetHashCode(), 0);
            MaxLoss.Add(entry.Key.GetHashCode(), entry.Value);
        }
        foreach (var entry in other.getResources())
        {
            MaxGain.Add(entry.Key.GetHashCode(), entry.Value);
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
            PTrade2.GetComponent<TextMeshProUGUI>().text = "+" + CurrentTrades[currentResource].ToString();
            PTrade2.GetComponent<TextMeshProUGUI>().text = CurrentTrades[currentResource].ToString();
        }
        else if (CurrentTrades[currentResource] < 0)
        {
            PTrade2.GetComponent<TextMeshProUGUI>().text = "+" + CurrentTrades[currentResource].ToString();
            PTrade2.GetComponent<TextMeshProUGUI>().text = CurrentTrades[currentResource].ToString();
        }
        else
        {
            PTrade1.GetComponent<TextMeshProUGUI>().text = "0";
            PTrade2.GetComponent<TextMeshProUGUI>().text = "0";
        }
        if (MaxLoss[currentResource] == 0)
        {
            decrease.interactable = false;
        }
        if (MaxGain[currentResource] == 0)
        {
            increase.interactable = false;
        }

    }


    // referenced by Unity Button
    void AddToCurrent()
    {
        decrease.interactable = true;  
        int nw = CurrentTrades[currentResource] + 1;

    }

    void GiveToEnemy()
    {
        increase.interactable = true;
        int nw = CurrentTrades[currentResource] - 1;

    }

    void Left()
    {
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(left.gameObject))
        {
            LeanTween.scale(left.gameObject, left.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        currentResource--; if (currentResource < 1) { currentResource = 5; }
        UpdateGUI();
    }
     void Right()
    {
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(right.gameObject))
        {
            LeanTween.scale(right.gameObject, right.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
        currentResource++; if (currentResource > 5) { currentResource = 1; }
        UpdateGUI();
    }

    void AcceptTrade()
    {
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(accept.gameObject))
        {
            LeanTween.scale(accept.gameObject, accept.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.0f).setLoopPingPong(1);
        }
    }


}
