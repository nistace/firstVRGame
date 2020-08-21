using UnityEngine;

public class MiscStats : MonoBehaviour {
	[SerializeField] protected TextMesh _leftHandBulletsCount;
	[SerializeField] protected TextMesh _leftHandKillCount;
	[SerializeField] protected TextMesh _leftHandAccuracy;
	[SerializeField] protected TextMesh _rightHandBulletsCount;
	[SerializeField] protected TextMesh _rightHandKillCount;
	[SerializeField] protected TextMesh _rightHandAccuracy;

	private int countLeftBulletsShot  { get; set; }
	private int countRightBulletsShot { get; set; }
	private int countLeftKills        { get; set; }
	private int countRightKills       { get; set; }

	private void Start() {
		var gameManager = FindObjectOfType<GameManager>();
		gameManager.onStartGame.AddListener(EnableListeners);
		gameManager.onStartGame.AddListener(ResetStats);
		FindObjectOfType<GameManager>().onEndGame.AddListener(DisableListeners);
		ResetStats();
	}

	private void ResetStats() {
		countLeftBulletsShot = 0;
		countRightBulletsShot = 0;
		countLeftKills = 0;
		countRightKills = 0;
		RefreshUi();
	}

	private void EnableListeners() {
		foreach (var pistol in FindObjectsOfType<Pistol>()) {
			pistol.onBulletShot.AddListener(HandleBulletShot);
		}
		FindObjectOfType<EnemySpawner>().onEnemyKilled.AddListener(HandleEnemyKilled);
	}

	private void DisableListeners() {
		foreach (var pistol in FindObjectsOfType<Pistol>()) {
			pistol.onBulletShot.RemoveListener(HandleBulletShot);
		}
		FindObjectOfType<EnemySpawner>().onEnemyKilled.RemoveListener(HandleEnemyKilled);
	}

	private void HandleEnemyKilled(Enemy enemy, PlayerDamageSource source) {
		if (source == PlayerDamageSource.LeftPistol) countLeftKills++;
		else if (source == PlayerDamageSource.RightPistol) countRightKills++;
		else return;
		RefreshUi();
	}

	private void HandleBulletShot(Bullet bullet) {
		if (bullet.source == PlayerDamageSource.LeftPistol) countLeftBulletsShot++;
		else if (bullet.source == PlayerDamageSource.RightPistol) countRightBulletsShot++;
		else return;
		RefreshUi();
	}

	private void RefreshUi() {
		_leftHandBulletsCount.text = $"Shots: {countLeftBulletsShot}";
		_leftHandKillCount.text = $"Kills: {countLeftKills}";
		_leftHandAccuracy.text = $"Accuracy: {(countLeftBulletsShot > 0 ? (float) countLeftKills / countLeftBulletsShot : 0):P2}";
		_rightHandBulletsCount.text = $"Shots: {countRightBulletsShot}";
		_rightHandKillCount.text = $"Kills: {countRightKills}";
		_rightHandAccuracy.text = $"Accuracy: {(countRightBulletsShot > 0 ? (float) countRightKills / countRightBulletsShot : 0):P2}";
	}
}