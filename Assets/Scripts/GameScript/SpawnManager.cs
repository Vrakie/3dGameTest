using UnityEngine;
public class SpawnManager : MonoBehaviour
{
    #region VARIABLE
    public static SpawnManager Instance;
	public Spawnpoint[] spawnpoints;
    #endregion
    void Awake() => Instance = this;
	public Transform GetSpawnpoint() => spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
}
