using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class Enemy : NetworkBehaviour
{
    public float maxCoordX;
    public float minCoordX;

    public float maxCoordY;
    public float minCoordY;

    public float horizontalVelocity;
    public float verticalVelocity;

    //Network
    private NetworkVariableVector3 networkPosition = new NetworkVariableVector3();
    public Vector3 componentTransform;
    public ulong localClientId;

    private void Start()
    {
        localClientId = NetworkManager.Singleton.LocalClientId;
    }

    private void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if ((transform.position.x >= maxCoordX) || transform.position.x <= minCoordX)
        {
            horizontalVelocity *= -1;
        }

        if ((transform.position.y >= maxCoordY) || transform.position.y <= minCoordY)
        {
            verticalVelocity *= -1;
        }
        Vector3 enemyVelocity = new Vector3(horizontalVelocity * Time.fixedDeltaTime, verticalVelocity  * Time.deltaTime, 0);

        Vector3 newPosition = transform.position;
        newPosition += enemyVelocity;
        transform.position = newPosition;

        CallToMovePlatform(transform.position);
    }

    void CallToMovePlatform(Vector3 newPos)
    {
        // try to get the local client object..return when unsuccessful
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out var networkedPlatform))
        {
            return;
        }
        // get the component we want to check
        if (!networkedPlatform.PlayerObject.TryGetComponent<Transform>(out var transformNetworkedPlatform))
        {
            return;
        }

        componentTransform = transformNetworkedPlatform.position;
        updateTransformServerRpc(componentTransform);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateTransformServerRpc(Vector3 newPosition)
    {
        networkPosition.Value = newPosition;
    }
}
