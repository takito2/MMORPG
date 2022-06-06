using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class UIMinimap : MonoSingleton<UIMinimap> {

    public Collider minimapBoudingBox;
    public Image miniMap;
    public Image arrow;
    public Text miniName;

    private Transform playerTransform;

	// Use this for initialization
	//void Start () {
 //       Debug.Log("UIMInimap Init");
 //       //this.InitMap();
	//}

    protected override void OnStart()
    {
        base.OnStart();
    }

    public void InitMap()
    {
        this.miniName.text = User.Instance.CurrentMapData.Name;
        if(this.miniMap.overrideSprite == null)
        this.miniMap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();
        this.miniMap.SetNativeSize();
        this.miniMap.transform.localPosition = Vector3.zero;
        this.playerTransform = User.Instance.CurrentCharacterObject.transform;
    }
	
	// Update is called once per frame
	void Update () {
        float realWidth = minimapBoudingBox.bounds.size.x;
        float realHeight = minimapBoudingBox.bounds.size.z;

        float relaX = playerTransform.position.x - minimapBoudingBox.bounds.min.x;
        float relaY = playerTransform.position.z - minimapBoudingBox.bounds.min.z;

        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        this.miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.miniMap.rectTransform.localPosition = Vector2.zero;

        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }
}
