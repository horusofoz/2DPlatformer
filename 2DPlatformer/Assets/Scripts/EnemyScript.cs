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
			bool condition = false;
			foreach(var collision in ray)
			{
				if (collision.collider != null && (string.IsNullOrEmpty(Tag) || Tag == collision.collider.gameObject.tag))
				{
					condition = true;
				}
			}
			return condition;
		}
		public RaycastHit2D[] Ray(Vector2 parent = new Vector2(), Directions direction = Directions.Right)
		{
			// Relativity for turning around
			Vector2 relativeDirection = Direction;
			relativeDirection.x *= (int)direction;
			Vector2 relativePosition = Position;
			relativePosition.x *= (int)direction;
			relativePosition += parent;
			if (LayerToInteract != default(LayerMask))
			{
				return Physics2D.RaycastAll(relativePosition, relativeDirection, Distance, LayerToInteract.value);
			}
			return Physics2D.RaycastAll(relativePosition, relativeDirection, Distance);
		}
	}
	public enum Directions
	{
		Left = -1,
		Right = 1,
	}
	private bool _grounded;
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
			// This is for the sake of the spinning it does if it's in the air. Again, a better solution is welcome. 
			if (!FloorCheck.CheckHit(transform.position, Direction))
			{
				FloorCheck.Event.Invoke();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
			_grounded = true;
	}
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
			_grounded = false;
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
		if (!_grounded) return;
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
}