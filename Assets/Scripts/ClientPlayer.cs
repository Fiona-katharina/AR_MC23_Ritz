using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ClientPlayer : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> name = new NetworkVariable<FixedString32Bytes>("");
    private NetworkVariable<int> lifePoints = new NetworkVariable<int>(0);
    private NetworkVariable<int> armorClass = new NetworkVariable<int>(0);
    private NetworkVariable<int> attackBonus = new NetworkVariable<int>(0);
    NetworkVariable<bool> isClaimed = new NetworkVariable<bool>(false);
    private bool possible;
    //external references
    [SerializeField] TextMeshProUGUI nameTag;
    GameObject Settings;
    InstantiateChars Map;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            lifePoints.OnValueChanged += OnSomeValueChanged;
            armorClass.OnValueChanged += OnSomeValueChanged;
            attackBonus.OnValueChanged += OnSomeValueChanged;
            name.OnValueChanged += OnSomeValueChanged;
            possible = true;
        }
    }
    private void OnSomeValueChanged(int previous, int current)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
    }
    private void OnSomeValueChanged(FixedString32Bytes previous, FixedString32Bytes current)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
    }

    private void Start()
    {
        Settings = GameObject.FindGameObjectWithTag("Settings").gameObject;
        Map = GameObject.FindGameObjectWithTag("Map").gameObject.GetComponent<InstantiateChars>();
    }
    public void SetName(FixedString32Bytes name)
    {
        this.name.Value = name;
    }
    public void setVals(int lP, int aP, int atP)
    {
        if (IsHost)
        {
            lifePoints.Value = lP;
            armorClass.Value = aP;
            attackBonus.Value = atP;
            isClaimed.Value = true;
        }
        else setVals_ServerRpc(lP, aP, atP);
    }
    public void setVals(int lP, int aP, int atP, FixedString32Bytes name)
    {
        if (IsHost)
        {
            lifePoints.Value = lP;
            armorClass.Value = aP;
            attackBonus.Value = atP;
            isClaimed.Value = true;
            nameTag.text = (this.name.ToString());
        }
        else
        {
            setVals_ServerRpc(lP, aP, atP, name);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void setVals_ServerRpc(int lP, int aP, int atP, FixedString32Bytes name)
    {
        lifePoints.Value = lP;
        armorClass.Value = aP;
        attackBonus.Value = atP;
        isClaimed.Value = true;
        SetName(name);
    }

    [ServerRpc(RequireOwnership = false)]
    void setVals_ServerRpc(int lP, int aP, int atP)
    {
        lifePoints.Value = lP;
        armorClass.Value = aP;
        attackBonus.Value = atP;
        isClaimed.Value = true;
        string t = "Claimed";
        SetName(new FixedString32Bytes(t));
    }

    [ServerRpc(RequireOwnership = false)]
    public void setPositionServerRpc(Vector3 pos)
    {
        Debug.Log("attempted to place Target");
        this.transform.position = pos;
    }
    private void Update()
    {
        nameTag.text = this.name.Value.ToString();
        NetworkClient client = NetworkManager.Singleton.LocalClient;
        if (IsHost)
        {
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                HandleUserInput(Camera.main.ScreenPointToRay(Input.GetTouch(0).position));
            }
            if (Input.GetMouseButtonDown(0)) HandleUserInput(Camera.main.ScreenPointToRay(Input.mousePosition));
        }
        else if (possible)
        {
            //check if Player already claimed an object:
            if (client != null)
            {
                if (client.PlayerObject.GetComponent<Player>().HasClaimedPLayer())
                {
                    possible = false;
                }
            }

            //get touch input
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                HandleUserInput(Camera.main.ScreenPointToRay(Input.GetTouch(0).position));
            }
            if (Input.GetMouseButtonDown(0)) HandleUserInput(Camera.main.ScreenPointToRay(Input.mousePosition));
        }
    }
    private void HandleUserInput(Ray raycast)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(raycast, out raycastHit))
        {
            if (raycastHit.collider == this.gameObject.GetComponent<Collider>())
            {
                if (!IsHost)
                {
                    if (!isClaimed.Value)
                    {
                        Settings.GetComponent<DwarfInit>().setEditTarget(this.gameObject);
                    }
                    else GameObject.FindGameObjectWithTag("Log").GetComponent<TextMeshProUGUI>().text = "Character already occupied!";
                }
                else
                {
                   Map.setPlayerTarget(this.gameObject);
                }
            }
        }
    }
}
