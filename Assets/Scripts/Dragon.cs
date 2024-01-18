using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Dragon : NetworkBehaviour
{
    NetworkVariable<int> lifePoints = new NetworkVariable<int>(10);
    private int armorClass;
    private int attackBonus;
    //external links
    GameObject Settings;
    InstantiateChars InstantiateChars;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            lifePoints.OnValueChanged += OnSomeValueChanged;
        }
    }
    private void OnSomeValueChanged(int previous, int current)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");

    }

    void Start()
    {
        armorClass = 5;
        attackBonus = 0;
        Settings = GameObject.FindGameObjectWithTag("Settings").gameObject;
        InstantiateChars=GameObject.FindGameObjectWithTag("Map").gameObject.GetComponent<InstantiateChars>();
    }

    public void setVals(int lP, int aP, int atP)
    {
        if (IsHost)
        {
            lifePoints.Value = lP;
            armorClass = aP;
            attackBonus = atP;
        }
    }

    public void setVals(int[] vals)
    {
        if (IsHost)
        {
            lifePoints.Value = vals[0];
            armorClass = vals[1];
            attackBonus = vals[2];
        }
    }

    public int[] getVals()
    {
        return new int[]{ lifePoints.Value, armorClass, attackBonus};
    }

    [ServerRpc(RequireOwnership =false)]
    public void attackServerRpc(int points)
    {
        lifePoints.Value = lifePoints.Value - points;
        if (lifePoints.Value < 0)
        {
            this.gameObject.GetComponent<NetworkObject>().Despawn(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void setPositionServerRpc(Vector3 pos)
    {
        Debug.Log("attempted to place Target");
        this.transform.position = pos;
    }

    private string GetDecription()
    {
        return ("LifeP: "+ lifePoints.Value.ToString()+" , Armor: "+ armorClass.ToString()+" , Attack+: "+ attackBonus.ToString());
    }

    private void Update()
    {
        if (Settings==null) { Settings = GameObject.FindGameObjectWithTag("Settings").gameObject; }

        if (((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began)) || Input.GetMouseButtonDown(0))
        {
            HandleUserInput();
        }
    }

    private void HandleUserInput()
    {
        Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(raycast, out raycastHit))
        {
            if (raycastHit.collider == this.gameObject.GetComponent<Collider>())
            {
                if (IsHost)
                {
                    if (InstantiateChars.setEditTarget(this.gameObject))
                        GameObject.FindGameObjectWithTag("Log").gameObject.GetComponent<ErrorLog>().setLogMsg(GetDecription());
                }
                else
                {
                    Settings.GetComponent<DwarfInit>().setEnemyTarget(this.gameObject);
                }
            }
        }
    }
}
