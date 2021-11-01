using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class resizeOnHover: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

	public int hoverSize;                    //Button hover size
	public int originalSize;                 //Button original size
	private RectTransform thisRect;          //Rect - UI object treated as button


	void Start(){
		thisRect = GetComponent<RectTransform>();                        //Assign rect component of "this" attached game object
		thisRect.sizeDelta = new Vector2( originalSize, originalSize);   //Assign size based on Inspector Integer
	}

	//Resize UI Rect object on hover - make bigger
	public void OnPointerEnter(PointerEventData eventData)
	{
		thisRect.sizeDelta = new Vector2( hoverSize, hoverSize);  
	}

	//Resize UI Rect object on hover exit - back to original size
	public void OnPointerExit(PointerEventData eventData)
	{

		thisRect.sizeDelta = new Vector2( originalSize, originalSize);

	}

}

		
