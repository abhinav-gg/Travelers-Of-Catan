using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSelector : MonoBehaviour
{

    public string SaveFileName;
    public GameObject[] SaveSlots;


    [Space(10)]
    [Header("Animation objs")]
    public GameObject panel;
    public GameObject cancelBtn;


    Action callback;


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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
