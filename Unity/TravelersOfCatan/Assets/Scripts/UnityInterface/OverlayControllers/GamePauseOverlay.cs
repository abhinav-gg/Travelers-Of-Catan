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
    public GameObject Save;
    public GameObject MusicParent;
    [Space(10)]
    public Sprite MusicUnmuted;
    public Sprite MusicMuted;



    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 700; // VERY foreground
        Slider.onValueChanged.AddListener((float v) => volumeChange(v));
        Slider.value = AudioManager.i.VolumeModifier;
        Resume.onClick.AddListener(() =>
        {
            AudioManager.i.Play("UIClick");
            CloseGUI();

        });
        MuteBG.onClick.AddListener(() => 
        {
            AudioManager.i.ToggleMute(Background: true); // Mutes background music
            UpdateMusicBtn(true);

        });
      
        MuteSFX.onClick.AddListener(() =>
        {
            AudioManager.i.ToggleMute(Background: false); // Mutes sound effects
            UpdateMusicBtn(false);

        });
        
        if (AudioManager.i.isMuted(Background: true))
        {
            MuteBG.GetComponent<Image>().sprite = MusicMuted;
        }
        Save.GetComponent<Button>().onClick.AddListener(OnExit);

        UpdateMusicBtn(true); UpdateMusicBtn(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void volumeChange(float x)
    {
        AudioManager.i.ChangeMasterVolume(x);
        UpdateMusicBtn(true); UpdateMusicBtn(false);
    }

    void CloseGUI()
    {
        //lean tween everything away
        LeanTween.scale(Save, new Vector3(), 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.scale(MusicParent, new Vector3(), 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.scale(InfoBtn, new Vector3(), 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.scale(Display, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.rotateAround(Display, Vector3.forward, 360, 0.5f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.scale(PauseBtn, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.scale(Panel, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
        {

            UnityUI.Interface.TimerActive = true;
            Destroy(gameObject);

        });
    }

    // add public functions for the SETTINGS and PAUSE menu setups



    void UpdateMusicBtn(bool background)
    {
        Sprite img = AudioManager.i.isMuted(background) ? MusicMuted : MusicUnmuted;
        if (Slider.value == 0.0f)
        {
            img = MusicMuted;
        }
        if (background)
        {
            MuteBG.GetComponent<Image>().sprite = img;
        }
        else
        {
            MuteSFX.GetComponent<Image>().sprite = img; 
        }
    }


    void OnExit()
    {
        AudioManager.i.Play("UIClick");
        if (!LeanTween.isTweening(Save))
        {
            LeanTween.scale(Save, Save.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInCubic).setLoopPingPong(1);
        }
        UnityUI.Interface.CloseAllGameUIs();
        UnityUI.Interface.SaveAndExit();
        
    }


}
