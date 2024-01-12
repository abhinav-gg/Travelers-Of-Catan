using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class TextAnim : MonoBehaviour // rewrite myshelf
{
    [SerializeField] TextMeshProUGUI _textMeshPro;

    public string[] stringArray;

    [SerializeField] bool muted = false;
    [SerializeField] float timeBtwnChars = 0.1f;
    [SerializeField] float timeBtwnWords = 0.5f;
    [SerializeField] float timeBtwnSentences = 0.75f;
    [SerializeField] bool loop = false;

    int i = 0;

    public void OnEnable()
    {
        i = 0;
        if (_textMeshPro == null)
        {
            ResetTool();
        }
        EndCheck();
    }

    public void ResetTool()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        stringArray = _textMeshPro.text.Split('\n');
    }

    public void EndCheck()
    {
        if (!loop && i > stringArray.Length - 1)
            return;
        else
        {
            i %= stringArray.Length;
            _textMeshPro.text = stringArray[i];
            StartCoroutine(TextVisible());

        }

    }


    private IEnumerator TextVisible()
    {
        if (!muted)
            AudioManager.i.Play("Write");
        _textMeshPro.ForceMeshUpdate();
        int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            _textMeshPro.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
            {
                i += 1;
                break;
            }

            if (stringArray[i].ToCharArray()[counter] == ' ')
                yield return new WaitForSeconds(timeBtwnWords);
            else
                yield return new WaitForSeconds(timeBtwnChars);
            counter += 1;


        }
        AudioManager.i.Stop("Write");
        yield return new WaitForSeconds(timeBtwnSentences);
        Invoke("EndCheck", timeBtwnWords);


    }
}