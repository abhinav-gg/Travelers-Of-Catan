using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NEAGame;

/// <summary>
/// Class to control the shop overlay
/// </summary>
public class ShopOverlay : MonoBehaviour
{
    // Start is called before the first frame update
    int currentID = 0;

    string[] shoppingOrder = new string[] { "Road", "Village", "Wall", "City" };
    public Sprite[] shoopingImages = new Sprite[4];
    public GameObject[] texts = new GameObject[5];
    // same order as the resource class. for referece this is: brick, sheep, ore, wood, wheat

    [Space(10)]
    [Header("Animation objects")]
    public Button purchase;
    public GameObject LeftBtn;
    public GameObject RightBtn;

    private float Buffer = 0.0f;

    // Start is called before the first frame update
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
        if (purchase.interactable)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnCarousel(false);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnCarousel(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseGUI();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnPurchase();
        }
    }

    // FixedUpdate is called once per frame
    private void FixedUpdate()
    {
        Buffer = Mathf.Max(0, Buffer-Time.deltaTime);

    }

    // Attempt to purchase the item
    void OnPurchase()
    {
        UnityUI.Interface.AttemptPurchase(shoppingOrder[currentID]);
        CloseGUI();
    }


    // Close the GUI
    public void CloseGUI()
    {

        LeanTween.scale(purchase.gameObject, new Vector3(0, 0, 0), 0.75f).setEase(LeanTweenType.easeOutCirc)
            .setOnComplete(() => Destroy(gameObject));
        
    }

    // Moves to the next item in the carousel
    public void OnCarousel(bool right)
    {
        if (Buffer > 0)
        {
            return;
        }
        Buffer = 1.2f;
        purchase.interactable = false;
        AudioManager.i.Play("UIClick");
        GameObject btnToTween;
        if (right)
        {
            btnToTween = RightBtn;
            currentID++;
        }
        else
        {
            btnToTween = LeftBtn;
            currentID--;
        }
        if (!LeanTween.isTweening(btnToTween))
        {
            LeanTween.scale(btnToTween, btnToTween.transform.localScale * 1.2f, 0.1f).setEase(LeanTweenType.easeOutCirc).setLoopPingPong(1);
        }
        currentID %= 4; // wrap around the current ID

        Vector3 startpos = purchase.gameObject.transform.position;
        int dir = right ? 1 : -1;
        LeanTween.moveLocalX(purchase.gameObject, startpos.x + (dir*1000f), 0.5f).setDelay(0.1f).setEase(LeanTweenType.easeInCirc).setOnComplete(() =>
        {
            purchase.gameObject.transform.position = new Vector3(- (dir*2000), 0, 0) + startpos;
            purchase.gameObject.GetComponent<Image>().sprite = shoopingImages[currentID];
            UpdateDisplayCounts();
            LeanTween.move(purchase.gameObject, startpos, 0.5f).setEase(LeanTweenType.easeOutCirc);
        }); // animate the purchase image across the screen and update the costs.

    }

    // Update the display counts
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
