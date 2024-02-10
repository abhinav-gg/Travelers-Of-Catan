using UnityEngine;
using TMPro;

/// <summary>
/// <c>VictoryManager</c> is the class that manages the victory scene.
/// </summary>
public class VictoryManager : MonoBehaviour
{

    public GameObject MainText;
    public GameObject[] VictoryImages;
  
    // Method to set up the victory screen
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
