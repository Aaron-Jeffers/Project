using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class MultiplayerScore : NetworkBehaviour
{
    public GameObject panel;
    public Text text;

    public int restartCounter = 1;

    private NetworkVariableInt networkClientInt = new NetworkVariableInt(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    private NetworkVariableULong networkClientIDUlong = new NetworkVariableULong(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    private NetworkVariableBool networkClientIDChangedBool = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    public Dictionary<ulong, int> localPlayersScoreDictionary = new Dictionary<ulong, int>();
    public NetworkDictionary<ulong, int> playerScoresDictionary = new NetworkDictionary<ulong, int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    [ServerRpc(RequireOwnership = false)]
    public void syncScoreToAllClientServerRpc(ulong clientId, int clientScore)
    {
        if(networkClientIDUlong.Value != clientId)
        {
            networkClientIDUlong.Value = clientId;
            networkClientIDChangedBool.Value = true;
        }
        else
        {
            networkClientIDChangedBool.Value = false;
        }
        networkClientInt.Value = clientScore;
    }

    private void OnEnable()
    {
        networkClientInt.OnValueChanged += updateTheScoreClientRpc;
    }

    private void OnDisable()
    {
        networkClientInt.OnValueChanged -= updateTheScoreClientRpc;
    }

    [ClientRpc]
    private void updateTheScoreClientRpc(int oldScaleScore, int newScaleScore)
    {
        if (!playerScoresDictionary.ContainsKey(networkClientIDUlong.Value))
        {
            playerScoresDictionary[networkClientIDUlong.Value] = 0;
        }

        if (!localPlayersScoreDictionary.ContainsKey(networkClientIDUlong.Value))
        {
            localPlayersScoreDictionary[networkClientIDUlong.Value] = 0;
        }

        int highestScore = 0;
        if(newScaleScore > oldScaleScore)
        {
            highestScore = newScaleScore;
        }
        else
        {
            highestScore = oldScaleScore;
        }

        if (IsOwner)
        {
            if(networkClientIDChangedBool.Value == false)
            {
                localPlayersScoreDictionary[networkClientIDUlong.Value] = highestScore;
            }
            else
            {
                localPlayersScoreDictionary[networkClientIDUlong.Value] = newScaleScore;
            }

            if (localPlayersScoreDictionary[networkClientIDUlong.Value] != playerScoresDictionary[networkClientIDUlong.Value])
            {
                playerScoresDictionary[networkClientIDUlong.Value] = localPlayersScoreDictionary[networkClientIDUlong.Value];
            }
        }
    }

    private void OnGUI()
    {
        int x = 0;
        foreach(KeyValuePair<ulong,int> entry in playerScoresDictionary)
        {
            GUI.Label(new Rect(10, 60+ (15 * x), 300, 20), "PlayerID " + entry.Key + " has a score of " + entry.Value);
            x++;
            if (entry.Value >= 10 * restartCounter)
            {
                Time.timeScale = 0;
                panel.SetActive(true);
                text.text = "PlayerID " + entry.Key + " won the game with a score of: " + entry.Value;
            }
        }
    }

    public void Restart()
    {
        restartCounter++;
    }
}
