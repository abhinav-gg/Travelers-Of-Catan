using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NEAGame;
using TMPro;
using System.Reflection;
using System.Dynamic;

public class PlayersSetup : MonoBehaviour
{
    public Button Remove;

    public PlayerSlot[] playerAdders;
    [SerializeField] List<PlayerTemplate> players = new List<PlayerTemplate>();
    [SerializeField] GameObject newPlayerGUI;
    [SerializeField] GameObject ColorSelector;

    public Button back;
    public Button cont;

    Button AddPlayer;
    PlayerNameInp overlay;
    ColorChoiceControl colorChoice;

    List<string> colors = new List<string>();
    float RemoveCD = 0.0f;
    bool isGetting;
    bool submittedName;
    bool gettingColor;
    int playerChangingColor;

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < 4; i++)
        {
            playerAdders[i].gameObject.SetActive(false);
        }
        

        cont.onClick.AddListener(Continue);
        cont.interactable = false;
        back.onClick.AddListener(Back);
        Remove.onClick.AddListener(RemovePlayer);
        SetupPlayerAdder();

        foreach (PlayerColors playerColors in Enum.GetValues(typeof(PlayerColors)))
        {
            colors.Add(playerColors.ToString());
        }


    }

    void SetupPlayerAdder()
    {
        if (players.Count == 4)
        {
            return;
        }
        playerAdders[players.Count].transform.localScale = new Vector3();
        playerAdders[players.Count].gameObject.SetActive(true);
        LeanTween.scale(playerAdders[players.Count].gameObject, new Vector3(1, 1, 1), 0.5f).setEase(LeanTweenType.easeInOutElastic).setDelay(0.5f);
        AddPlayer = playerAdders[players.Count].AddButton.GetComponent<Button>();
        AddPlayer.onClick.AddListener(PlayerAddButton);
        
        isGetting = false;
    }

    public void PlayerAddButton()
    {
        if (isGetting)
        {
            return;
        }
        isGetting = true;
        overlay = Instantiate(newPlayerGUI).GetComponent<PlayerNameInp>();
        overlay.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        overlay.gameObject.GetComponent<Canvas>().sortingLayerID = 2;
        overlay.GetComponent<Canvas>().sortingLayerName = "UI";
        overlay.gameObject.GetComponent<Canvas>().sortingOrder = 1000;
        overlay.button.onClick.AddListener(SaveNewPlayer);
        if (players.Count == 0)
        {
            overlay.toggle.interactable = false;
            // The first player is always a human
        }
        submittedName = false;
    }

    bool checkIfColorIsTaken(string color)
    {
        return players.Any(p => p.color == color);
    }

    string FirstAvailableColor()
    {
        foreach (string color in colors)
        {
            if (!checkIfColorIsTaken(color))
            {
                return color;
            }
        }
        return "clear"; // never happens as there are always more colors than players
    }

    void ColorChangeButton(int index)
    {
        if (gettingColor)
            return;
        gettingColor = true;
        colorChoice = Instantiate(ColorSelector).GetComponent<ColorChoiceControl>();
        // use linq to get a list of the color indexes that are taken already by other players
        List<int> indexes = players.Where(p => players.IndexOf(p) != index).Select(p => colors.IndexOf(p.color)).ToList();
        colorChoice.Setup(colors, indexes);
        colorChoice.SetCurrent(players[index].color);
        playerChangingColor = index;

    }

    public void NewColorSave(string choice)
    {
        gettingColor = false;
        players[playerChangingColor].color = choice;
        playerAdders[playerChangingColor].Color.GetComponent<Image>().color = UnityUI.textToColor(choice);
    }



    void updateContinue()
    {
        cont.interactable = players.Count > 1;
    }

    void SaveNewPlayer()
    {
        if (submittedName) return;
        submittedName = true;
        string name = overlay.inputField.text;
        bool isbot = overlay.toggle.isOn;

        overlay.CloseGUI();
        int index = players.Count;
        LeanTween.scale(playerAdders[players.Count].AddButton.gameObject, new Vector3(), 0.5f).setEase(LeanTweenType.easeInOutElastic).setOnComplete(() =>
        {
            Remove.interactable = true;
            playerAdders[index].PlayerName.GetComponent<TextMeshProUGUI>().text = name;
            playerAdders[index].PlayerName.GetComponent<TextAnim>().ResetTool();
            playerAdders[index].Parent.gameObject.SetActive(true);
            playerAdders[index].AddButton.SetActive(false);
            string col = FirstAvailableColor();
            playerAdders[index].Color.GetComponent<Image>().color = UnityUI.textToColor(col);
            playerAdders[index].Color.GetComponent<Button>().onClick.AddListener(() => { ColorChangeButton(index); });
            players.Add(new PlayerTemplate { name = name, ai = isbot, color = col.ToString() });
            updateContinue();
            SetupPlayerAdder();
        });    
    }


    private void Back()
    {
        UnityUI.Interface.GoHome();
    }

    void Update()
    {
        RemoveCD -= Time.deltaTime;
    }

    void RemovePlayer()
    {
        AudioManager.i.Play("UIClick");
        if (RemoveCD > 0.0f)
            return;
        RemoveCD = 0.8f;

        LeanTween.scale(Remove.gameObject, Remove.transform.localScale * 0.8f, 0.1f).setEase(LeanTweenType.easeOutCirc).setLoopPingPong(1);

        players.RemoveAt(players.Count-1);
        updateContinue();
        if (players.Count == 0)
        {
            Remove.interactable = false;
        }
        if (players.Count < 3)
        {
            LeanTween.scale(playerAdders[players.Count+1].gameObject, new Vector3(), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        }
        SetupPlayerAdder();
        LeanTween.rotateAround(playerAdders[players.Count].Parent, Vector3.forward, 360, 0.5f).setEase(LeanTweenType.easeInOutElastic).setOnComplete(() => {
            playerAdders[players.Count].Parent.SetActive(false);
            playerAdders[players.Count].AddButton.SetActive(true);
            LeanTween.scale(playerAdders[players.Count].AddButton, new Vector3(1, 1, 1), 0.5f).setEase(LeanTweenType.easeInOutElastic);
        });
        isGetting = false;
    }
    
    public void Continue()
    {
        UnityUI.Interface.game = new TravelersOfCatan(UnityUI.Interface, 15, 1, 910f);
        foreach (PlayerTemplate p in players) 
        { 
            if (p.ai)
            {
                UnityUI.Interface.game.AddAI(p.name, p.color);
            }
            else
            {
                UnityUI.Interface.game.AddPlayer(p.name, p.color);
            }
        }
        UnityUI.Interface.CommenceGame();
    }

    [Serializable]
    class PlayerTemplate
    {
        public string name;
        public string color;
        public bool ai;
    }
}


