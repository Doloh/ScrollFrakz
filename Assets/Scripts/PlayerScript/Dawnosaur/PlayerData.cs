using UnityEngine;
[CreateAssetMenu(menuName = "Player Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player

public class PlayerData : ScriptableObject
{
	[Header("Run")]
	public float runMaxSpeed; //Target speed we want the player to reach. (distance parcourue en 1 fixedUpdate = 0.2s)
	[Range(1f, 10)] public float runAcceleration; // Combien de 0,1s pour reach maxspeed (1=> instant, 10=> 1 seconde)
	[Range(1f, 10)] public float runDecceleration; // Même principe que l'acceleration
	[HideInInspector] public float runAccelAmount; // Le ratio d'accélération calculé qui sera appliqué dans la formule de run finale
	[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .

	//Unity Callback, called when the inspector updates (quand on change une value sur unity)
	// C'est ici qu'on calcul nos variables computed
	private void OnValidate()
	{

		// Transforme le run acceleration en un vrai coeff à appliquer sur le calcul de la force (voir readme pour explication du coeff)
		runAccelAmount = (1/Time.fixedDeltaTime) * (1/runAcceleration);
		runDeccelAmount = (1 / Time.fixedDeltaTime) * (1 / runDecceleration);
	}
}