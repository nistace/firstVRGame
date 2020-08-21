using UnityEngine;
using UnityEngine.Events;

public class Pistol : MonoBehaviour {
	public class BulletShotEvent : UnityEvent<Bullet> { }

	[SerializeField] protected Bullet      _bulletPrefab;
	[SerializeField] protected float       _bulletShootForce = 50;
	[SerializeField] protected Transform   _bulletSpawn;
	[SerializeField] protected AudioSource _audioSource;

	private HandControllerInput controllerInput { get; set; }

	public BulletShotEvent onBulletShot { get; } = new BulletShotEvent();

	private void Start() {
		controllerInput = GetComponentInParent<HandControllerInput>();
		controllerInput.onTriggerPressed.AddListener(HandleShoot);
	}

	private void HandleShoot() {
		var bullet = Instantiate(_bulletPrefab, _bulletSpawn.position, Quaternion.identity, null);
		bullet.source = controllerInput.isRightController ? PlayerDamageSource.RightPistol : PlayerDamageSource.LeftPistol;
		bullet.Shoot(_bulletSpawn.forward * _bulletShootForce);
		Destroy(bullet.gameObject, 3);
		_audioSource.Play();
		onBulletShot.Invoke(bullet);
	}
}