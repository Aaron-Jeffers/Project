using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class MultiplayerHealth : NetworkBehaviour
{
    public GameObject panel;
    public Text text;

    public int restartCounter = 1;

    private NetworkVariableInt networkClientInt = new NetworkVariableInt(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    private NetworkVariableULong networkClientIDUlong = new NetworkVariableULong(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    private NetworkVariableBool networkClientIDChangedBool = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    public Dictionary<ulong, int> localPlayersHealthDictionary = new Dictionary<ulong, int>();
    public NetworkDictionary<ulong, int> playerHealthDictionary = new NetworkDictionary<ulong, int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    [ServerRpc(RequireOwnership = false)]
    public void syncHealthToAllClientServerRpc(ulong clientId, int clientHealth)
    {
        if (networkClientIDUlong.Value != clientId)
        {
            networkClientIDUlong.Value = clientId;
            networkClientIDChangedBool.Value = true;
        }
        else
        {
            networkClientIDChangedBool.Value = false;
        }
        networkClientInt.Value = clientHealth;
    }

    private void OnEnable()
    {
        networkClientInt.OnValueChanged += updateTheHealthClientRpc;
    }

    private void OnDisable()
    {
        networkClientInt.OnValueChanged -= updateTheHealthClientRpc;
    }

    [ClientRpc]
    private void updateTheHealthClientRpc(int oldScaleHealth, int newScaleHealth)
    {
        if (!playerHealthDictionary.ContainsKey(networkClientIDUlong.Value))
        {
            playerHealthDictionary[networkClientIDUlong.Value] = 0;
        }

        if (!localPlayersHealthDictionary.ContainsKey(networkClientIDUlong.Value))
        {
            localPlayersHealthDictionary[networkClientIDUlong.Value] = 0;
        }

        int highestHealth = 100;
        if (newScaleHealth < oldScaleHealth)
        {
            highestHealth = newScaleHealth;
        }
        else
        {
            highestHealth = oldScaleHealth;
        }

        if (IsOwner)
        {
            if (networkClientIDChangedBool.Value == false)
            {
                localPlayersHealthDictionary[networkClientIDUlong.Value] = highestHealth;
            }
            else
            {
                localPlayersHealthDictionary[networkClientIDUlong.Value] = newScaleHealth;
            }

            if (localPlayersHealthDictionary[networkClientIDUlong.Value] != playerHealthDictionary[networkClientIDUlong.Value])
            {
                playerHealthDictionary[networkClientIDUlong.Value] = localPlayersHealthDictionary[networkClientIDUlong.Value];
            }
        }
    }

    private void OnGUI()
    {
        int x = 0;
        foreach (KeyValuePair<ulong, int> entry in playerHealthDictionary)
        {
            GUI.Label(new Rect(500, 60 + (15 * x), 300, 20), "PlayerID " + entry.Key + " has " + entry.Value + " health");
            x++;
            if(entry.Value <= (-2 -(100 * restartCounter)))
            {
                Time.timeScale = 0;
                panel.SetActive(true);
                text.text = "PlayerID " + entry.Key + " lost the game with zero health";
            }
        }
    }

    public void Restart()
    {
        restartCounter += 1;
    }
}
