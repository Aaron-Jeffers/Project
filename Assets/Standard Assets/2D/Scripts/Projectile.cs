using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class Projectile : NetworkBehaviour
{
    public float velocity = 10;

    //Network
    private NetworkVariableVector3 networkPosition = new NetworkVariableVector3();
    public Vector3 componentTransform;
    public ulong localClientId;

    private void Awake()
    {
       
    }
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
        
        Vector3 projectileVelocity = new Vector3 (velocity * Time.fixedDeltaTime, 0 ,0);

        Vector3 newPosition = transform.position;
        newPosition += projectileVelocity;
        transform.position = newPosition;

        CallToMoveProjectile(transform.position);
    }

    void CallToMoveProjectile(Vector3 newPos)
    {
        // try to get the local client object..return when unsuccessful
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out var networkedProjectile))
        {
            return;
        }
        // get the component we want to check
        if (!networkedProjectile.PlayerObject.TryGetComponent<Transform>(out var transformNetworkedProjectile))
        {
            return;
        }

        componentTransform = transformNetworkedProjectile.position;
        updateTransformServerRpc(componentTransform);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateTransformServerRpc(Vector3 newPosition)
    {
        networkPosition.Value = newPosition;
    }
}
