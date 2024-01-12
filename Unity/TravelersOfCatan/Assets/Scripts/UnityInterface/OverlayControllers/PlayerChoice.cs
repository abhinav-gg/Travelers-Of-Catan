using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;
using TMPro;

public class PlayerChoice : MonoBehaviour
{
    public GameObject ParentObj;
    public GameObject CloseBtn;
    public GameObject PlayerSlotPrefab;

    List<Player> options;
    bool found = false;
    // Start is called before the first frame update

    public void CloseGUI()
    {

        Destroy(gameObject);
    }


    void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 250;
        CloseBtn.GetComponent<Button>().onClick.AddListener(CloseGUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Setup(List<Player> players)
    {
        options = players;
        StartCoroutine(AddPlayers());
    }

    IEnumerator AddPlayers()
    {
        foreach (Player p in options)
        {
            PlayerTradeSlot go = Instantiate(PlayerSlotPrefab, ParentObj.transform).GetComponent<PlayerTradeSlot>();
            go.PlayerColor.GetComponent<Image>().color = UnityUI.textToColor(p.color);
            go.PlayerName.GetComponent<TextMeshProUGUI>().text = p.playerName;
            go.ButtonObj.GetComponent<Button>().onClick.AddListener(() => Selected(p));
            yield return new WaitForSeconds(0.1f);
        }

    }

    void Selected(Player pl)
    {
        AudioManager.i.Play("UIClick");

        if (found)
            return;
        found = true;
        // call reference with this player!
        UnityUI.Interface.SelectPartner(pl);
        CloseGUI();
    }

}