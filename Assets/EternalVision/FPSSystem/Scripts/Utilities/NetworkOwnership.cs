using System.Collections;

[System.Serializable]
public struct NetworkOwnership
{
    public bool isOwner;
    public bool isClient;
    public bool isServer;
    public bool isHost;

    public void CreateNew(bool owner, bool client, bool server, bool host)
    {
        isOwner = owner;
        isClient = client;
        isServer = server;
        isHost = host;
    }
}
