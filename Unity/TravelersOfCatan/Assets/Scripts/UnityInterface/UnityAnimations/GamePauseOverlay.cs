using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseOverlay : MonoBehaviour
{
    // Start is called before the first frame update
    public Button Resume;
    [SerializeField] Slider Slider;
    [Space(10)]
    public Button MuteBG;
    public Button MuteSFX;
    [Space(10)]
    public GameObject Panel;
    public GameObject PauseBtn;
    public GameObject InfoBtn;
    public GameObject Display;

    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 500;
        Slider.onValueChanged.AddListener((float t) => AudioManager.i.ChangeMasterVolume(t));
        Slider.value = AudioManager.i.VolumeModifier;
        Resume.onClick.AddListener(() =>
        {
            AudioManager.i.Play("UIClick");
            CloseGUI();

        });
        MuteBG.onClick.AddListener(() => 
        {
            AudioManager.i.ToggleMute(Background: true); // Mutes background music

        });
        MuteSFX.onClick.AddListener(() =>
        {
            AudioManager.i.ToggleMute(Background: false); // Mutes sound effects

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CloseGUI()
    {
        //lean tween everything away
        LeanTween.scale(Display, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.rotateAround(Display, Vector3.forward, 360, 0.5f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.scale(InfoBtn, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.scale(PauseBtn, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.scale(Panel, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
        {

            UnityUI.Interface.TimerActive = true;
            Destroy(gameObject);

        });
    }

    // add public functions for the SETTINGS and PAUSE menu setups






    void OnSave()
    {

    }


}
