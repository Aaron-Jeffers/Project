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
        if(!IsOwner)
        {
            return;
        }
        if(!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        SpawnParticleServerRpc();

        Instantiate(particlePrefab, transform.position, transform.rotation);
    }
    
    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void SpawnParticleServerRpc()
    {
        SpawnParticleClientRpc();
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    private void SpawnParticleClientRpc()
    {
        if(!IsOwner)
        {
            Instantiate(particlePrefab, transform.position, transform.rotation);
        }       
    }
}
