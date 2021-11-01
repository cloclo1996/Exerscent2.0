using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class setName : MonoBehaviour {

	public GameObject currentBtn; 
	public Image theImage;

	void Start () {
		Transform tempTransform =  this.gameObject.transform.parent;
		currentBtn = tempTransform.gameObject;

	}
	
	// Update is called once per frame
	void Update () {

		string tempTitle=currentBtn.name; //Assign button name to string variable

		theImage = currentBtn.gameObject.GetComponent<Image> ();  //Assign Image to Image variable

		tempTitle=theImage.sprite.name;                          //Assign the sprite name to temporary variable
		string finalString=tempTitle.Substring(3);               //Assign the title, starting from the third char(ignoring 'img' prefix) (0)I (1)M (2)G 

		Text textObject = this.gameObject.GetComponent<Text> ();  //Assign text component of game object with this script attatched to a Text variable
		textObject.text = finalString.ToUpper();                  //Make the final string uppercase and assign it to the text object component

	}
}
