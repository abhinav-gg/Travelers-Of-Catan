using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNameInp : MonoBehaviour
{


    public string FinalName;
    public bool IsBot = false;

    public Toggle toggle;   
    public TMP_InputField inputField;
    public Button button;

    [Space(1)]
    [Header("Tweening Object")]
    [Space(1)]

    public GameObject panel;
    public GameObject banner;
    public GameObject input;
    public GameObject botImage;
    public GameObject checkbox;
    public GameObject doneBox;
    int NameMaxLength = 10;

    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 1;

        panel.transform.localScale = new Vector3(0, 0, 0);
        Vector3 bannerOG = banner.transform.localScale;
        banner.transform.localScale = new Vector3(0, 0, 0);
        Vector3 inputOG = input.transform.localScale;
        input.transform.localScale = new Vector3(0, 0, 0);
        Vector3 doneOG = doneBox.transform.localScale;
        doneBox.transform.localScale = new Vector3(0, 0, 0);
        Vector3 botOG = botImage.transform.localScale;
        botImage.transform.localScale = new Vector3(0, 0, 0);
        Vector3 checkOG = checkbox.transform.localScale;
        checkbox.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(panel, new Vector3(1, 1, 1), 1f).setEaseOutBack();
        LeanTween.scale(banner, bannerOG, 1f).setEaseOutBack();
        LeanTween.rotateAround(banner, Vector3.forward, 360, 0.5f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(input, inputOG, 1f).setEaseOutBack().setDelay(0.1f);
        LeanTween.scale(botImage, botOG, 1f).setEaseOutBack().setDelay(0.2f);
        LeanTween.scale(checkbox, checkOG, 1f).setEaseOutBack().setDelay(0.3f);
        LeanTween.scale(doneBox, doneOG, 1f).setEaseOutBack().setDelay(0.4f);


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (button.interactable)
            {
                button.onClick.Invoke();

            }
        }
    }


    public void CloseGUI()
    {

        LeanTween.moveY(banner, banner.transform.position.y + 300f, 0.75f).setEaseInBack();
        LeanTween.scale(panel, new Vector3(), 1f).setEaseInBack().setOnComplete(() => { Destroy(gameObject); });
        LeanTween.scale(input, new Vector3(), 0.75f).setEaseInBack().setDelay(0.15f);
        LeanTween.scale(botImage, new Vector3(), 0.75f).setEaseInBack();
        LeanTween.scale(checkbox, new Vector3(), 0.75f).setEaseInBack();
        LeanTween.scale(doneBox, new Vector3(), 0.75f).setEaseInBack().setDelay(0.25f);
        LeanTween.rotateAround(doneBox, Vector3.forward, 360, 1f).setEase(LeanTweenType.easeInSine).setDelay(0.25f);
    }

    public void TextChange()
    {
        // Func called from Unity Input Field UI element
        string name = inputField.text;
        button.interactable = true;
        if (name.Length > NameMaxLength)
        {
            name = name.Substring(0, NameMaxLength);
            inputField.text = name;
        } 
        else if (name.Length < 3)
        {
            button.interactable = false;
        } 
        
        string output = "";
        
        foreach (char x in name.ToCharArray())
        {
            if (char.IsLetterOrDigit(x))
            {
                output += x;
            }
        }
        inputField.text = output;
        // sanitize input

    }

}
