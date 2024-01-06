using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ColorChoiceControl : MonoBehaviour
{

    public GameObject[] colorChoiceBtns;
    public Sprite Taken;
    public Sprite NotTaken;

    [Header("Animation Objects")]
    public GameObject panel;
    public GameObject CloseBtn;
    public GameObject ColParent;

    List<string> col;
    string current = "clear";
    bool isClosing = false;

    // Start is called before the first frame update
    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 600;

        Vector3 init = panel.transform.localScale;
        panel.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(panel, init, 0.5f).setEase(LeanTweenType.easeInSine).setDelay(0.5f);
        init = CloseBtn.transform.localScale;
        CloseBtn.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(CloseBtn, init, 0.5f).setEase(LeanTweenType.easeInSine).setDelay(0.5f);
        CloseBtn.GetComponent<Button>().onClick.AddListener(CloseGUI);
        ColParent.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(ColParent.GetComponent<CanvasGroup>(), 1, 0.5f).setEase(LeanTweenType.easeInSine).setDelay(0.5f);
    }

    public void CloseGUI()
    {
        if (isClosing)
            return;
        isClosing = true;
        FindObjectOfType<PlayersSetup>().NewColorSave(current);
        AudioManager.i.Play("Click");
        // lean tween everything away
        LeanTween.moveLocalY(ColParent, -1000f, 0.5f).setEase(LeanTweenType.easeOutSine).setDelay(0.1f);
        LeanTween.rotateAround(CloseBtn , Vector3.forward, 360, 0.5f).setEase(LeanTweenType.easeShake).setDelay(0.0f);
        LeanTween.scale(CloseBtn , new Vector3(), 0.25f).setEase(LeanTweenType.easeShake).setDelay(0.25f);
        LeanTween.alphaCanvas(ColParent.GetComponent<CanvasGroup>(), 0, 0.75f).setEase(LeanTweenType.easeOutSine).setDelay(0.0f);
        LeanTween.scale(panel, new Vector3(), 0.75f).setEase(LeanTweenType.easeOutSine).setDelay(0.25f).setOnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void Setup(List<string> col, List<int> taken)
    {
        Assert.IsTrue(col.Count == colorChoiceBtns.Length);
        int i = 0;
        foreach (string c in col)
        {
            colorChoiceBtns[i].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SetCurrent(c));
            colorChoiceBtns[i].transform.GetChild(0).GetComponent<Image>().color = UnityUI.textToColor(c);
            if (taken.Contains(i))
            {
                colorChoiceBtns[i].GetComponent<Image>().sprite = Taken;
                colorChoiceBtns[i].transform.GetChild(0).GetComponent<Image>().sprite = Taken;
                colorChoiceBtns[i].transform.GetChild(0).GetComponent<Button>().interactable = false;
            }
            else
            {
                colorChoiceBtns[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void SetCurrent(string i)
    {
        AudioManager.i.Play("UIClick");
        if (current == i)
            return;
        Color search = UnityUI.textToColor(current);
        Color search2 = UnityUI.textToColor(i);
        foreach (var btn in colorChoiceBtns)
        {
            if (btn.transform.GetChild(0).GetComponent<Image>().color == search)
            {
                btn.transform.GetChild(0).GetComponent<Image>().sprite = NotTaken;
                btn.GetComponent<Image>().sprite = NotTaken;
            }
            else if (btn.transform.GetChild(0).GetComponent<Image>().color == search2)
            {
                btn.transform.GetChild(0).GetComponent<Image>().sprite = Taken;
                btn.GetComponent<Image>().sprite = Taken;
            }
        }
        current = i;
    }

}
