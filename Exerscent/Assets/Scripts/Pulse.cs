using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Pulse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Slowly pulse size of gameObject foreve
		gameObject.transform.DOScale(new Vector3(1.02f, 1.02f, 1.02f), 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
