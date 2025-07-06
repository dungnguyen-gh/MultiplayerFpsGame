using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Steamworks;

public class FpsNetworkManager : NetworkManager
{
    [SerializeField]
    private GameObject enterAddressPanel = null,
    landingPage = null, lobbyUI = null;
    [SerializeField] private TMP_InputField addressField = null;
    [SerializeField] private GameObject startGameButton = null;
    public List<PlayerScript> playersList = new List<PlayerScript>();

    [SerializeField] private GameObject PlayerGO = null;

    [SerializeField] private bool useSteam = true;
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    void Start()
    {
        if (!useSteam) { return; }
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        PlayerScript playerStartPrefab = conn.identity.GetComponent<PlayerScript>();
        playersList.Add(playerStartPrefab);

        if (playersList.Count == 2)
        {
            startGameButton.SetActive(true);
        }
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        PlayerScript playerStartPrefab = conn.identity.GetComponent<PlayerScript>();
        playersList.Remove(playerStartPrefab);
        startGameButton.SetActive(false);
    }
    public void HostLobby()
    {
        landingPage.SetActive(false);

        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2);
            return;
        }

        NetworkManager.singleton.StartHost();
    }
    public void JoinButton()
    {
        enterAddressPanel.SetActive(true);
        landingPage.SetActive(false);
    }
    public void JoinLobby()
    {
        NetworkManager.singleton.networkAddress = addressField.text;
        NetworkManager.singleton.StartClient();
    }
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        lobbyUI.SetActive(true);
        enterAddressPanel.SetActive(false);
        landingPage.SetActive(false);
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        landingPage.SetActive(true);
        lobbyUI.SetActive(false);
        enterAddressPanel.SetActive(false);
    }
    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
    public void StartGame()
    {
        ServerChangeScene("GameLevel");
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (SceneManager.GetActiveScene().name.StartsWith("GameLevel"))
        {
            foreach (PlayerScript player in playersList)
            {
                var connectionTC = player.connectionToClient;
                GameObject playerP = Instantiate(PlayerGO, GetStartPosition().transform.position, Quaternion.identity);
                NetworkServer.ReplacePlayerForConnection(connectionTC, playerP);
            }
        }
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            landingPage.SetActive(true);
            return;
        }

        NetworkManager.singleton.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostIP", SteamUser.GetSteamID().ToString());

    }
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; }
        string HostIP = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostIP");

        NetworkManager.singleton.networkAddress = HostIP;
        NetworkManager.singleton.StartClient();

        landingPage.SetActive(false);
    } 
}
