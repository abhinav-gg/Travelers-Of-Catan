using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>GamePauseOverlay</c> is a class that manages the pause and settings overlays.
/// </summary>
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
    public GameObject Display;
    public GameObject Save;
    public GameObject MusicParent;
    [Space(10)]
    public Sprite MusicUnmuted;
    public Sprite MusicMuted;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.i.StopAll();
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 700; // VERY foreground
        Slider.onValueChanged.AddListener((float v) => volumeChange(v));
        Slider.value = AudioManager.i.VolumeModifier;
        // Add listeners to the buttons
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

    // The settings page is the pause page but with the save button removed
    public void RemoveSaveButton()
    {
        Save.SetActive(false);
    }

    // Method called when the volume slider is changed by Unity
    void volumeChange(float x)
    {
        AudioManager.i.ChangeMasterVolume(x);
        UpdateMusicBtn(true); UpdateMusicBtn(false);
    }

    // Method to close the pause overlay
    public void CloseGUI()
    {
        //lean tween everything away
        LeanTween.scale(Save, new Vector3(), 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.scale(MusicParent, new Vector3(), 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.scale(Display, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.rotateAround(Display, Vector3.forward, 360, 0.5f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.scale(PauseBtn, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setDelay(0.1f);
        LeanTween.scale(Panel, new Vector3(), 0.75f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
        {

            UnityUI.Interface.TimerActive = true;
            Destroy(gameObject);

        });
    }

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

    // onclick method for the save and exit button.
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
