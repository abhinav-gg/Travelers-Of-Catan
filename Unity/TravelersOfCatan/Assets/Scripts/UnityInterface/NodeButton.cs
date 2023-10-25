using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;

public class NodeButton : MonoBehaviour
{
    public Vector3 NodePos;
    public Node node;
    public Button btn;
    public RawImage img;
    public Texture village;
    public Texture city;


    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<RawImage>();
        img.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EnableButton()
    {
        btn.interactable = true;
    }

    public void DisableButton()
    {
        btn.interactable = false;
    }



    public void OnClick()
    {
        Debug.Log("Clicked");
    }

    public Vector3 GetGlobalPos()
    {
        return transform.position;
    }



    public void SetVillage()
    {

        img.texture = village;
        img.enabled = true;
    }

    public void UpgradeVillage()
    {
        if (img.texture == village)
        {
            img.texture = city;
        }
        else
        {
            Debug.LogError("this should not be allowed");
        }
    }

}
