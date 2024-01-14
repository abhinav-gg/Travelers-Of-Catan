using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{

    public GameObject Background;
    public GameObject Reset;
    public GameObject SlotText;
    public GameObject StatusText;
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 init;
        init = Background.transform.localScale;
        Background.transform.localScale = new Vector3();
        LeanTween.scale(Background, init, 0.5f).setEase(LeanTweenType.easeOutBack);
        init = Reset.transform.localScale;
        Reset.transform.localScale = new Vector3();
        LeanTween.scale(Reset, init, 0.25f).setEase(LeanTweenType.easeOutBack).setDelay(0.1f);
        init = SlotText.transform.localScale;
        SlotText.transform.localScale = new Vector3();
        LeanTween.scale(SlotText, init, 0.35f).setEase(LeanTweenType.easeOutBack).setDelay(0.1f);
        init = StatusText.transform.localScale;
        StatusText.transform.localScale = new Vector3();
        LeanTween.scale(StatusText, init, 0.35f).setEase(LeanTweenType.easeOutBack).setDelay(0.1f);
    }

}
