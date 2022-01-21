using MLAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI.SceneManagement;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class MultiPlayerManager : NetworkBehaviour
{
    string sceneName;

    private void Start()
    {
        Time.timeScale = 1;
        sceneName = SceneManager.GetActiveScene().name;
    }
    public void RestartButton()
    {
        GetComponent<MultiplayerScore>().Restart();
        NetworkSceneManager.SwitchScene(sceneName);        
    }
}
