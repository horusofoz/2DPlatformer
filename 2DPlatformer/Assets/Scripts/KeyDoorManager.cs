using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorManager : MonoBehaviour {
	public KeyDoorPair[] KeyDoors;
	private void Awake()
	{
		foreach (var keydoorpair in KeyDoors)
		{
			keydoorpair.SetKeyDoors();
		}
	}
}
