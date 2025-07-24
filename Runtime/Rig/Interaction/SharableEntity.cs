using System.Collections.Generic;
using Mirror;
using UnityEngine.Events;

public abstract class SharableEntity : NetworkBehaviour
{
    public bool RequiresAuthority { get; protected set; } = false;
    public UnityEvent OnShare;
    public UnityEvent OnUnshare;

    [SyncVar]
    public bool IsShared;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (netIdentity.netId <= 0) NetworkServer.Spawn(netIdentity.gameObject);
        netIdentity.AssignClientAuthority(NetworkServer.localConnection);
    }

    [ServerRpc]
    public void Share(NetworkConnectionToClient conn = null)
    {
        if (IsShared || RequiresAuthority)
            return;
        AssignAuth(conn);
        IsShared = true;
        ShareEvent(true);
    }

    private void AssignAuth(NetworkConnectionToClient conn)
    {
        netIdentity.RemoveClientAuthority();
        netIdentity.AssignClientAuthority(conn);
    }

    private void ShareEvent(bool isSharing = true)
    {
        if (isSharing) OnShare?.Invoke();
        else OnUnshare?.Invoke();
    }

    [ServerRpc]
    public void Unshare(NetworkConnectionToClient conn = null)
    {
        if (RequiresAuthority || !IsShared) return;
        if (!IsShared)
        {
            return;
        }
        IsShared = false;
        ShareEvent(isSharing: false);
    }
}
public class ServerRpc : CommandAttribute
{
    public ServerRpc()
    {
        this.requiresAuthority = false;
    }
}