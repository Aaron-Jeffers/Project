using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;

public class MenuScript : MonoBehaviour
{
    public GameObject panel;
    private string defaultIPAddress = "127.0.0.1";
    public string ipAddress;
    UNetTransport transport;

    public void HostButton()
    {
        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = ipAddress;
        NetworkManager.Singleton.StartHost();
        setMenuPanel(false);
    }

    public void JoinButton()
    {
        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = ipAddress;
        NetworkManager.Singleton.StartClient();
        setMenuPanel(false);
    }

    public void onChangeIPAddress(string IP)
    {
        ipAddress = IP;
    }

    private void setMenuPanel(bool i)
    {
        panel.SetActive(i);
    }
}
