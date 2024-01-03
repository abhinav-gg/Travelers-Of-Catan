using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseOverlay : MonoBehaviour
{
    // Start is called before the first frame update
    public Button Resume;
    [SerializeField] Slider Slider;



    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingLayerID = 2;
        GetComponent<Canvas>().sortingOrder = 1700;
        Slider.onValueChanged.AddListener((float t) => AudioManager.i.ChangeMasterVolume(t));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CloseGUI()
    {
        //lean tween everything away


        UnityUI.Interface.TimerActive = true;
    }




}
