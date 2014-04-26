using UnityEngine;
using System.Collections;

public class NetworkPlayerInfo {

	public NetworkPlayer m_networkPlayer;
	public string m_playerName;

	public NetworkPlayerInfo(NetworkPlayer networkPlayer, string name)
	{
		m_networkPlayer = networkPlayer;
		m_playerName = name;
	}
}
