using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyEdgePatrol : MonoBehaviour
{
	private Rigidbody2D _rb;
	[Header("Movement")]
	public float Speed;
	public float footDist;

	public float AvoidWallDist = 0.3f;
	public float AvoidDropDist = 0.3f;

	private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		GetComponent<Animator>().SetBool("isWalking", true);
	}

	private void FixedUpdate()
	{
		_rb.velocity = new Vector2(Speed * Time.fixedDeltaTime, _rb.velocity.y);
	}

	private void Update()
	{
		EdgeCheck();
		WallCheck();
		GetComponent<SpriteRenderer>().flipX = Speed < 0 ? true : false;
	}

	private void WallCheck()
	{
		#region Check distance to walls
		RaycastHit2D leftWallRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.left, 5, LayerMask.GetMask("Ground"));
		RaycastHit2D rightWallRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.right, 5, LayerMask.GetMask("Ground"));
		#endregion
		#region If the there's a wall on the left, go right
		if (leftWallRay.collider != null && leftWallRay.distance < AvoidWallDist)
		{
			Speed = Speed > 0 ? Speed : -Speed;
		}
		#endregion
		#region If there's a wall to the right, go left
		else if (rightWallRay.collider != null && rightWallRay.distance < AvoidWallDist)
		{
			Speed = Speed < 0 ? Speed : -Speed;
		}
		#endregion
	}

	private void EdgeCheck()
	{
		#region  Get feet positions
		Vector2 leftFoot = new Vector2(
		/*X*/transform.position.x - footDist / 2f,
		/*Y*/transform.position.y);
		Vector2 rightFoot = new Vector2(
		/*X*/transform.position.x + footDist / 2f,
		/*Y*/transform.position.y);
		#endregion
		#region Check raycast for each foot
		RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down);
		RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down);
		#endregion
		// If there's no difference, just don't do anything
		if (rightRay.distance == leftRay.distance) return;

		#region If the left foot is over the edge, go right
		if (leftRay.collider == null || leftRay.distance > AvoidDropDist)
		{
			Speed = Speed > 0 ? Speed : -Speed;
		}
		#endregion
		#region If the right foot is over the edge, go left
		else if (rightRay.collider == null || rightRay.distance > AvoidDropDist)
		{
			Speed = Speed < 0 ? Speed : -Speed;
		}
		#endregion

	}
#if UNITY_EDITOR
	RaycastHit2D leftWallRay;
	RaycastHit2D rightWallRay;
	private void OnDrawGizmos()
	{
		leftWallRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.left, 5, LayerMask.GetMask("Ground"));
		rightWallRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.right, 5, LayerMask.GetMask("Ground"));
		Vector2 leftFoot = new Vector2(transform.position.x - footDist / 2f, transform.position.y);
		Vector2 rightFoot = new Vector2(transform.position.x + footDist / 2f, transform.position.y);
		Gizmos.DrawCube(leftFoot, new Vector3(0.2f, 0.2f, 0.2f));
		Gizmos.DrawCube(rightFoot, new Vector3(0.2f, 0.2f, 0.2f));
		if(leftWallRay.collider != null)
			Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 0.5f), leftWallRay.point);
		if (rightWallRay.collider != null)
			Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 0.5f), rightWallRay.point);
	}
#endif
}