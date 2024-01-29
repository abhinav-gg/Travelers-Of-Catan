using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryManager : MonoBehaviour
{

    public GameObject MainText;
    public GameObject[] VictoryImages;
  
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
