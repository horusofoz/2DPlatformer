using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyScript : MonoBehaviour
{
	// A class dedicated to ray information and an accompanying action to excecute in-game. 
	// This can be used to trigger animations or states, such as the wolf jump and turning around
	// on ray collision.
	[Serializable]
	public class RayCheckAction
	{
		// A completely useless field, I am using it for identification and ease of use in the editor
		public string ActionName = "Action";
		[Header("Ray Instructions")]
		public Vector2 Position;
		public Vector2 Direction;
		public float Distance;
		[Header("Action and Collisions")]
		public string Tag;
		public LayerMask LayerToInteract = default(LayerMask);
		public UnityEvent Event;
		public bool CheckHit(Vector2 parent = new Vector2(), Directions direction = Directions.Right)
		{
			var ray = Ray(parent, direction);
			return ray.collider != null && (string.IsNullOrEmpty(Tag) || Tag == ray.collider.gameObject.tag);
		}
		public RaycastHit2D Ray(Vector2 parent = new Vector2(), Directions direction = Directions.Right)
		{
			// Relativity for turning around
			Vector2 relativeDirection = Direction;
			relativeDirection.x *= (int)direction;
			Vector2 relativePosition = Position;
			relativePosition.x *= (int)direction;
			relativePosition += parent;
			if (LayerToInteract != default(LayerMask))
			{
				return Physics2D.Raycast(relativePosition, relativeDirection, Distance, LayerToInteract.value);
			}
			return Physics2D.Raycast(relativePosition, relativeDirection, Distance);
		}
	}
	public enum Directions
	{
		Left = -1,
		Right = 1,
	}
	private Rigidbody2D _rb;
	[Header("Movement")]
	public float Speed;
	public Directions Direction = Directions.Right;
	[Header("Actions and Checks")]
	public RayCheckAction[] CheckActions;
	// There's Probably a better way to do this, but since "floor check" is checking for a lack of collision, 
	// the checker in the update loop won't work for checking an absence of floor. 
	public RayCheckAction FloorCheck;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}
	private void Start()
	{
		// Just to check if the level designer wanted a skeleton that faced left in the beginning
		// and hadn't set the direction. 
		Direction = transform.localScale.x < 0 ? Directions.Left : Directions.Right;
		if (GetComponent<Animator>())
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
		// Foreach RayCheckAction, check if there has been a collision and invoke the action. 
		foreach (var check in CheckActions)
		{
			if (check.CheckHit(transform.position, Direction))
			{
				check.Event.Invoke();
			}
		}
		// Floor check and subsequent action. 
		if (!FloorCheck.CheckHit(transform.position, Direction))
		{
			FloorCheck.Event.Invoke();
		}
	}
	#region Actions
	public void TurnAround()
	{ 
		Direction = (Directions)(-(int)Direction);

		Vector3 newScale = transform.localScale;
		newScale.x *= newScale.x < 0 ? -(float)Direction : (float)Direction;
		transform.localScale = newScale;
	}
	public void Leap(float force)
	{
		print("leap");
		// not moving in the air already, will improve this
		if(_rb.velocity.y == 0)
			_rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
	}
	#endregion
#if UNITY_EDITOR
	public bool ShowGizmos = true;
	private void OnDrawGizmos()
	{
		if (!ShowGizmos) return;
		foreach (var check in CheckActions)
		{
			DrawRay(check);
		}
		DrawRay(FloorCheck);
	}
	public void DrawRay(RayCheckAction rayCheck)
	{
		// Get relative direction vector scaled by distance
		Vector2 relativeDirection = rayCheck.Direction;
		relativeDirection.x *= (int)Direction;
		relativeDirection *= rayCheck.Distance;
		// Get relative position
		Vector2 relativePosition = rayCheck.Position;
		relativePosition.x *= (int)Direction;
		relativePosition += (Vector2)transform.position;
		// Draw relative position to scaled relative directions
		Gizmos.DrawLine(relativePosition, relativePosition + relativeDirection);
	}
#endif
	/*
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
				// Check again to prevent turning once per frame
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
	public bool ShowGizmos = true;
	private void OnDrawGizmos()
	{
		if (!ShowGizmos) return;
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
*/
}