using NEAGame;
using UnityEngine;


/// <summary>
/// <c>PlayerAnimator</c> is the class that animates the player game object. This includes moving the player and bobbing the current player indicator.
/// </summary>
public class PlayerAnimator : MonoBehaviour
{

    public Player player;
    public bool isCurrentPlayer = false;
    public GameObject indicator;

    Vector3 indpos;

    // Start is called before the first frame update
    void Start()
    {
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
