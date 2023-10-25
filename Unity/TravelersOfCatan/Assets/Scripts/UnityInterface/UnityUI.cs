using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;
using App;


public class UnityUI : MonoBehaviour//, UI // This is the tip of the Unity interface
{

    public System.Numerics.Vector3 ConvertVector(Vector3 vec)
    {
        return new System.Numerics.Vector3(vec.x, vec.y, vec.z);
    }

    public static Vector3 ConvertVector(System.Numerics.Vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.Z);
    }

    //public static Vector3 CubicToOddRow(System.Numerics.Vector3 vec)
    //{
    //    return new Vector3(,vec.X,0)
    //}


    public static UnityUI Interface { get; private set; }
    public TravelersOfCatan game;



    


    // Start is called before the first frame update
    void Awake()
    {
        if (Interface == null)
            Interface = this;
        
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            game.ShowBoard();
        }*/
    }


}
