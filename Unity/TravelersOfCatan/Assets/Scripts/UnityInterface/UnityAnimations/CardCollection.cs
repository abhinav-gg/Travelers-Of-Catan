using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCollection : MonoBehaviour
{

    public List<Sprite> cards = new List<Sprite>();
    [SerializeField] public Transform destination;
    // Start is called before the first frame update
    void Start()
    {
        float delay = Random.Range(0.5f, 1f);
        float duration = Random.Range(0.7f, 1.1f);
        // tween card to move to destination, the original position is set by the instantiator
        LeanTween.move(gameObject, GameObject.FindGameObjectsWithTag("CardCollection").FirstOrDefault().transform, duration).setEase(LeanTweenType.easeOutBounce).setDelay(delay).setOnComplete(() =>
        {
            AudioManager.i.Play("Ding1");
            LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBounce).setDelay(0.15f).setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetCard(int resource)
    {
        GetComponent<SpriteRenderer>().sprite = cards[resource];
    }


}
