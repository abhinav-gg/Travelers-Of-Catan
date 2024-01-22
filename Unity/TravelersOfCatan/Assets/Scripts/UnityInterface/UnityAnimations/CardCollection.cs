using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Class to control a card for the collection and trading of resources
/// <br/> Used for animation only
/// </summary>
public class CardCollection : MonoBehaviour
{

    public List<Sprite> cards = new List<Sprite>();
   
    // Start is called before the first frame update


    public void SetCard(int resource, Vector3 Dest)
    {
        GetComponent<SpriteRenderer>().sprite = cards[resource];

        float delay = Random.Range(0.5f, 1f);
        float duration = Random.Range(0.7f, 1.1f);
        LeanTween.move(gameObject, Dest, duration).setEase(LeanTweenType.easeOutBounce).setDelay(delay).setOnComplete(() =>
        {
            AudioManager.i.Play("Ding");
            LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBounce).setDelay(0.15f).setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        });


    }


}
