using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NEAGame;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class to control the save selector GUI
/// </summary>
public class SaveSelector : MonoBehaviour
{

    public string SaveFileName;
    public SaveSlot[] SaveSlots;


    [Space(10)]
    [Header("Animation objs")]
    public GameObject panel;
    public GameObject cancelBtn;


    MethodInfo callback;

    // Start is called before the first frame update
    void Start()
    {


        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 800;

        cancelBtn.GetComponent<Button>().onClick.AddListener(() => {
            AudioManager.i.Play("UIClick");
            if (!LeanTween.isTweening(cancelBtn))
            {
                LeanTween.scale(cancelBtn, cancelBtn.transform.localScale, 0.1f).setEase(LeanTweenType.easeInCubic).setLoopPingPong(1);
            }
            CloseGUI();
        });
    }


    public void CloseGUI()
    {
        // Lean tween here
        Destroy(gameObject);
    }


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
                slot.StatusText.GetComponent<TextMeshProUGUI>().text = "Load Save";
                slot.SlotText.GetComponent<TextMeshProUGUI>().text = "Save " + (i + 1);
            }
            else
            {
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

    void ResetClick(int i)
    {
        AudioManager.i.Play("UIClick");
        JSON_manager json = new JSON_manager(i);
        json.ClearSave();
        Setup();
    }



    void BtnClick(int i, bool hasData)
    {
        AudioManager.i.Play("UIClick");
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
