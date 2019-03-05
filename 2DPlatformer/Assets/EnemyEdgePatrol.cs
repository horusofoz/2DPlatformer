using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyEdgePatrol : MonoBehaviour
{
	private Rigidbody2D _rb;
	private BoxCollider2D _collider;
	[Header("Movement")]
	public float Speed;
	public float AvoidedFallDist = 0.5f;
	public float leftDistance;
	public float rightDistance;
	public Vector2 leftFoot;
	public Vector2 rightFoot;

	private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_collider = GetComponent<BoxCollider2D>();
	}

	private void FixedUpdate()
	{
		_rb.velocity = new Vector2(Speed * Time.fixedDeltaTime, _rb.velocity.y);
	}

	private void Update()
	{
		 leftFoot = new Vector2(_collider.transform.position.x - 1, _collider.transform.position.y +0.05f);
		 rightFoot = new Vector2(_collider.transform.position.x + 1, _collider.transform.position.y + 0.05f);
		RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down);
		leftDistance = leftRay.distance;
		if(leftRay.collider == null || leftRay.distance > AvoidedFallDist)
		{
			print("left detected");
			Speed = Speed < 0 ? -Speed : Speed;
		}
		RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down);
		rightDistance = rightRay.distance;
		if (rightRay.collider == null || rightRay.distance > AvoidedFallDist)
		{
			print("right detected");
			Speed = Speed > 0 ? -Speed : Speed;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawCube(leftFoot, new Vector3(0.2f, 0.2f, 0.2f));
		Gizmos.DrawCube(rightFoot, new Vector3(0.2f, 0.2f, 0.2f));
	}
}
