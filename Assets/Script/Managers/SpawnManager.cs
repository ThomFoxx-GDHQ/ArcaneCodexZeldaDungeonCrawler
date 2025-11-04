using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] private GameObject[] _lootSpawnPrefabs;
    [SerializeField] private int[] _lootDropRate;
    [SerializeField] private float _lootDropChance = .75f;
    private int _lootDropTotal;

    private void Start()
    {
       foreach(int rate in _lootDropRate)
            _lootDropTotal += rate;
    }

    public void DropLoot(Vector3 position)
    {
        float chance = Random.value;
        if (chance > _lootDropChance) return;

        int drop = Random.Range(0, _lootDropTotal);

        for (int i = 0; i < _lootDropRate.Length; i++)
        {
            if (drop < _lootDropRate[i])
            {
                Instantiate(_lootSpawnPrefabs[i], position, Quaternion.identity);
                return;
            }
            else
                drop -= _lootDropRate[i];
        }
    } 
}