using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public Wave[] waves; // The waves the spawner will loop through
    public Enemy enemy;

    Wave currentWave; // Gives access to comparisons for triggers
    int currentWaveNumber;

    int enemiesRemainingToSpawn; // Stop spawning when this == 0
    int enemiesRemainingAlive; // Start the next wave when this == 0
    float nextSpawnTime; // Spawn cooldown

    void Start() {
        NextWave();
    }

    void Update() {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime) {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy; // Spawn a new enemy
            spawnedEnemy.OnDeath += OnEnemyDeath; // Subscribe that enemy's OnEnemyDeath() method to the OnDeath event
        }
    }

    void OnEnemyDeath() {
        enemiesRemainingAlive--;
        print("Enemy died"); // Debug

        // Conditions for beginning the next wave (currently triggers if all enemies are dead)
        if (enemiesRemainingAlive == 0) {
            NextWave(); 
        }
    }

    void NextWave() {
        currentWaveNumber++;
        print("Wave: " + currentWaveNumber); // Debug
        if (currentWaveNumber - 1 < waves.Length)
            currentWave = waves[currentWaveNumber - 1];

        enemiesRemainingToSpawn = currentWave.enemyCount;
        enemiesRemainingAlive = enemiesRemainingToSpawn;
    }
    }

    [System.Serializable]
    public class Wave {
        public int enemyCount; // The number of enemies to spawn
        public float timeBetweenSpawns; // Spawn cooldown
    }

