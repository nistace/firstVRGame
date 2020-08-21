using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour {
	public class KilledEvent : UnityEvent<Enemy, PlayerDamageSource> { }

	[SerializeField] protected Transform    _target;
	[SerializeField] protected float        _speed = 1;
	[SerializeField] protected float        _score;
	[SerializeField] protected AudioSource  _audioSource;
	[SerializeField] protected Renderer     _bodyRenderer;
	[SerializeField] protected GameObject[] _aliveGameObjects;
	[SerializeField] protected GameObject[] _deadGameObjects;
	[SerializeField] protected Material     _deadMaterial;

	public Transform target {
		get => _target;
		set => _target = value;
	}

	public float speed {
		get => _speed;
		set => _speed = value;
	}

	public float score {
		get => _score;
		set => _score = value;
	}

	public float size {
		get => transform.localScale.x;
		set => transform.localScale = new Vector3(value, value, value);
	}

	private bool killed { get; set; }

	public KilledEvent onKilled { get; } = new KilledEvent();

	private new Transform transform { get; set; }

	private void Awake() {
		transform = base.transform;
	}

	private void Update() {
		transform.LookAt(target.position);
		if (killed) {
			transform.position += Time.deltaTime * Vector3.up;
			size = Mathf.Lerp(size, 0, Time.deltaTime);
		}
		else transform.position += Time.deltaTime * _speed * transform.forward;
	}

	private void OnCollisionEnter(Collision other) {
		if (other.collider.gameObject.layer != LayerMask.NameToLayer("Bullet")) return;
		var bullet = other.collider.GetComponentInParent<Bullet>();
		if (bullet) Kill(bullet.source);
	}

	public void Kill(PlayerDamageSource source) {
		if (killed) return;
		killed = true;
		_audioSource.Play();
		_bodyRenderer.material = _deadMaterial;
		foreach (var aliveGameObject in _aliveGameObjects) aliveGameObject.SetActive(false);
		foreach (var deadGameObject in _deadGameObjects) deadGameObject.SetActive(true);
		Destroy(gameObject, 2);
		foreach (var childCollider in GetComponentsInChildren<Collider>()) childCollider.enabled = false;
		onKilled.Invoke(this, source);
	}
}