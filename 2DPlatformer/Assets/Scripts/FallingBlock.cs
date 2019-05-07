using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour {
	enum FallBlockState
	{
		Idle,
		FallDelay,
		Falling,
	}
	FallBlockState _state = FallBlockState.Idle;
	public float FallDelay = 2f;
	public float FallSpeed = 7f;

	private void Update(){
		switch (_state)
		{
			case FallBlockState.FallDelay:
				FallDelay -= Time.deltaTime;
				if(FallDelay <= 0f){
					_state = FallBlockState.Falling;
				}
				break;
			case FallBlockState.Falling:
				transform.position += Vector3.down * FallSpeed * Time.deltaTime;
				break;
		}
	}

	private void OnBecameInvisible(){
		if (_state != FallBlockState.Idle) Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D collision){
		if(_state == FallBlockState.Idle)
			_state = FallBlockState.FallDelay;
	}
}
