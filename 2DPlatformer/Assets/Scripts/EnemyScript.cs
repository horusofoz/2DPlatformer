using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyScript : MonoBehaviour
{
	private enum dir
	{
		Left = -1,
		Right = 1,
	}
	private dir _direction = dir.Right;
	private Rigidbody2D _rb;
	private delegate void Checks();
	private Checks _checksToMake;
	[Header("Behaviours")]
	public bool DoEdgeCheck;
	public bool DoWallCheck = true;
	[Header("Movement")]
	public float Speed;
	[Header("Sensitivity")]
	public Vector2 WallCheckPos;
	public Vector2 EdgeCheckPos;
	//public float EdgeDist = 1f;
	public float AvoidWallDist = 0.3f;
	public float AvoidDropDist = 0.3f;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}
	private void Start()
	{
		_direction = transform.localScale.x < 0 ? dir.Left : dir.Right;
		if(DoEdgeCheck)_checksToMake += EdgeCheck;
		if(DoWallCheck)_checksToMake += WallCheck;
		_checksToMake += delegate() 
		{
			GetComponent<SpriteRenderer>().flipX = _direction == dir.Left ? true : false;
		};
		GetComponent<Animator>().SetBool("isWalking", true);
	}

	private void FixedUpdate()
	{
		Vector3 newVelocity = _rb.velocity;
		newVelocity.x = (float)_direction * Speed * Time.fixedDeltaTime;
		_rb.velocity = newVelocity;

	}

	private void Update()
	{
		// Equivalent to running all of the required checks
		//Eg.
		//WallCheck();
		//EdgeCheck();
		//GetComponent<SpriteRenderer>().flipX = _direction == dir.Left ? true : false;
		_checksToMake();
	}

	private void WallCheck()
	{
		Vector2 wallChecker = (Vector2)transform.position;
		wallChecker.x += WallCheckPos.x * (float)_direction;
		wallChecker.y += WallCheckPos.y;
		// Check distance to walls
		RaycastHit2D forwardRay = Physics2D.Raycast(wallChecker, Vector2.right * (float)_direction, AvoidWallDist, LayerMask.GetMask("Ground"));
		// If the there's a wall, turn around
		if (forwardRay.collider != null)
		{
			_direction = (dir)(0 - (int)_direction);
			print("Wall Turn");
		}
	}

	private void EdgeCheck()
	{
		Vector2 edgeCheckerPos = transform.position;
		edgeCheckerPos.x += EdgeCheckPos.x * (float)_direction;
		edgeCheckerPos.y += EdgeCheckPos.y;

		#region Check raycast for each foot
		RaycastHit2D ray = Physics2D.Raycast(edgeCheckerPos, Vector2.down, AvoidDropDist, LayerMask.GetMask("Ground"));
		#endregion
		//If the foot is over the edge, turn around
		if (ray.collider == null)
		{
			// Turn around
			_direction = (dir)(0 - (int)_direction);
			print("Edge Turn");
		}

	}
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		_direction = transform.localScale.x < 0 ? dir.Left : dir.Right;
		// Wall Gizmo
		Gizmos.DrawLine(
			(Vector2)transform.position + WallCheckPos, 
			(Vector2)transform.position + WallCheckPos + Vector2.right * (float)_direction * AvoidWallDist);
		// Foot Gizmo
		Vector2 edgeCheckerPos = transform.position;
		edgeCheckerPos.x += EdgeCheckPos.x * (float)_direction;
		edgeCheckerPos.y += EdgeCheckPos.y;
		Vector2 edgeCheckEndPos = edgeCheckerPos - Vector2.up * AvoidDropDist;
		// Cube
		Gizmos.DrawCube(edgeCheckerPos, Vector3.one * 0.2f);
		// Line
		Gizmos.DrawLine(edgeCheckerPos, edgeCheckEndPos);

		//leftWallRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.left, 5, LayerMask.GetMask("Ground"));
		//rightWallRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.right, 5, LayerMask.GetMask("Ground"));
		//#region  Get feet positions
		//Vector2 leftFoot = new Vector2(
		///*X*/transform.position.x - FootDist / 2f,
		///*Y*/transform.position.y);
		//Vector2 rightFoot = new Vector2(
		///*X*/transform.position.x + FootDist / 2f,
		///*Y*/transform.position.y);
		//#endregion
		//Gizmos.DrawCube(leftFoot, new Vector2(0.1f, 0.1f));
		//Gizmos.DrawCube(rightFoot, new Vector2(0.1f, 0.1f));
		//	Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 0.5f), leftWallRay.point);
		//Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 0.5f), rightWallRay.point);
	}
#endif
}