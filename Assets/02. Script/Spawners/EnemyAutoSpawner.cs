using UnityEngine;
using ArelWars.Units;

public class EnemyAutoSpawner : MonoBehaviour
{
    public Spawner enemyUp;
    public Spawner enemyDown;
    public float minInterval = 2.5f;
    public float maxInterval = 4.5f;
    private float timer;

    private void OnEnable()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawner sp = (Random.value < 0.5f) ? enemyUp : enemyDown;
            if (sp != null)
            {
                var r = Random.value;
                var t = r < 0.34f ? UnitType.Warrior : (r < 0.67f ? UnitType.Shielder : UnitType.Archer);
                sp.SpawnUnit(t);
            }
            timer = Random.Range(minInterval, maxInterval);
        }
    }
}
