using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Vuforia;
using Unity.Netcode;
using System;

public class InstantiateChars : NetworkBehaviour
{
    public GameObject DragonPrefab;
    public GameObject DwarfPrefab;
    private GameObject currentEdit;
    private GameObject currentPlayer;
    private GameObject DM;
    private ErrorLog errorLog;
    [SerializeField] GameObject markerEdit;
    [SerializeField] GameObject Dice;
    [SerializeField] DragonInit DragonEdit;

    void Start()
    {
        DM = GameObject.FindGameObjectWithTag("DM");
        errorLog= GameObject.FindGameObjectWithTag("Log").GetComponent<ErrorLog>();
    }
    //create an instance of the enemy mesh
    public void _createDragon(Vector3 pos, Quaternion rot)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if (currentEdit == null)
            {
                GameObject newDragon = GameObject.Instantiate(DragonPrefab, pos, rot, this.transform);
                int[] vals = DM.GetComponent<Dragon>().getVals();
                newDragon.GetComponent<NetworkObject>().Spawn();
                newDragon.GetComponent<Dragon>().setVals(vals);
                newDragon.transform.parent = this.transform;
            }
            else placeTarget();
        }
    }
    //move the player mesh hyrarchically under this object 
    public void _spawnPlayer(Vector3 pos, Quaternion rot)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if (currentPlayer == null)
            {
                GameObject player = GameObject.Instantiate(DwarfPrefab, pos, rot, this.transform);
                player.GetComponent<NetworkObject>().Spawn();
                player.transform.parent = this.transform;
            }
            else destroyPlayer();
        }
    }
    //possibility to expand:
    public void attackPlayer(GameObject obj)
    {
        obj.GetComponent<Player>().hurt();
    }
    public bool setEditTarget(GameObject obj)
    {
        if (currentEdit != null&&obj==currentEdit)
        {
            currentEdit = null;
            DragonEdit.setTarget(null);
            markerEdit.SetActive(false);
            return false;
        }
        else
        {
            currentEdit = obj;
            DragonEdit.setTarget(obj);
            markerEdit.SetActive(true);
            markerEdit.transform.parent = currentEdit.transform.parent;
            markerEdit.transform.position = obj.transform.position + new Vector3(0.02f, 0.055f, 0);
            return true;
        }
    }
    public bool setPlayerTarget(GameObject obj)
    {
        if (currentPlayer != null && obj == currentPlayer)
        {
            currentPlayer = null;
            markerEdit.SetActive(false);
            return false;
        }
        else
        {
            currentPlayer = obj;
            markerEdit.SetActive(true);
            markerEdit.transform.parent = currentPlayer.transform.parent;
            markerEdit.transform.position = obj.transform.position + new Vector3(0.005f, 0.055f, 0);
            return true;
        }
    }
    public void placeTarget()
    {
        if (currentEdit != null)
        {
            //Check if currentEdit is a Dragon object
            if (currentEdit.GetComponent<Dragon>() != null)
            {
                currentEdit.GetComponent<Dragon>().setVals(DM.GetComponent<Dragon>().getVals());
                Vector3 newpos = Dice.transform.position;
                currentEdit.GetComponent<Dragon>().setPositionServerRpc(newpos);
                markerEdit.transform.position = Dice.transform.position;
            }
            else errorLog.setLogMsg("Select a Dragon object first!");
        }
    }
    private void destroyPlayer()
    {
        //Make sure currentEdit is a Player object
        if (currentPlayer.GetComponent<ClientPlayer>() != null)
        {
            currentPlayer.GetComponent<NetworkObject>().Despawn(true);
            markerEdit.SetActive(false);
        }
        else errorLog.setLogMsg("Select a Player object first!");
    }
}
