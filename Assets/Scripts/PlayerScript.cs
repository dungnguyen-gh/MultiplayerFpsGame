using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class PlayerScript : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleSteamIdUpdated))] private ulong SteamId;
    [SerializeField] private TMP_Text nameTxt = null;
    // public override void OnStartClient()
    // {
    //     DontDestroyOnLoad(gameObject);
    //     base.OnStartClient();
    // }
    public void SetSteamID(ulong SteamID)
    {
        this.SteamId = SteamID;   
    }
    private void HandleSteamIdUpdated(ulong OldSteamId, ulong NewSteamId)
    {
        var cSteamId = new CSteamID(NewSteamId);
        nameTxt.text = SteamFriends.GetFriendPersonaName(cSteamId);
    }
}
