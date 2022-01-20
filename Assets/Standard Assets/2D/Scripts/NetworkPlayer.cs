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
    public int theScore;
    
    public override void NetworkStart()
    {
        localClientID = NetworkManager.Singleton.LocalClientId;
        theScore = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Mushroom"))
        {
            if (NetworkManager.Singleton.LocalClientId == OwnerClientId)
            {
                coinCollectedServerRpc(OwnerClientId);
            }
        }        
    }

    [ServerRpc(RequireOwnership = false)]
    public void coinCollectedServerRpc(ulong clientId)
    {
        //Request the players to send all their scores 
        coinCollectedClientRpc(clientId);
    }

    [ClientRpc]
    private void coinCollectedClientRpc(ulong targetClientId)
    {
        //get the TargetClientId, compare it to the owner id and if the same update the score
        if(targetClientId == OwnerClientId)
        {
            GameObject theClientObject = this.gameObject;

            GetComponent<NetworkPlayer>().theScore += 1;
        }

        GameObject.Find("MultiplayerManager").GetComponent<MultiplayerScore>().syncScoreToAllClientServerRpc(OwnerClientId, theScore);
        Debug.Log("the score of player " + targetClientId + " is " + GetComponent<NetworkPlayer>().theScore);
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
