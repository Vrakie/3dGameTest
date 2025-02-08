using Photon.Pun;
using UnityEngine;

public class PingDisplay : MonoBehaviour
{
    private Rect pingRect = new Rect(10, 400, 200, 30);

    private void OnGUI()
    {
        if (!PhotonNetwork.IsConnected) return;

        int ping = PhotonNetwork.GetPing();
        Color originalColor = GUI.color;


        if (ping < 50) GUI.color = Color.green;
        else if (ping < 100) GUI.color = Color.yellow;
        else GUI.color = Color.red;

        GUI.Label(pingRect, "Ping : " + ping + " ms");

        GUI.color = originalColor;
    }
}
