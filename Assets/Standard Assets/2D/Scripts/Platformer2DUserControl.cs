using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using MLAPI;

[RequireComponent(typeof (PlatformerCharacter2D))]
public class Platformer2DUserControl : NetworkBehaviour
{
    private PlatformerCharacter2D m_Character;
    private bool m_Jump;

    private void Awake()
    {
        //m_Character = GetComponent<PlatformerCharacter2D>();
    }




    private void FixedUpdate()
    {
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<PlatformerCharacter2D>();
            if (player)
            {
                // Read the inputs.
                bool crouch = Input.GetKey(KeyCode.LeftControl);
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                // Pass all parameters to the character control script.
                player.Move(h, crouch, m_Jump);
                m_Jump = false;
            }         
        } 
    }
}

