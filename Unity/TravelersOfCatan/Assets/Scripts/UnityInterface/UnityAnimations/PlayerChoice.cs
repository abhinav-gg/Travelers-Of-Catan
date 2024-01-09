using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAGame;

public class PlayerChoice : MonoBehaviour
{
    public GameObject guardian;
    public GameObject PlayerSlotPrefab;

    List<Player> options;
    bool found = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Setup(List<Player> players)
    {
        options = players;
    }

    IEnumerator AddPlayers()
    {
        foreach (Player p in options)
        {
            GameObject go = Instantiate(PlayerSlotPrefab, transform);
            //go.GetComponent<>();
            // .playerName and .color
            yield return new WaitForSeconds(0.1f);
        }

    }

    void Selected(Player pl)
    {
        // call reference with this player!
    }

}
