using UnityEngine;
[CreateAssetMenu(menuName = "Player Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player

public class PlayerData : ScriptableObject
{
	[Header("Test")]
	public float test;
}