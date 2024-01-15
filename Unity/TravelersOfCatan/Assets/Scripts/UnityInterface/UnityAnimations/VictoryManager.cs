using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryManager : MonoBehaviour
{

    public GameObject MainText;
    public GameObject[] VictoryImages;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(string[] names)
    {
        string endText = "";
        int i = 0;
        foreach (string name in names)
        {
            VictoryImages[i].SetActive(true);
            endText += name + "\n";
            i++;
        }
        MainText.GetComponent<TextMeshProUGUI>().text = endText;
    }
}
