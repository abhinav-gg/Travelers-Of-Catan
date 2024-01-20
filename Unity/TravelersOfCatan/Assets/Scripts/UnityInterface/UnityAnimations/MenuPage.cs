using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPage : MonoBehaviour
{
    
    public Button about;
    public Button load;
    public Button close;
    public Button settings;
    // Start is called before the first frame update
    void Start()
    {
        //start.onClick.AddListener(UnityUI.Interface.StartGameButton);
        about.onClick.AddListener(() => {
            AudioManager.i.Play("UIClick");
            UnityUI.Interface.AboutButton();
        });
        load.onClick.AddListener(() => {
            AudioManager.i.Play("UIClick");
            UnityUI.Interface.LoadGameButton();
        });
        close.onClick.AddListener(() => {
            AudioManager.i.Play("UIClick");
            UnityUI.Interface.QuitButton();
        });
        settings.onClick.AddListener(() =>
        {
            AudioManager.i.Play("UIClick");
            UnityUI.Interface.HomeScreenSettingsButton();
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
