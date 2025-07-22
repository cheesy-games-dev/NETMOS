using System.Collections.Generic;
using Mirror;
using UnityEngine.Events;

public abstract class SharableEntity : NetworkBehaviour
{
    public bool RequiresAuthority { get; protected set; } = false;
    public UnityEvent OnShare;
    public UnityEvent OnUnshare;
    public List<ConnectionMotive> RequestingConnections = new();

    [SyncVar]
    public bool IsShared;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (!NetworkServer.spawned.ContainsKey(netIdentity.netId)) NetworkServer.Spawn(gameObject);
        netIdentity.AssignClientAuthority(NetworkServer.localConnection);
    }

    [Command(requiresAuthority = false)]
    public void Share(NetworkConnectionToClient conn = null)
    {
        if (RequiresAuthority) return;
        ConnectionMotive connection = new(conn, true);
        RequestingConnections.Add(connection);
        if (IsShared)
            return;
        RequestingConnections.Remove(connection);
        AssignAuth(conn);
        IsShared = true;
        ShareEvent(true);
    }

    [Command]
    protected void Destroy(NetworkIdentity identity)
    {
        NetworkServer.Destroy(identity.gameObject);
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

    [Command(requiresAuthority = false)]
    public void Unshare(NetworkConnectionToClient conn = null)
    {
        if (RequiresAuthority) return;
        ConnectionMotive connection = new(conn, true);
        if (RequestingConnections.Contains(connection)) RequestingConnections.Remove(connection);
        if (!IsShared)
        {
            return;
        }
        IsShared = false;
        ShareEvent(isSharing: false);
        if (RequestingConnections[0].IsMotive)
        {
            AssignAuth(RequestingConnections[0].Connection);
        }
    }
}
public class ServerRpc : CommandAttribute
{
    public ServerRpc()
    {
        this.requiresAuthority = false;
    }
}
public struct ConnectionMotive
{
    public NetworkConnectionToClient Connection;
    public bool IsMotive;
    public ConnectionMotive(NetworkConnectionToClient connection, bool isMotive = true)
    {
        Connection = connection;
        IsMotive = isMotive;
    }
}