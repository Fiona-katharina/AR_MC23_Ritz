using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Vuforia;

public class VB1 : MonoBehaviour
{
    GameObject reference;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonPressed(OnButtonPressed);
        this.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonReleased(OnButtonReleased);
        player = this.transform.parent;
    }

    public void OnButtonPressed(VirtualButtonBehaviour v)
    {
        Debug.Log("Button 1 is Pressed");
    }

    public void OnButtonReleased(VirtualButtonBehaviour v)
    {
        Debug.Log("Button 1 is Released");
        fixChar();
    }

    public void fixChar()
    {
        reference.GetComponent<InstantiateChars>()._spawnPlayer(player.transform.position, player.transform.localRotation);
    }
    // Update is called once per frame
    void Update()
    {
        //needed to be done on update to get point in time that map is spawned
        if (reference == null) { 
            reference = GameObject.FindGameObjectWithTag("Map");
        }
    }
}
