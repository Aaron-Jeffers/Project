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
    public int theHealth;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private GameObject projectilePrefab;

    public override void NetworkStart()
    {
        localClientID = NetworkManager.Singleton.LocalClientId;
        theScore = 0;
    }

    //public void LaunchProjectile()
    //{
    //    SpawnProjectileOnTargetServerRpc(localClientID);
    //}

    //[ServerRpc(RequireOwnership = false)]
    //private void SpawnProjectileOnTargetServerRpc(ulong targertClientId)
    //{
    //    if (IsServer)
    //    {
    //        SpawnProjectileOnTargetClientRpc(targertClientId);
    //    }
    //}

    //[ClientRpc]
    //private void SpawnProjectileOnTargetClientRpc(ulong targetClientId)
    //{
    //    if (IsServer)
    //    {
    //        NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out var networkedClient);
    //        GameObject obj = Instantiate(projectilePrefab, networkedClient.PlayerObject.GetComponent<Transform>().position, networkedClient.PlayerObject.GetComponent<Transform>().rotation);       
    //    }
    //    else
    //    {
    //        if (targetClientId == OwnerClientId)
    //        {
    //            GameObject obj = Instantiate(particlePrefab, transform.position, transform.rotation);
    //        }
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionString = collision.gameObject.tag;
        if (collision.gameObject.CompareTag("Mushroom") || collision.gameObject.CompareTag("Enemy"))
        {
            //collision.gameObject.GetComponent<AudioSource>().Play();
            if (NetworkManager.Singleton.LocalClientId == OwnerClientId)
            {
                collisionServerRpc(OwnerClientId, collisionString, true);
            }
        }
        //if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    collision.gameObject.GetComponent<AudioSource>().Play();
        //}
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        string collisionString = collision.gameObject.tag;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (NetworkManager.Singleton.LocalClientId == OwnerClientId)
            {
                collisionServerRpc(OwnerClientId, collisionString, false);
            }
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        collision.gameObject.GetComponent<AudioSource>().Stop();
    //    }
    //}

    [ServerRpc(RequireOwnership = false)]
    public void collisionServerRpc(ulong clientId, string collisionString, bool play)
    {
        //Request the players to send all their scores 
        collisionClientRpc(clientId, collisionString, play);
    }

    [ClientRpc]
    private void collisionClientRpc(ulong targetClientId, string collision, bool play)
    {
        //get the TargetClientId, compare it to the owner id and if the same update the score
        if(targetClientId == OwnerClientId)
        {
            GameObject theClientObject = this.gameObject;

            switch (collision)
            {
                case "Mushroom":
                    GetComponent<NetworkPlayer>().theScore += 1;
                    GameObject.Find("MultiplayerManager").GetComponent<MultiplayerScore>().syncScoreToAllClientServerRpc(OwnerClientId, theScore);

                    if (play)
                    {
                        GameObject.FindGameObjectWithTag(collision).GetComponent<Mushroom>().UpdatePosition();
                        GameObject.FindGameObjectWithTag(collision).GetComponent<AudioSource>().Play();
                    }                   
                    break;
                case "Enemy":
                    GetComponent<NetworkPlayer>().theHealth -= 1;
                    GameObject.Find("MultiplayerManager").GetComponent<MultiplayerHealth>().syncHealthToAllClientServerRpc(OwnerClientId, theHealth);
                    if (play)
                    {
                        GameObject.FindGameObjectWithTag(collision).GetComponent<AudioSource>().Play();
                    }

                    if (IsServer)
                    {
                        NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out var networkedClient);
                        Instantiate(particlePrefab, networkedClient.PlayerObject.GetComponent<Transform>().position, networkedClient.PlayerObject.GetComponent<Transform>().rotation);
                    }
                    else
                    {
                        if (targetClientId == OwnerClientId)
                        {
                            Instantiate(particlePrefab, transform.position, transform.rotation);
                        }
                    }
                    break;
            }
        }

        //GameObject.Find("MultiplayerManager").GetComponent<MultiplayerScore>().syncScoreToAllClientServerRpc(OwnerClientId, theScore);
        //Debug.Log("the score of player " + targetClientId + " is " + GetComponent<NetworkPlayer>().theScore);
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
