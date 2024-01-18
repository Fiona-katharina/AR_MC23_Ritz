using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Vuforia;

public class VB : MonoBehaviour
{
    [SerializeField] GameObject reference;
    private Transform Target;

    void Start()
    {
        Debug.Log("Button erkannt!");
        this.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonPressed(OnButtonPressed);
        this.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonReleased(OnButtonReleased);
        Target=this.transform.parent;
    }

    public void OnButtonPressed(VirtualButtonBehaviour v)
    {
        Debug.Log("Button 1 is Pressed");
    }

    public void OnButtonReleased(VirtualButtonBehaviour v)
    {
        Debug.Log("Button 1 is Released");
        placeDragon();
    }
    public void placeDragon()
    {
        reference.GetComponent<InstantiateChars>()._createDragon(Target.transform.position, Target.transform.localRotation);
    }
}
