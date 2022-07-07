using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class MapController : MonoBehaviour {

    public Collider minimapBoundingbox;
	// Use this for initialization
	void Start () {
        MinimapManager.Instance.UpdateMinimap(minimapBoundingbox);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
