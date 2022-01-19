using MLAPI;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject panel;

    public void HostButton()
    {
        NetworkManager.Singleton.StartHost();
        setMenuPanel(false);
    }

    public void JoinButton()
    {
        NetworkManager.Singleton.StartClient();
        setMenuPanel(false);
    }

    private void setMenuPanel(bool i)
    {
        panel.SetActive(i);
    }
}
