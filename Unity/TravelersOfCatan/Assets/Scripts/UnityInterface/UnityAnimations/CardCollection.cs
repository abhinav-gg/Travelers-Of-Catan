using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Animates a card on the screen for the collection and trading of resources
/// </summary>
public class CardCollection : MonoBehaviour
{

    public List<Sprite> cards = new List<Sprite>();
   

    // Method to set the card to a specific resource and move it to a location on screen as an animation for collection and trading
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
