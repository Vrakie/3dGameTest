using Photon.Realtime;
using TMPro;
using UnityEngine;
public class RoomListItem : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] TMP_Text text;
	public RoomInfo info;
    #endregion
    public void SetUp(RoomInfo _info)
	{
		info = _info;
		text.text = _info.Name;
	}
	public void OnClick() => Launcher.Instance.JoinRoom(info);
}