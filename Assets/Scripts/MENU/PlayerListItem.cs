﻿using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
public class PlayerListItem : MonoBehaviourPunCallbacks
{
    #region VARIABLE
    [SerializeField] TMP_Text text;
	public Player player;
    #endregion
    public void SetUp(Player _player)
	{
		player = _player;
		text.text = _player.NickName;
	}
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if(player == otherPlayer)
			Destroy(gameObject);
	}
	public override void OnLeftRoom() => Destroy(gameObject);
}