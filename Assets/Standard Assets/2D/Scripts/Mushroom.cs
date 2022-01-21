using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class Mushroom : NetworkBehaviour
{
    //Network
    private NetworkVariableVector3 networkPosition = new NetworkVariableVector3();
    public Vector3[] positions;
    public ulong localClientId;
    private int positionNumber = 0;

    private void Start()
    {
        localClientId = NetworkManager.Singleton.LocalClientId;
    }

    public void UpdatePosition()
    {
        int nextPos = Random.Range(0, positions.Length - 1);        
        transform.position = positions[nextPos];
        CallToMoveMushroom(transform.position);
    }

    void CallToMoveMushroom(Vector3 newPos)
    {
        // try to get the local client object..return when unsuccessful
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out var networkedMushroom))
        {
            return;
        }
        // get the component we want to check
        if (!networkedMushroom.PlayerObject.TryGetComponent<Transform>(out var transformNetworkedMushroom))
        {
            return;
        }

        positions[positionNumber] = transformNetworkedMushroom.position;
        updateTransformServerRpc(positions[positionNumber]);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateTransformServerRpc(Vector3 newPosition)
    {
        networkPosition.Value = newPosition;
    }
}
