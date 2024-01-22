using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Class to animate text in the UI
/// <br/> Source: <seealso href="ZZZZZZ"/>
/// </summary>
public class TextAnim : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;
    [SerializeField] bool muted = false;
    [SerializeField] float timeBtwnChars = 0.1f;
    [SerializeField] float timeBtwnWords = 0.125f;
    [SerializeField] float timeBtwnSentences = 0.15f;

    string endText;

    public void OnEnable()
    {
        StartCoroutine(TextVisible());
    }

    public void ResetTool()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        endText = _textMeshPro.text;
    }


    private IEnumerator TextVisible()
    {
        yield return 0;
        ResetTool();
        if (!muted)
            AudioManager.i.Play("Write");
        _textMeshPro.ForceMeshUpdate();
        int totalVisibleCharacters = endText.Length;
        int counter = 0;

        while (true)
        {
            _textMeshPro.maxVisibleCharacters = counter;

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