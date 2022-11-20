using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public TextMeshProUGUI LobbyNameText;
    public TextMeshProUGUI LobbyIDText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;

    public Button StartGameButton;
    public TextMeshProUGUI ReadyButtonText;

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

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    public void StartGame(string SceneName)
    {
        LocalPlayerController.CanStartGame(SceneName);
    }

    public void ReadyPlayer()
    {
        Debug.Log("ReadyPressed!!");
        LocalPlayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (LocalPlayerController.PlayerReady)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool AllReady = true;
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!player.PlayerReady)
            {
                AllReady = false;
                break;
            }
        }
        if (AllReady && Manager.GamePlayers.Count > 0)
        {
            if (LocalPlayerController.PlayerIdNumber == 1)
            { 
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void LeaveLobby(){
        Debug.Log("LeaveLobby pressed");
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        Manager.StopHost();
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated) { CreateHostPlayerItem(); } // Host
        if (PlayerListItems.Count < Manager.GamePlayers.Count) { CreateClientPlayerItem(); } // Client
        if (PlayerListItems.Count > Manager.GamePlayers.Count) { RemovePlayerItem(); } // Client
        if (PlayerListItems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem(); } // Client
    }

    public void FindLocalPlayer(GameObject localPlayer)
    {
        // LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        // LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
        LocalPlayerObject = localPlayer;
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.PlayerReady;
            NewPlayerItemScript.SetPlayerValues();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.PlayerReady;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.PlayerReady;
                    PlayerListItemScript.SetPlayerValues();
                    if(player == LocalPlayerController) // us
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerlistitem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerlistitem.ConnectionID))
            {
                playerListItemToRemove.Add(playerlistitem);
            }
        }

        if (playerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem tmp_remove in playerListItemToRemove)
            {
                if(tmp_remove != null)
                {
                    GameObject objectToRemove = tmp_remove.gameObject;
                    PlayerListItems.Remove(tmp_remove);
                    Destroy(objectToRemove);
                    objectToRemove = null;
                }
            }
        }
    }

}
