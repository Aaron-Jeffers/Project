using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    public Vector3 componentTransform;
    public ulong localClientID;
    private NetworkVariableVector3 networkClientScale = new NetworkVariableVector3();
    
    public override void NetworkStart()
    {
        localClientID = NetworkManager.Singleton.LocalClientId;
    }

    public void callTheRPCToFlip()
    {
        // try to get the local client object..return when unsuccessful
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientID, out var networkedClient))
        {
            return;
        }

        // get the component we want to check
        if (!networkedClient.PlayerObject.TryGetComponent<Transform>(out var transformNetworkClient))
        {
            return;
        }

        componentTransform = transformNetworkClient.localScale;
        AdjustClientScaleWhenFlippingServerRpc(componentTransform);
    }

    [ServerRpc]
    public void AdjustClientScaleWhenFlippingServerRpc(Vector3 localScalVector)
    {
        networkClientScale.Value = localScalVector;
    }

    private void OnEnable()
    {
        networkClientScale.OnValueChanged += OnClientScaleChange;
    }

    private void OnDisable()
    {
        networkClientScale.OnValueChanged -= OnClientScaleChange;
    }

    private void OnClientScaleChange(Vector3 oldScaleVector3, Vector3 newScaleVector3)
    {
        if(newScaleVector3 == new Vector3(0.0f,0.0f,0.0f))
        {
            newScaleVector3 = new Vector3(1.771533f, 1.771533f, 1.0f);
        }
        transform.localScale = newScaleVector3;
    }
}
