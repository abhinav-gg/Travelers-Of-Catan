using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;

public class NodeButton : MonoBehaviour
{

    public Node node;
    public Button btn;


    // Start is called before the first frame update
    void Start()
    {
        
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

    public Vector3 GetNodePos()
    {
        System.Numerics.Vector3 n = node.position;
        return new Vector3(n.X, n.Y, n.Z);
    }

}
