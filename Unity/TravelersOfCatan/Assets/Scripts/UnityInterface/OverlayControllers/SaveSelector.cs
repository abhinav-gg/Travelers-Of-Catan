using NEAGame;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// <c>SaveSelector</c> is the class that manages the game save selection overlay. It is used to display the save game slots for the user to load or create.
/// </summary>
public class SaveSelector : MonoBehaviour
{

    public string SaveFileName;
    public SaveSlot[] SaveSlots;


    [Space(10)]
    [Header("Animation objs")]
    public GameObject panel;
    public GameObject cancelBtn;
    public GameObject SaveBtnParent;

    // Start is called before the first frame update
    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 800;
        // add listener for close button click
        cancelBtn.GetComponent<Button>().onClick.AddListener(() => {
            AudioManager.i.Play("UIClick");
            if (!LeanTween.isTweening(cancelBtn))
            {
                LeanTween.scale(cancelBtn, cancelBtn.transform.localScale, 0.1f).setEase(LeanTweenType.easeInCubic).setLoopPingPong(1);
            }
            CloseGUI();
        });
    }

    // method to close the save selector
    public void CloseGUI()
    {
        AudioManager.i.Play("UIClick");
        
        LeanTween.scale(cancelBtn, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInCubic);
        LeanTween.scale(SaveBtnParent, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.rotateAround(SaveBtnParent, Vector3.forward, 180, 0.25f).setEase(LeanTweenType.easeInCubic).setDelay(0.15f);
        LeanTween.scale(panel, Vector3.zero, 0.6f).setEase(LeanTweenType.easeInCubic).setOnComplete(() => { Destroy(gameObject); });
        
    }

    // method to setup the save selector
    public void Setup()
    {
        panel.SetActive(true);
        cancelBtn.SetActive(true);
        for (int i = 0; i < SaveSlots.Length; i++)
        {
            JSON_manager json = new JSON_manager(i);
            bool hasData = json.DoesGameExist();
            SaveSlot slot = SaveSlots[i];
            slot.ID = i;
            if (hasData)
            {
                // allows the user to load the save
                slot.StatusText.GetComponent<TextMeshProUGUI>().text = "Load Save";
                slot.SlotText.GetComponent<TextMeshProUGUI>().text = "Save " + (i + 1);
            }
            else
            {
                // allows the user to create a new save
                slot.StatusText.GetComponent<TextMeshProUGUI>().text = "Create New";
                slot.SlotText.GetComponent<TextMeshProUGUI>().text = "Save " + (i + 1);
                slot.Reset.SetActive(false);
            }
            slot.Background.GetComponent<Button>().onClick.RemoveAllListeners();
            slot.Reset.GetComponent<Button>().onClick.RemoveAllListeners();
            slot.Background.GetComponent<Button>().onClick.AddListener(() => BtnClick(slot.ID, hasData));
            slot.Reset.GetComponent<Button>().onClick.AddListener(() => ResetClick(slot.ID));
        }
    }

    // onclick for the reset button
    void ResetClick(int i)
    {
        AudioManager.i.Play("UIClick");
        JSON_manager json = new JSON_manager(i);
        json.ClearSave();
        Setup();
    }

    // onclick for the save slots
    void BtnClick(int i, bool hasData)
    {
        if (hasData)
        {
            UnityUI.Interface.SelectGameToLoad(i);
        }
        else
        {
            UnityUI.Interface.CreateNewGame(i);
        }
        CloseGUI();
    }
}
