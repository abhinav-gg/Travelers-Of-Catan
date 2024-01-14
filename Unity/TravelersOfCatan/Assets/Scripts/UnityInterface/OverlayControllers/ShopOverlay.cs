using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NEAGame;

public class ShopOverlay : MonoBehaviour
{
    // Start is called before the first frame update
    int currentID = 0;

    string[] shoppingOrder = new string[] { "Road", "Village", "Wall", "City" };
    public Sprite[] shoopingImages = new Sprite[4];
    public GameObject[] texts = new GameObject[5];

    [Space(10)]
    [Header("Animation objects")]
    public Button purchase;
    public GameObject LeftBtn;
    public GameObject RightBtn;

    // brick, sheep, ore, wood, wheat

    private float Buffer = 0.0f;

    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 700;
        purchase.onClick.AddListener(() => OnPurchase());
        purchase.gameObject.GetComponent<Image>().sprite = shoopingImages[currentID];
        UpdateDisplayCounts();
        AudioManager.i.Play("Purchase");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnCarousel(false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnCarousel(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseGUI();
        }
    }

    private void FixedUpdate()
    {
        Buffer = Mathf.Max(0, Buffer-Time.deltaTime);

    }

    void OnPurchase()
    {
        UnityUI.Interface.AttemptPurchase(shoppingOrder[currentID]);
        CloseGUI();
    }

    public void CloseGUI()
    {

        LeanTween.scale(purchase.gameObject, new Vector3(0, 0, 0), 0.75f).setEase(LeanTweenType.easeOutCirc)
            .setOnComplete(() => Destroy(gameObject));
        
    }


    public void OnCarousel(bool right)
    {
        purchase.interactable = false;
        if (Buffer > 0)
        {
            return;
        }
        Buffer = 1f;
        AudioManager.i.Play("UIClick");
        if (right)
        {
            if (!LeanTween.isTweening(RightBtn))
            {
                LeanTween.scale(RightBtn, RightBtn.transform.localScale * 1.2f, 0.1f).setEase(LeanTweenType.easeOutCirc).setLoopPingPong(1);
            }
            currentID++;
            if (currentID > 3)
            {
                currentID = 0;
            }
        }
        else
        {
            if (!LeanTween.isTweening(LeftBtn))
            {
                LeanTween.scale(LeftBtn, LeftBtn.transform.localScale * 1.2f, 0.1f).setEase(LeanTweenType.easeOutCirc).setLoopPingPong(1);
            }
            currentID--;
            if (currentID < 0)
            {
                currentID = 3;
            }
        }

        Vector3 startpos = purchase.gameObject.transform.position;
        int dir = right ? 1 : -1;
        LeanTween.moveLocalX(purchase.gameObject, startpos.x + (dir*1000f), 0.5f).setDelay(0.1f).setEase(LeanTweenType.easeInCirc).setOnComplete(() =>
        {
            purchase.gameObject.transform.position = new Vector3(- (dir*2000), 0, 0) + startpos;
            purchase.gameObject.GetComponent<Image>().sprite = shoopingImages[currentID];
            UpdateDisplayCounts();
            LeanTween.move(purchase.gameObject, startpos, 0.5f).setEase(LeanTweenType.easeOutCirc);
        });

    }

    void UpdateDisplayCounts()
    {
        bool canBuy = true;
        foreach (var entry in UnityUI.Interface.game.GetDifference(shoppingOrder[currentID]))
        {

            int index = entry.Key.GetHashCode() - 1;
            if (entry.Value >= 0)
            {
                texts[index].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                texts[index].GetComponentInChildren<TextMeshProUGUI>().text
                    = TravelersOfCatan.GetCostOfUpgrade(shoppingOrder[currentID])[entry.Key].ToString();
            }
            else
            {
                canBuy = false;
                texts[index].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                texts[index].GetComponentInChildren<TextMeshProUGUI>().text = entry.Value.ToString().Trim('-');
            }
        }

        if (canBuy)
        {
            purchase.interactable = true;
        }
        else
        {
            purchase.interactable = false;
        }

    }

}
