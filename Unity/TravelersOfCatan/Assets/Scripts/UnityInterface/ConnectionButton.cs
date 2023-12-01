using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;
using System;

public class ConnectionButton : MonoBehaviour
{

    public NodeButton[] nodes = new NodeButton[2];
    public Connection connection;

    public GameObject Road;
    public GameObject Wall;
    public GameObject ButtonObj;
    public Button btn;
    // Start is called before the first frame update
    void Start()
    {
        UpdateConnection();
        btn.onClick.AddListener(UnityUI.Interface.OnConnectionClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateConnection()
    {
        Vector3 n1 = nodes[0].transform.position;
        Vector3 n2 = nodes[1].transform.position;

        // Animate images based on the direction of this connection so that they are always facing the right way


        if (n1.x == n2.x)
        {
            if (n1.y > n2.y)
            {
                Road.transform.Rotate(0, 0, 180);
                ButtonObj.transform.Rotate(0, 0, 180);
            }
        }

        else
        {
            float gradient = (n1.y-n2.y) / (n1.x-n2.x);
            if (gradient > 0)
            {
                ButtonObj.transform.Rotate(0, 0, -60);
                Road.transform.Rotate(0, 0, -60);
            }
            else
            {
                Wall.GetComponent<SpriteRenderer>().flipX = true;
                ButtonObj.transform.Rotate(0, 0, 60);
                Road.transform.Rotate(0, 0, 60);
            }
        }


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

}
