using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class MovingPlatform : NetworkBehaviour
{
    public float maxCoord;
    public float minCoord;

    public float velocity = 1;

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
        if ((transform.position.y >= maxCoord) || transform.position.y <= minCoord)
        {
            velocity *= -1;
        }
        Vector3 platformVelocity = new Vector3(0, velocity * Time.fixedDeltaTime, 0);

        Vector3 newPosition = transform.position;
        newPosition += platformVelocity;
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

    //private void OnEnable()
    //{
    //    networkPosition.OnValueChanged += OnClientPositionChange;
    //}

    //private void OnDisable()
    //{
    //    networkPosition.OnValueChanged += OnClientPositionChange;
    //}

    //private void OnClientPositionChange(Vector3 oldPos, Vector3 newPos)
    //{
    //    if (newPos == new Vector3(0.0f, 0.0f, 0.0f))
    //    {
    //        newPos = new Vector3(1.0f, 1.0f, 1.0f);
    //    }
    //    //transform.localScale = newPos;
    //}
}
