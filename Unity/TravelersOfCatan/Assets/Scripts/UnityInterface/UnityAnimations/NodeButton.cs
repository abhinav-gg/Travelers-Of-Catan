using UnityEngine;
using UnityEngine.UI;
using NEAGame;

/// <summary>
/// Class that is attached to the node buttons in the game scene
/// </summary>
public class NodeButton : MonoBehaviour
{
    public Vector3 NodePos;
    public Node node;
    public GameObject flag;
    public Button btn;
    public SpriteRenderer img;
    [InspectorName("Village")]
    public Sprite village;
    [InspectorName("City")]
    public Sprite city;


    // Start is called before the first frame update
    void Awake()
    {
        img.enabled = false;
        btn.onClick.AddListener(OnClick);
        btn.gameObject.SetActive(false);
    }

    // method to enable the current node button
    public void EnableButton()
    {
        btn.gameObject.SetActive(true);
        btn.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(btn.gameObject, new Vector3(1, 1, 1), 0.75f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.1f);
        btn.interactable = true;
    }

    // method to disable the current node button
    public void DisableButton()
    {
        btn.interactable = false;
        btn.gameObject.SetActive(false);
    }

    // method to handle the click event on the node button
    public void OnClick()
    {
        UnityUI.Interface.OnNodeClick(this);
    }

    // method to update the settlement beneath the node button
    public void UpdateSettlement()
    {

        var sc = img.transform.localScale;
        img.enabled = true;
        flag.SetActive(true);
        if (node.status.GetStatus() == "Village")
        {
            flag.GetComponent<SpriteRenderer>().color = UnityUI.Interface.GetPlayerColor(node.status.GetOccupant());
            img.sprite = village;
        }
        else if (node.status.GetStatus() == "City")
        {
            flag.GetComponent<SpriteRenderer>().color = UnityUI.Interface.GetPlayerColor(node.status.GetOccupant());
            img.sprite = city;
        }
        else
        {
            LeanTween.scale(img.gameObject, new Vector3(0, 0, 0), 0.75f).setEase(LeanTweenType.easeInOutElastic).setOnComplete(() =>
            {
                flag.SetActive(false);
                img.enabled = false;
            });
        }
        img.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(img.gameObject, sc, 0.75f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.5f);
    }
}
