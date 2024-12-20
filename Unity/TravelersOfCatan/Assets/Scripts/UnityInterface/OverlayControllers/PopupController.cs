using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// <c>PopupController</c> is the class that manages the overlay to display a message to the user.
/// </summary>
public class PopupController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject MainText;
    public GameObject CancelBtn;
    public GameObject TextBG;
    public GameObject Header;

    // Start is called before the first frame update
    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 900;
        CancelBtn.GetComponent<Button>().onClick.AddListener(CloseGUI);
        // do some animation for the popup to appear
        Vector3 init;
        init = TextBG.transform.localScale;
        TextBG.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(TextBG, init, 0.5f).setEase(LeanTweenType.easeOutSine).setDelay(0.15f);
        init = Header.transform.localScale;
        Header.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(Header, init, 0.5f).setEase(LeanTweenType.easeOutSine);
        init = CancelBtn.transform.localScale;
        CancelBtn.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(CancelBtn, init, 0.5f).setEase(LeanTweenType.easeOutSine);
        LeanTween.rotateAround(CancelBtn, Vector3.forward, 360f, 0.5f).setEase(LeanTweenType.easeOutSine);
        LeanTween.rotateAround(Header, Vector3.forward, 360f, 0.5f).setEase(LeanTweenType.easeOutSine);

    }

    // method to close the popup
    public void CloseGUI()
    {
        // do some tweening
        AudioManager.i.Play("UIClick");
        AudioManager.i.Stop("Write");
        LeanTween.scale(Header, new Vector3(0, 0, 0), 0.75f).setEase(LeanTweenType.easeInElastic);
        LeanTween.scale(CancelBtn, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInElastic);
        LeanTween.rotateAround(TextBG, Vector3.forward, 360f, 0.5f).setEase(LeanTweenType.easeInElastic);
        LeanTween.scale(TextBG, new Vector3(0, 0, 0), 0.75f).setEase(LeanTweenType.easeInElastic).setDelay(0.15f).setOnComplete(() => Destroy(gameObject));
    }

    // method to update the text of the popup
    public void Setup(string content)
    {
        MainText.GetComponent<TextMeshProUGUI>().text = content;
    }
}
