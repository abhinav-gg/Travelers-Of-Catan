using System.Collections;
using System.Collections.Generic;
using NEAGame;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    public Player player;
    // Start is called before the first frame update
    public bool isCurrentPlayer = false;
    public GameObject indicator;

    Vector3 indpos;
    void Start()
    {
        // get local pos of indicator
        indpos = indicator.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCurrentPlayer && !LeanTween.isTweening(indicator))
        {
            // bob the indicator up and down using LeanTween
            LeanTween.moveLocalY(indicator, indpos.y - 0.15f, 1.5f).setEaseInOutSine().setLoopPingPong();
        }
        else if (!isCurrentPlayer)
        {
              // stop bobbing the indicator
            LeanTween.cancel(indicator);
            indicator.transform.localPosition = indpos;
        }
    }
}
