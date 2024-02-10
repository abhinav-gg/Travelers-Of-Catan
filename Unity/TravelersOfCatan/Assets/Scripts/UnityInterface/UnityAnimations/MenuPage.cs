using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>MenuPage</c> is the class that adds listeners to the buttons on the menu page.
/// </summary>
public class MenuPage : MonoBehaviour
{
    
    public Button about;
    public Button load;
    public Button close;
    public Button settings;

    // Start is called before the first frame update
    void Start()
    {
        // Add listeners and sound effects to the buttons on the menu page
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
}
