using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
	[SerializeField] protected bool      _isOn;
	[SerializeField] protected Enemy     _enemyPrefab;
	[SerializeField] protected Transform _player;
	[SerializeField] protected float     _spawnDistance = 20;
	[SerializeField] protected float     _minSpawnDelay = 2;
	[SerializeField] protected float     _maxSpawnDelay = 4;
	[SerializeField] protected float     _easiestSpeed  = .4f;
	[SerializeField] protected float     _hardestSpeed  = 4f;
	[SerializeField] protected float     _easiestSize   = 1.5f;
	[SerializeField] protected float     _hardestSize   = .5f;
	[SerializeField] protected float     _easiestScore  = 10;
	[SerializeField] protected float     _hardestScore  = 100;
	[SerializeField] protected int       _level         = 1;

	private HashSet<Enemy> allActiveEnemies { get; } = new HashSet<Enemy>();

	private float spawnDelay { get; set; }

	public int level {
		get => _level;
		set => _level = value;
	}

	public bool isOn {
		get => _isOn;
		set => _isOn = value;
	}

	public Enemy.KilledEvent onEnemyKilled { get; } = new Enemy.KilledEvent();

	private void Start() {
		spawnDelay = Random.Range(_minSpawnDelay, _maxSpawnDelay);
	}

	public void Update() {
		if (!_isOn) return;
		spawnDelay -= Time.deltaTime;
		if (spawnDelay > 0) return;
		SpawnEnemy();
		spawnDelay = Random.Range(_minSpawnDelay, _maxSpawnDelay) / level;
	}

	private void SpawnEnemy() {
		var newEnemy = Instantiate(_enemyPrefab, _player.position + new Vector3(Random.Range(-1f, 1f), 0, 1).normalized * _spawnDistance, Quaternion.identity, null);
		newEnemy.target = _player;
		var speedDifficulty = Random.value;
		var sizeDifficulty = Random.value;
		newEnemy.speed = Mathf.Lerp(_easiestSpeed, _hardestSpeed, speedDifficulty);
		newEnemy.size = Mathf.Lerp(_easiestSize, _hardestSize, sizeDifficulty);
		newEnemy.score = Mathf.Lerp(_easiestScore, _hardestScore, (speedDifficulty + sizeDifficulty) / 2);
		newEnemy.onKilled.AddListener(HandleEnemyKilled);
		allActiveEnemies.Add(newEnemy);
	}

	private void HandleEnemyKilled(Enemy deadEnemy, PlayerDamageSource killSource) {
		allActiveEnemies.Remove(deadEnemy);
		onEnemyKilled.Invoke(deadEnemy, killSource);
	}

	public void KillAll() {
		foreach (var enemy in allActiveEnemies.ToArray()) enemy.Kill(PlayerDamageSource.EndGame);
	}
}