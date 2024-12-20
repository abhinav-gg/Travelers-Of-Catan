using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>CardCollection</c> animates a card on the screen to indicate the collection or trading of a resource.
/// </summary>
public class CardCollection : MonoBehaviour
{

    public List<Sprite> cards = new List<Sprite>();
   
    // Method to set the card to a specific resource and move it to a location on screen as an animation for collection and trading
    public void SetCard(int resource, Vector3 Dest)
    {
        GetComponent<SpriteRenderer>().sprite = cards[resource];
        // Animate the card to its destination. This is either the inventory button or the trading partner
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
