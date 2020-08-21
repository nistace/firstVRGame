using UnityEngine;

public class Bullet : MonoBehaviour {
	[SerializeField] protected Rigidbody _rigidbody;

	public PlayerDamageSource source { get; set; }

	public void Shoot(Vector3 force) => _rigidbody.AddForce(force);
}