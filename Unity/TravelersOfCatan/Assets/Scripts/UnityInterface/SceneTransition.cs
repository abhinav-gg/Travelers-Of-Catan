using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Class to control the scene transition animation
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimation()
    {
        StartCoroutine(AnimationSameScene());
    }

    IEnumerator AnimationSameScene()
    {
        GetComponentInChildren<Animator>().SetTrigger("Exit");
        yield return new WaitForSeconds(1.25f);
        GetComponentInChildren<Animator>().SetTrigger("Enter");
    }

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
