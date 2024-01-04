using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Interface { get; private set; }
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        if (Interface == null)
        {
            Interface = this;
        }
        else
        {
            Destroy(gameObject);
        }
        animator = GetComponent<Animator>();

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendToScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        animator.SetTrigger("Exit");
        yield return new WaitForSeconds(1.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }


}
