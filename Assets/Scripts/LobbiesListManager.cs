using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;

    public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    public GameObject lobbiesButton;
    public GameObject hostButton;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake()
    {
        if(instance == null) { instance = this; }
    }

    public void DestroyLobbies()
    {
        foreach(GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        Debug.Log("DisplayLobbies " + lobbyIDs.Count.ToString());
        DestroyLobbies();
        for(int i = 0; i < lobbyIDs.Count; i++){
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);
                LobbyDataEntry createdItemLobbyEntry = createdItem.GetComponent<LobbyDataEntry>();
                CSteamID lobbyId = (CSteamID)lobbyIDs[i].m_SteamID;
                createdItemLobbyEntry.lobbyID = lobbyId;
                createdItemLobbyEntry.lobbyName = SteamMatchmaking.GetLobbyData(lobbyId, "name");
                createdItemLobbyEntry.SetLobbyData();

                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;

                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void GetListOfLobbies()
    {
        Debug.Log("GetListOfLobbies");
        lobbiesButton.SetActive(false);
        hostButton.SetActive(false);
        lobbiesMenu.SetActive(true);
    
        SteamLobby.Instance.GetLobbiesList();
    }

    public void GoBackFromLobbies()
    {
        DestroyLobbies();

        lobbiesMenu.SetActive(false);
        lobbiesButton.SetActive(true);
        hostButton.SetActive(true);
    }
}
