using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    // Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool PlayerReady;

    private AnimationScript animation_script;
    private ThirdPersonController thid_person_controller;
    private PlayerInput player_input;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        player_input = GetComponent<PlayerInput>();
        player_input.enabled = true;
    }

    public override void OnStartAuthority()
    {
        Debug.Log("OnStartAuthority");
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer(this.gameObject);
        LobbyController.Instance.UpdateLobbyName();
        Debug.Log("//OnStartAuthority");
    }

    public override void OnStartClient()
    {
        Debug.Log("OnStartClient");
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Debug.Log("OnStopClient");
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    [Command]
    private void CMDSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.PlayerReady, !this.PlayerReady);
    }

    public void ChangeReady()
    {
        if (hasAuthority)
        {
            CMDSetPlayerReady();
        }
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer)
        {
            this.PlayerName = NewValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    private void PlayerReadyUpdate(bool OldValue, bool NewValue)
    {
        if (isServer)
        {
            this.PlayerReady = NewValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }
    
    public void CanStartGame(string SceneName)
    {
        if(hasAuthority)
        {
            CMDCanStartGame(SceneName);
        }
    }

    [Command]
    public void CMDCanStartGame(string SceneName)
    {
        Manager.StartGame(SceneName);
    }
}
