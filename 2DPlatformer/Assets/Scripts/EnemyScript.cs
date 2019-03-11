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
	dir _direction;
	public float EdgeDist = 1f;
	private Rigidbody2D _rb;
	private delegate void Checks();
	private Checks _checksToMake;
	[Header("Behaviours")]
	public bool DoEdgeCheck;
	public bool DoWallCheck = true;
	[Header("Movement")]
	public float Speed;
	[Header("Sensitivity")]
	public float FootDist;
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
			GetComponent<SpriteRenderer>().flipX = Speed < 0 ? true : false;
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
		_checksToMake();
	}

	private void WallCheck()
	{
		// Check distance to walls
		RaycastHit2D forwardRay = Physics2D.Raycast(transform.position, Vector2.right * (float)_direction, AvoidWallDist, LayerMask.GetMask("Ground"));
		// If the there's a wall on the left, go right
		if (forwardRay.collider != null)
		{
			_direction = (dir)(0 - (int)_direction);
		}
	}

	private void EdgeCheck()
	{
		#region  Get feet positions
		Vector2 rightFoot = new Vector2(
		/*X*/transform.position.x + FootDist / 2f * (float)_direction,
		/*Y*/transform.position.y);
		#endregion
		Vector3 edgeDistDetector = transform.position;
		edgeDistDetector.x += EdgeDist * (float)_direction;
		#region Check raycast for each foot
		RaycastHit2D rightRay = Physics2D.Raycast(edgeDistDetector, Vector2.down, AvoidDropDist, LayerMask.GetMask("Ground"));
		#endregion
		#region If the right foot is over the edge, go left
		if (rightRay.collider == null)
		{
			_direction = (dir)(0 - (int)_direction);
		}
		#endregion

	}
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + Vector3.right * (float)_direction * AvoidWallDist);
		Vector2 rightFoot = new Vector2(
		/*X*/transform.position.x + FootDist / 2f * (float)_direction,
		/*Y*/transform.position.y);
		Gizmos.DrawCube(rightFoot, Vector3.one * 0.2f);
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