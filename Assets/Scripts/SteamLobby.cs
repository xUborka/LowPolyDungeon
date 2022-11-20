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
    protected Callback<LobbyChatUpdate_t> LobbyChatUpdate;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEnter;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;
    
    public List<CSteamID> lobbyIDs = new List<CSteamID>();

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;

    public GameObject HostButton;
    public GameObject LobbiesButton;

    private void Start(){
        Debug.Log("Start");
        if (!SteamManager.Initialized){ return; }
        if (Instance == null) { Instance = this; }
        Debug.Log("Initialized");

        Debug.Log(SteamFriends.GetPersonaName().ToString());

        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void HostLobby(){
        Debug.Log("Host pressed");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback){
        if (callback.m_eResult != EResult.k_EResultOK){
            return;
        }
        Debug.Log("Lobby Created");
        manager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        // SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "game_type", "low_polly_stuff");
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback){
        
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback){
        Debug.Log("Request to join lobby");
        LobbiesButton.SetActive(false);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback){
        // Everyone
        HostButton.SetActive(false);
        LobbiesButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        // Client
        if (NetworkServer.active){ return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void GetLobbiesList()
    {
        if (lobbyIDs.Count > 0) { lobbyIDs.Clear(); }

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);
        // SteamMatchmaking.AddRequestLobbyListStringFilter("game_type", "low_polly_stuff", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbiesListManager.instance.listOfLobbies.Count > 0){LobbiesListManager.instance.DestroyLobbies(); }

        for(int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.instance.DisplayLobbies(lobbyIDs, result);
    }

}
