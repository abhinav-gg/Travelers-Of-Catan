using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;
using System;

public class ConnectionAnimator : MonoBehaviour
{

    public Connection connection;

    public GameObject Road;
    public GameObject Wall;

    // Start is called before the first frame update


    public void UpdateConnection(Vector3 n1, Vector3 n2)
    {

        // Animate images based on the direction of this connection so that they are always facing the right way

        if (n1.y == n2.y && n1.z == n2.z)
        {
            
        }
        else
        {
            if (n1.x == n2.x && n1.y == n2.y)
            {
                Wall.GetComponent<SpriteRenderer>().flipX = true;
                Road.transform.Rotate(0, 0, 60);
            }
            else
            {
                Road.transform.Rotate(0, 0, -60);
            }
        }


    }


    public void UpdateDisplay()
    {
        switch (connection.GetStatus())
        {
            case "Road":
                Road.SetActive(true);
                Wall.SetActive(false);
                break;
            case "Wall":
                Road.SetActive(false);
                Wall.SetActive(true);
                break;
            default:
                Road.SetActive(false);
                Wall.SetActive(false);
                break;
        }   

        // add player color flag here in future


    }


}
