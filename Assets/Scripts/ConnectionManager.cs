using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Services.Core;
using System;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using UnityEngine.Assertions;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] GameObject Dragon;
    [SerializeField] GameObject Player;
    [SerializeField] ErrorLog ErrorLog;
    [SerializeField] Button buttonHost;
    [SerializeField] Button buttonClient;
    [SerializeField] GameObject PlayerInit;
    [SerializeField] TextMeshProUGUI playerNumberInfo;
    [SerializeField] TextMeshProUGUI playerNumberInfo_player;
    [SerializeField] TextMeshProUGUI _Name;
    [SerializeField] TextMeshProUGUI _InfoLog;
    [SerializeField] TextMeshProUGUI HostBoundText;
    [SerializeField] TextMeshProUGUI JoinCodeInput;
    [SerializeField] TextMeshProUGUI CodeInfo;
    // GUI variables
    string joinCode = "n/a";
    // Allocation response objects
    Allocation hostAllocation;
    JoinAllocation playerAllocation;
    // Control variables
    bool isHost;
    bool isPlayer;

    // UTP variables
    NetworkDriver hostDriver;
    NetworkDriver playerDriver;
    NativeList<NetworkConnection> serverConnections;
    NetworkConnection clientConnection;
    private int maxConnections=5;

    async void Start()
    {
        // Initialize Unity Services
        await UnityServices.InitializeAsync();
    }

    public async void OnStartClientAsHost()
    {
        isHost = true;
        buttonHost.transform.parent.gameObject.SetActive(false);
        _Name.SetText("Dungeon Master");

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        //Setup Netcode:
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(hostAllocation, "udp"));
        joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Started Host for Netcode succesfully");
        }
        else Debug.Log("Failed to start Host!");

    }

    public async void OnStartClientAsPlayer()
    {
        if (JoinCodeInput.text.Length>6)
        {
            isPlayer = true;

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            playerAllocation = await RelayService.Instance.JoinAllocationAsync(JoinCodeInput.text.Substring(0, 6));

            // Extract the Relay server data from the Join Allocation response.
            var relayServerData = new RelayServerData(playerAllocation, "udp");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            if (NetworkManager.Singleton.StartClient())
            {
                buttonHost.transform.parent.gameObject.SetActive(false);
                PlayerInit.SetActive(true);
                Dragon.gameObject.SetActive(false);
                Player.gameObject.SetActive(false);
            }
        }
        else ErrorLog.setLogMsg("Please enter a valid join code!");
    }

    void Update()
    {
        if (isHost)
        {
            UpdateHost();
            UpdateHostUI();
        }
        else if (isPlayer)
        {
            UpdatePlayer();
            UpdatePlayerUI();
        }
    }
    void OnDestroy()
    {
        // Cleanup objects upon exit
        if (isHost)
        {
            serverConnections.Dispose();
        }
        else if (isPlayer)
        {
            //playerDriver.Dispose();
        }
    }

    void UpdateHostUI()
    {

        CodeInfo.text = joinCode;
        if(NetworkManager.Singleton.IsHost) playerNumberInfo.text ="Connected players: "+NetworkManager.Singleton.ConnectedClients.Count.ToString();
    }
    void UpdatePlayerUI()
    {
        _InfoLog.text = NetworkManager.Singleton.IsConnectedClient ? "Connected" : "connecting...";
    }

    /// Event handler for when the DisconnectPlayers (UTP) button is clicked.
    public void OnDisconnectPlayers()
    {
        if (serverConnections.Length == 0)
        {
            Debug.LogError("No players connected to disconnect.");
            return;
        }

        // In this sample, we will simply disconnect all connected clients.
        for (int i = 0; i < serverConnections.Length; i++)
        {
            // Here, we set the destination client's NetworkConnection to the default value. It will be recognized in the Host's Update loop as a stale connection, and be removed.
            serverConnections[i] = default(NetworkConnection);
        }
    }

    /// Event handler for when the client is disconnected
    public void OnDisconnect()
    {
        clientConnection = default(NetworkConnection);
    }

    void UpdateHost()
    {
        // Skip update logic if the Host is not yet bound.
        if (!hostDriver.IsCreated || !hostDriver.Bound)
        {
            return;
        }
        // This keeps the binding to the Relay server alive, preventing it from timing out due to inactivity.
        hostDriver.ScheduleUpdate().Complete();
        // Clean up stale connections.
        for (int i = 0; i < serverConnections.Length; i++)
        {
            if (!serverConnections[i].IsCreated)
            {
                Debug.Log("Stale connection removed");
                serverConnections.RemoveAt(i);
                --i;
            }
        }
        // Accept incoming client connections.
        NetworkConnection incomingConnection;
        while ((incomingConnection = hostDriver.Accept()) != default(NetworkConnection))
        {
            // Adds the requesting Player to the serverConnections list.This also sends a Connect event back the requesting Player, as a means of acknowledging acceptance.
            Debug.Log("Accepted an incoming connection.");
            serverConnections.Add(incomingConnection);
        }
    }

    void UpdatePlayer()
    {
        // Skip update logic if the Player is not yet bound.
        if (!playerDriver.IsCreated || !playerDriver.Bound)
        {
            return;
        }
        // This keeps the binding to the Relay server alive, preventing it from timing out due to inactivity.
        playerDriver.GetRelayConnectionStatus();
    }

}
