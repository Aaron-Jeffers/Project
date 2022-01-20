using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class ParticleSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject particlePrefab;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (IsServer)
            {
                SpawnParticleOnTargetServerRpc(NetworkManager.ConnectedClientsList[0].ClientId);
            }
            else
            {
                sendTargetFromClientServerRpc(0);
            }
        }
          
    }

    [ServerRpc(RequireOwnership = false)]
    private void sendTargetFromClientServerRpc(int targetId)
    {
        SpawnParticleOnTargetClientRpc(NetworkManager.ConnectedClientsList[targetId].ClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnParticleOnTargetServerRpc(ulong targertClientId)
    {
        if(IsServer)
        {
            SpawnParticleOnTargetClientRpc(targertClientId);
        }        
    }

    [ClientRpc]
    private void SpawnParticleOnTargetClientRpc(ulong targetClientId)
    {
        if(IsServer)
        {
            NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out var networkedClient);
            Instantiate(particlePrefab, networkedClient.PlayerObject.GetComponent<Transform>().position, networkedClient.PlayerObject.GetComponent<Transform>().rotation);
        }
        else
        {
            if(targetClientId == OwnerClientId)
            {
                Instantiate(particlePrefab, transform.position, transform.rotation);
            }
        }
    }

    //private void Update()
    //{
    //    if(!IsOwner)
    //    {
    //        return;
    //    }
    //    if(!Input.GetKeyDown(KeyCode.Space))
    //    {
    //        return;
    //    }

    //    SpawnParticleServerRpc();

    //    Instantiate(particlePrefab, transform.position, transform.rotation);
    //}

    //[ServerRpc(Delivery = RpcDelivery.Unreliable)]
    //private void SpawnParticleServerRpc()
    //{
    //    SpawnParticleClientRpc();
    //}

    //[ClientRpc(Delivery = RpcDelivery.Unreliable)]
    //private void SpawnParticleClientRpc()
    //{
    //    if(!IsOwner)
    //    {
    //        Instantiate(particlePrefab, transform.position, transform.rotation);
    //    }       
    //}
}
