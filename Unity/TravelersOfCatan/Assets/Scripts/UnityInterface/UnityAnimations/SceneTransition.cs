using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// <c>SceneTransition</c> is a singleton class that handles the transition between scenes, including the cloud transition animation.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition i { get; private set; }
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(gameObject);
        }
        animator = GetComponent<Animator>();

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (scene, mode) => OnNewScene();
    }

    // public method to play the scene transition animation
    public void PlayAnimation()
    {
        StartCoroutine(AnimationSameScene());
    }

    // Coroutine to play the scene transition animation
    IEnumerator AnimationSameScene()
    {
        GetComponentInChildren<Animator>().SetTrigger("Exit");
        yield return new WaitForSeconds(1.25f);
        GetComponentInChildren<Animator>().SetTrigger("Enter");
    }

    // Method to set the canvas to the new scene
    void OnNewScene()
    {
        StartCoroutine(WaitForNewScene());
    }

    // Wait for the new scene to load before setting the canvas to the new scene
    IEnumerator WaitForNewScene()
    {
        yield return new WaitForEndOfFrame();
        Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.planeDistance = 100;
        myCanvas.worldCamera = Camera.main;
        myCanvas.sortingLayerName = "UI";
        myCanvas.sortingOrder = 12000;
        GetComponentInChildren<Animator>().SetTrigger("Enter");


    }

    // Send the player to the scene with the given name
    public void SendToScene(string sceneName)
    {
        if (sceneName == "Game")
        {
            StartCoroutine(LoadGameScene());
        }
        else
        {
            StartCoroutine(LoadScene(sceneName));
        }
    }

    // Coroutine to load the scene with the given name
    IEnumerator LoadScene(string sceneName)
    {
        GetComponentInChildren<Animator>().SetTrigger("Exit");
        yield return new WaitForSeconds(1f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Coroutine to load the game scene
    IEnumerator LoadGameScene()
    {
        AudioManager.i.Play("GetSetGo");
        GetComponentInChildren<Animator>().SetTrigger("Exit");
        yield return new WaitForSeconds(1.5f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
