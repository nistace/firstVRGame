using UnityEngine;
using UnityEngine.Events;

public class Platform : MonoBehaviour {
	[SerializeField] protected Renderer _renderer;
	[SerializeField] protected Material _healthyMaterial;
	[SerializeField] protected Material _damagedMaterial;
	[SerializeField] protected Material _deadMaterial;
	[SerializeField] protected float    _damagedTime = 3;

	private bool beenDamaged { get; set; }

	private float delayBeforeReactivate { get; set; }

	private bool isOn { get; set; }

	public UnityEvent onDamaged { get; } = new UnityEvent();

	private void Update() {
		if (!isOn) return;
		if (!beenDamaged) return;
		delayBeforeReactivate -= Time.deltaTime;
		if (delayBeforeReactivate > 0) return;
		beenDamaged = false;
		_renderer.material = _healthyMaterial;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;
		var enemy = other.GetComponentInParent<Enemy>();
		if (!enemy) return;
		if (!beenDamaged) {
			beenDamaged = true;
			_renderer.material = _damagedMaterial;
			delayBeforeReactivate = _damagedTime;
			onDamaged.Invoke();
		}
		enemy.Kill(PlayerDamageSource.Platform);
	}

	public void SetOn(bool isOn) {
		this.isOn = isOn;
		_renderer.material = isOn ? _healthyMaterial : _deadMaterial;
		beenDamaged = false;
	}
}