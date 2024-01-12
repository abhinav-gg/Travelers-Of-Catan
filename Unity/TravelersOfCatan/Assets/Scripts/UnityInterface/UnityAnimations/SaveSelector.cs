using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSelector : MonoBehaviour
{

    public string SaveFileName;
    public GameObject[] SaveSlots;


    [Space(10)]
    [Header("Animation objs")]
    public GameObject panel;
    public GameObject cancelBtn;


    Action callback;

    // Start is called before the first frame update
    void Start()
    {
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


    public void Show(Action callback)
    {
        this.callback = callback;
        panel.SetActive(true);
        cancelBtn.SetActive(true);
        foreach (GameObject slot in SaveSlots)
        {
            slot.SetActive(true);
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
