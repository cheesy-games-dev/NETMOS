using Mirror;

namespace Netmos
{
    public class NetCmd : CommandAttribute
    {
    }

    public class NetClientRpc : ClientRpcAttribute
    {
    }

    /*
    public class NetVar<T> where T : SyncVar<T>
    {
        public T value;
    }
    */

    public class NetVar : SyncVarAttribute
    {

    }
}