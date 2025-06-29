using UnityEngine;
using Mirror;

public class NetworkStuff : NetworkBehaviour
{
    [SerializeField] GameObject FpsCamera = null, TpsMesh = null;
    void Start()
    {
        if (isLocalPlayer)
        {
            FpsCamera.SetActive(true);
            TpsMesh.SetActive(false);
        }
        else
        {
            FpsCamera.SetActive(false);
            TpsMesh.SetActive(true);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (!isLocalPlayer) return;
            LeaveGame();
        }
    }

    public void LeaveGame()
    {
        if (isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
