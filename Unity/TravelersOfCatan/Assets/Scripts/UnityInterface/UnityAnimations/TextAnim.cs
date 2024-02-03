using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Class to animate text in the UI
/// </summary>
public class TextAnim : MonoBehaviour
{
    public TextMeshProUGUI textObj;
    public bool muted = false;
    public float timeBtwnChars = 0.1f;
    public float timeBtwnWords = 0.125f;
    public float timeBtwnSentences = 0.15f;

    string endText;

    // OnEnable is called whenever the object is enabled
    public void OnEnable()
    {
        StartCoroutine(TextVisible());
    }

    // Method to reset the text object
    public void ResetTool()
    {
        textObj = GetComponent<TextMeshProUGUI>();
        endText = textObj.text;
    }

    // Method to add a letter-by-letter animation to the text object
    private IEnumerator TextVisible()
    {
        yield return 0;
        ResetTool();
        if (!muted)
            AudioManager.i.Play("Write");
        textObj.ForceMeshUpdate();
        int totalVisibleCharacters = endText.Length;
        int counter = 0;

        while (true)
        {
            textObj.maxVisibleCharacters = counter;

            if (counter >= totalVisibleCharacters)
            {
                break;
            }

            if (endText.ToCharArray()[counter] == ' ')
                yield return new WaitForSeconds(timeBtwnWords);
            else if (endText.ToCharArray()[counter] == '.')
                yield return new WaitForSeconds(timeBtwnSentences);
            else 
                yield return new WaitForSeconds(timeBtwnChars);
            counter += 1;

        }
        AudioManager.i.Stop("Write");
    }
}