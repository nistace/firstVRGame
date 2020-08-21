using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
	[SerializeField]                              protected EnemySpawner        _enemySpawner;
	[SerializeField]                              protected Platform            _platform;
	[SerializeField]                              protected HandControllerInput _restartHandInput;
	[Header("Texts in the sky")] [SerializeField] protected TextMesh            _scoreInTheSky;
	[SerializeField]                              protected TextMesh            _levelInTheSky;
	[SerializeField]                              protected TextMesh            _livesInTheSky;
	[Header("Score")] [SerializeField]            protected float               _score;
	[SerializeField]                              protected float               _nextLevelStep              = 500;
	[SerializeField]                              protected float               _additionalScoreForNextStep = 500;
	[SerializeField]                              protected float               _requiredForNextStep        = 500;
	[Header("Lives")] [SerializeField]            protected float               _livesPerGame               = 5;
	[SerializeField]                              protected float               _lives                      = 5;
	[Header("Audio")] [SerializeField]            protected AudioSource         _musicAudioSource;
	[SerializeField]                              protected float               _beforeStartMusicVolume = .1f;
	[SerializeField]                              protected float               _playingMusicVolume     = .7f;
	[SerializeField]                              protected AudioSource         _effectsSource;
	[SerializeField]                              protected AudioClip           _levelUpClip;
	[SerializeField]                              protected AudioClip           _startClip;
	[SerializeField]                              protected AudioClip           _damagedClip;
	[SerializeField]                              protected AudioClip           _lostClip;

	public UnityEvent onStartGame { get; } = new UnityEvent();
	public UnityEvent onEndGame   { get; } = new UnityEvent();

	private void Start() {
		_enemySpawner.onEnemyKilled.AddListener(HandleEnemyKilled);
		_platform.onDamaged.AddListener(HandlePlatformDamaged);
		SetBeforeStart();
	}

	private void HandlePlatformDamaged() {
		_lives--;
		if (_lives <= 0) {
			PlayAudioClip(_lostClip);
			onEndGame.Invoke();
			SetBeforeStart();
		}
		else {
			PlayAudioClip(_damagedClip);
		}
		RefreshUi();
	}

	private void SetBeforeStart() {
		_platform.SetOn(false);
		_enemySpawner.KillAll();
		_enemySpawner.isOn = false;
		_musicAudioSource.volume = _beforeStartMusicVolume;
		_restartHandInput.onPrimaryButtonPressed.AddListener(RestartGame);
		RefreshUi();
	}

	private void RestartGame() {
		_score = 0;
		_enemySpawner.level = 1;
		_lives = _livesPerGame;
		RefreshUi();
		_additionalScoreForNextStep = _nextLevelStep;
		_requiredForNextStep = _additionalScoreForNextStep;
		_enemySpawner.isOn = true;
		_platform.SetOn(true);
		PlayAudioClip(_startClip);
		_musicAudioSource.volume = _playingMusicVolume;
		_restartHandInput.onPrimaryButtonPressed.RemoveListener(RestartGame);
		onStartGame.Invoke();
	}

	private void PlayAudioClip(AudioClip clip) {
		_effectsSource.clip = clip;
		_effectsSource.Play();
	}

	private void HandleEnemyKilled(Enemy enemy, PlayerDamageSource source) {
		if (source != PlayerDamageSource.LeftPistol && source != PlayerDamageSource.RightPistol) return;
		_score += enemy.score;
		if (_score >= _requiredForNextStep) {
			_enemySpawner.level++;
			_additionalScoreForNextStep += _nextLevelStep;
			_requiredForNextStep += _additionalScoreForNextStep;
			PlayAudioClip(_levelUpClip);
		}
		RefreshUi();
	}

	private void RefreshUi() {
		_scoreInTheSky.text = "Score\n" + Mathf.FloorToInt(_score);
		_levelInTheSky.text = "Level\n" + Mathf.FloorToInt(_enemySpawner.level);
		_livesInTheSky.text = "";
		for (var i = 0; i < _lives; ++i) _livesInTheSky.text += '\u2665'.ToString();
	}
}