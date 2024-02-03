using UnityEngine;
using NEAGame;

/// <summary>
/// Class attached to the connection gameobject to rotate and position it
/// </summary>
public class ConnectionAnimator : MonoBehaviour
{

    public Connection connection;

    public GameObject Road;
    public GameObject Wall;

    // Use this for initialization
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

    // Method to update the display of the connection
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
                LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(() => { Destroy(gameObject); });
                break;
        }   

    }


}
