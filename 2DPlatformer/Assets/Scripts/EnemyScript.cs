using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyScript : MonoBehaviour
{
	public enum Direction
	{
		Left = -1,
		Right = 1,
	}
	public Direction _direction = Direction.Right;
	private Rigidbody2D _rb;
	private delegate bool Checks();
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
		_direction = transform.localScale.x < 0 ? Direction.Left : Direction.Right;
		if(DoEdgeCheck)_checksToMake += EdgeCheck;
		if(DoWallCheck)_checksToMake += WallCheck;
		GetComponent<Animator>().SetBool("isWalking", true);
	}

	private void FixedUpdate()
	{
		Vector3 newVelocity = _rb.velocity;
		newVelocity.x = (float)_direction * Speed * Time.fixedDeltaTime; // Should this be Time.deltaTime? - http://answers.unity.com/answers/871440/view.html
        _rb.velocity = newVelocity;
	}

	private void Update()
	{
		// Equivalent to running all of the required checks, used for togglability
		foreach (Checks check in _checksToMake.GetInvocationList())
		{
			if (check())
			{
				TurnAround();
				if (check())
				{
					TurnAround();
				}
			}
		}
	}

	private bool WallCheck()
	{
		Vector2 wallChecker = transform.position;
		wallChecker.x += WallCheckPos.x * (float)_direction;
		wallChecker.y += WallCheckPos.y;
		// Check distance to walls
		var forwardRay = Physics2D.Raycast(wallChecker, Vector2.right * (float)_direction, AvoidWallDist, LayerMask.GetMask("Ground"));
		// If the there's a wall, turn around
		if (forwardRay.collider != null)
		{
			return true;
		}
		return false;
	}

	private bool EdgeCheck()
	{
		Vector2 edgeCheckerPos = transform.position;
		edgeCheckerPos.x += EdgeCheckPos.x * (float)_direction;
		edgeCheckerPos.y += EdgeCheckPos.y;
		// Check for floor
		var downRay = Physics2D.Raycast(edgeCheckerPos, Vector2.down, AvoidDropDist, LayerMask.GetMask("Ground"));
		//If the foot is over the edge, turn around
		if (downRay.collider == null)
		{
			return true;
		}
		return false;
	}

	private void TurnAround()
	{
		_direction = (Direction)(-(int)_direction);

		Vector3 newScale = transform.localScale;
		newScale.x *= newScale.x < 0 ? -(float)_direction : (float)_direction;
		transform.localScale = newScale;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{	
		// Wall Gizmo
		Gizmos.DrawLine(
			(Vector2)transform.position + WallCheckPos,
			(Vector2)transform.position + WallCheckPos + Vector2.right * (float)_direction * AvoidWallDist);
		// Edge Gizmo
		Vector2 edgeCheckerPos = transform.position;
		edgeCheckerPos.x += EdgeCheckPos.x * (float)_direction;
		edgeCheckerPos.y += EdgeCheckPos.y;
		Vector2 edgeCheckEndPos = edgeCheckerPos - Vector2.up * AvoidDropDist;
		// Cube
		Gizmos.DrawCube(edgeCheckerPos, Vector3.one * 0.2f);
		// Line
		Gizmos.DrawLine(edgeCheckerPos, edgeCheckEndPos);
	}
#endif
}