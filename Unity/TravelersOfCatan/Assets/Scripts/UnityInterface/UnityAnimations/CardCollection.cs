using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CardCollection : MonoBehaviour
{

    public List<Sprite> cards = new List<Sprite>();
   
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetCard(int resource, Vector3 Dest)
    {
        GetComponent<SpriteRenderer>().sprite = cards[resource];
        Vector3 destination;
        if (Dest == new Vector3(0, 0, 0))
        {
            destination = GameObject.FindGameObjectsWithTag("CardCollection").FirstOrDefault().transform.position;
        }
        else
        {
            destination = Dest;
        }

        float delay = Random.Range(0.5f, 1f);
        float duration = Random.Range(0.7f, 1.1f);
        LeanTween.move(gameObject, destination, duration).setEase(LeanTweenType.easeOutBounce).setDelay(delay).setOnComplete(() =>
        {
            AudioManager.i.Play("Ding1");
            LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBounce).setDelay(0.15f).setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        });


    }


}
