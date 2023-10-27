using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;

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
        btn.onClick.AddListener(UnityUI.GameInterface.ConnectionButtonPressed);
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
                Wall.transform.Rotate(0, 0, 180);
                ButtonObj.transform.Rotate(0, 0, 180);
            }
        }

        else
        {
            float gradient = (n1.y-n2.y) / (n1.x-n2.x);
            if (gradient > 0)
            {
                Wall.transform.Rotate(0, 0, -60);
                ButtonObj.transform.Rotate(0, 0, -60);
                Road.transform.Rotate(0, 0, -60);
            }
            else
            {
                Wall.transform.Rotate(0, 0, 60);
                ButtonObj.transform.Rotate(0, 0, 60);
                Road.transform.Rotate(0, 0, 60);
            }
        }


    }
}
