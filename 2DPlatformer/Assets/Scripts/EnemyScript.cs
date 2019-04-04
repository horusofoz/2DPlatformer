using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyScript : MonoBehaviour
{
	public enum Directions
	{
		Left = -1,
		Right = 1,
	}
	public Directions Direction = Directions.Right;
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
		Direction = transform.localScale.x < 0 ? Directions.Left : Directions.Right;
		if(DoEdgeCheck)_checksToMake += EdgeCheck;
		if(DoWallCheck)_checksToMake += WallCheck;
		if(GetComponent<Animator>())
			GetComponent<Animator>().SetBool("isWalking", true);
	}

	private void FixedUpdate()
	{
		Vector3 newVelocity = _rb.velocity;
		// It should be nothing. I made the mistake of trying to normalize it so if we changed the fixed update intervals it would 
		// maintain the same speed. I was wrong as the physics engine takes care of that, this should specify the m/s
		// Not the actual movement distance, which would be normalized by deltaTime. Good pickup. 
		newVelocity.x = (float)Direction * Speed; // Should this be Time.deltaTime? - http://answers.unity.com/answers/871440/view.html
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
		wallChecker.x += WallCheckPos.x * (float)Direction;
		wallChecker.y += WallCheckPos.y;
		// Check distance to walls
		var forwardRay = Physics2D.Raycast(wallChecker, Vector2.right * (float)Direction, AvoidWallDist, LayerMask.GetMask("Ground"));
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
		edgeCheckerPos.x += EdgeCheckPos.x * (float)Direction;
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
		Direction = (Directions)(-(int)Direction);

		Vector3 newScale = transform.localScale;
		newScale.x *= newScale.x < 0 ? -(float)Direction : (float)Direction;
		transform.localScale = newScale;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{	
		// Wall Gizmo
		Gizmos.DrawLine(
			(Vector2)transform.position + WallCheckPos,
			(Vector2)transform.position + WallCheckPos + Vector2.right * (float)Direction * AvoidWallDist);
		// Edge Gizmo
		Vector2 edgeCheckerPos = transform.position;
		edgeCheckerPos.x += EdgeCheckPos.x * (float)Direction;
		edgeCheckerPos.y += EdgeCheckPos.y;
		Vector2 edgeCheckEndPos = edgeCheckerPos - Vector2.up * AvoidDropDist;
		// Cube
		Gizmos.DrawCube(edgeCheckerPos, Vector3.one * 0.2f);
		// Line
		Gizmos.DrawLine(edgeCheckerPos, edgeCheckEndPos);
	}
#endif
}