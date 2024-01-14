using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class TextAnim : MonoBehaviour // rewrite myshelf
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