using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEnter;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;

    public GameObject HostButton;
    public GameObject JoinButton;
    public TMP_InputField clientInputField;

    private void Start(){
        Debug.Log("Start");
        if (!SteamManager.Initialized){ return; }
        if (Instance == null) { Instance = this; }
        Debug.Log("Initialized");

        Debug.Log(SteamFriends.GetPersonaName().ToString());

        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby(){
        Debug.Log("Host pressed");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    public void JoinLobby(){
        Debug.Log("Join lobby");
        JoinButton.SetActive(false);
        clientInputField.gameObject.SetActive(false);
        SteamMatchmaking.JoinLobby(new CSteamID(ulong.Parse(clientInputField.text)));
    }

    private void OnLobbyCreated(LobbyCreated_t callback){
        if (callback.m_eResult != EResult.k_EResultOK){
            return;
        }
        Debug.Log("Lobby Created");
        manager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback){
        Debug.Log("Request to join lobby");
        JoinButton.SetActive(false);
        clientInputField.gameObject.SetActive(false);
        SteamMatchmaking.JoinLobby(new CSteamID(ulong.Parse(clientInputField.text)));
        // SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback){
        // Everyone
        HostButton.SetActive(false);
        JoinButton.SetActive(false);
        clientInputField.gameObject.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        // LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
        // lobbyidtext.text = CurrentLobbyID.ToString();

        // Client
        if (NetworkServer.active){ return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }
}
