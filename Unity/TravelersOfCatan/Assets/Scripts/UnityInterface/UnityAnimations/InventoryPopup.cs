using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPopup : MonoBehaviour
{
    public GameObject Background;
    public Button CloseButton;
    public TextMeshProUGUI Brick;
    public TextMeshProUGUI Sheep;
    public TextMeshProUGUI Ore;
    public TextMeshProUGUI Wood;
    public TextMeshProUGUI Wheat;
    // Start is called before the first frame update
    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 500;
        CloseButton.onClick.AddListener(CloseGUI);

        Background.LeanScale(new Vector3(0, 0, 0), 0);
        CloseButton.gameObject.LeanScale(new Vector3(0, 0, 0), 0);
        LeanTween.scale(Background, new Vector3(7.710301f, 2.923337f, 2.923337f), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.7f);
        LeanTween.scale(CloseButton.gameObject, new Vector3(1, 1, 1), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.7f);
        }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseGUI()
    {
        LeanTween.scale(CloseButton.gameObject, new Vector3(0, 0, 0), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);

        LeanTween.moveLocalY(Brick.gameObject, -1000, 0.75f).setEase(LeanTweenType.easeOutSine).setDelay(0.2f);
        LeanTween.moveLocalY(Sheep.gameObject, -1000, 0.75f).setEase(LeanTweenType.easeOutSine).setDelay(0.3f);
        LeanTween.moveLocalY(Ore.gameObject, -1000, 0.75f).setEase(LeanTweenType.easeOutSine).setDelay(0.4f);
        LeanTween.moveLocalY(Wood.gameObject, -1000, 0.75f).setEase(LeanTweenType.easeOutSine).setDelay(0.5f);
        LeanTween.moveLocalY(Wheat.gameObject, -1000, 0.75f).setEase(LeanTweenType.easeOutSine).setDelay(0.6f);
        LeanTween.scale(Background, new Vector3(0, 0, 0), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.7f).setOnComplete(() => { 
            Destroy(gameObject);
        });
    }

    public void Display (string Name, int count)
    {
        Vector3 TextScale = new Vector3(0.1395292f, 0.53154f, 0.53154f);
        switch (Name)
        {
            case ("Brick"):
                Brick.gameObject.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(Brick.gameObject, TextScale, 0.75f).setEase(LeanTweenType.easeOutElastic).setDelay(0.1f);
                Brick.text = FormatInt(count);
                break;
            case ("Sheep"):
                Sheep.gameObject.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(Sheep.gameObject, TextScale, 0.75f).setEase(LeanTweenType.easeOutElastic).setDelay(0.1f);

                Sheep.text = FormatInt(count);
                break;
            case ("Ore"):
                Ore.gameObject.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(Ore.gameObject, TextScale, 0.75f).setEase(LeanTweenType.easeOutElastic).setDelay(0.1f);

                Ore.text = FormatInt(count);
                break;
            case ("Wood"):
                Wood.gameObject.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(Wood.gameObject, TextScale, 0.75f).setEase(LeanTweenType.easeOutElastic).setDelay(0.1f);

                Wood.text = FormatInt(count);
                break;
            case ("Wheat"):
                Wheat.gameObject.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(Wheat.gameObject, TextScale, 0.75f).setEase(LeanTweenType.easeOutElastic).setDelay(0.1f);

                Wheat.text = FormatInt(count);
                break;
        }
    }

    string FormatInt(int count)
    {
        // return string in form XX
        if (count < 10)
        {
            return "0" + count.ToString();
        }
        else
        {
            return count.ToString();
        }
    }


}
