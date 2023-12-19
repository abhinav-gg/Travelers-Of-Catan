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
    [InspectorName("Village")]
    public Texture village;
    [InspectorName("City")]
    public Texture city;


    // Start is called before the first frame update
    void Awake()
    {
        img.enabled = false;
        btn.onClick.AddListener(OnClick);
        btn.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EnableButton()
    {
        btn.gameObject.SetActive(true);
        btn.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(btn.gameObject, new Vector3(1, 1, 1), 0.75f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.1f);
        btn.interactable = true;
    }

    public void DisableButton()
    {
        btn.interactable = false;
        btn.gameObject.SetActive(false);
    }



    public void OnClick()
    {
        UnityUI.Interface.OnNodeClick(this);
    }


    public void UpdateSettlement()
    {

        img.enabled = true;
        
        if (node.status.GetStatus() == "Village")
        {
            img.texture = village;
        }
        else if (node.status.GetStatus() == "City")
        {
            img.texture = city;
        }
        else
        {
            img.enabled = false;
        }
        var sc = img.transform.localScale;
        img.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(img.gameObject, sc, 0.75f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.5f);
    }


}
