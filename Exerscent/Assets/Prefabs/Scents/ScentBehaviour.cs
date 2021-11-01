using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

//Category enumerator
public enum ScentCategory {Fruit, Plant, Vegetable};
public class ScentBehaviour : MonoBehaviour {
	//Scent category
	public ScentCategory category; 
	//Scent image sprite renderer
	public Image scentImage;
	public Image separator;
	public Image background;
	public Color scentColor;
	public TextMeshProUGUI scentName;
	public gameSystemLogic gameManager;
	public scentData thisData;

	public bool selected = false;
	bool right = false;
	bool animating = false;
	
	public float cardSpeed = 1;

	// Use this for initialization
	void Start () {
		//Reference to game manager
		gameManager = GameObject.FindObjectOfType<gameSystemLogic>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Debug function that other scripts can call to see if linking scripts are succesfull
	public void debugFunc(){
		Debug.Log("It's working.");
	}

	//Set parameters on spawn
	public void setData(scentData newData) {
		scentImage.sprite = newData.scentSprite;
		category = newData.category;
		scentName.SetText(newData.name);
		scentColor = newData.cardColor;
		thisData = newData;
		Color32 colorSample = AverageColorFromTexture(scentImage.sprite.texture);
		background.color = new Color32(
			(byte)((int)map(colorSample.r, 0, 255, 80, 255)),
			(byte)((int)map(colorSample.g, 0, 255, 80, 255)),
			(byte)((int)map(colorSample.b, 0, 255, 80, 255)),
			255
			);
		scentName.color = new Color32(
			(byte)((int)map(colorSample.r, 0, 255, 0, 150)),
			(byte)((int)map(colorSample.g, 0, 255, 0, 150)),
			(byte)((int)map(colorSample.b, 0, 255, 0, 150)),
			255
			);
	}

    //Fetch scent data
	public scentData getData() {
		return thisData;
	}

	//Check if correct on click
	public void onClick() {
		transform.DOShakePosition(0.3f, 10, 25, 90);
		selected = true;
		gameManager.checkAttempt();
		
	}

	//Make big on hover
	 public void pointerEnter() {
		 transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .2f).SetEase(Ease.InOutSine);
    }

	//Make small on hover
    public void pointerLeave() {
		transform.DOScale(new Vector3(1, 1, 1), .2f).SetEase(Ease.InOutSine);
    }

	//Animate towards assigned grid position
	public void animateIn(Vector3 destination, int speed) {
		Sequence inSequence = DOTween.Sequence();
		inSequence.Append(transform.DOJump(destination, 5, 1, cardSpeed).SetEase(Ease.InOutSine));
		inSequence.Insert(1f, transform.DOPunchPosition(new Vector3(20, 20, 20), .2f, 5, 1));
		inSequence.Insert(1f, transform.DOPunchRotation(new Vector3(7, 7, 7), .15f, 5, 1));
	}

	//Animate out of view
	public void animateOut() {
		Sequence outSequence = DOTween.Sequence();
		outSequence.Append(transform.DOJump(new Vector3(300, 0, 10), 5, 1, 1.6f).SetEase(Ease.InOutSine));
		outSequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), .15f).SetEase(Ease.InOutSine));
	}

	//Animate card depending on success/fail
	public IEnumerator animateResult(bool correct) {
		right = correct;
		gameObject.GetComponent<Image>().raycastTarget = false;
		Sequence highlight = DOTween.Sequence();
		highlight.Append(transform.DOJump(new Vector3(0, 0, 10), 5, 1, .7f).SetEase(Ease.InOutSine));
		highlight.Insert(0, gameObject.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), .7f).SetEase(Ease.InOutSine));
		highlight.Insert(0, transform.DORotate(new Vector3(0, -180, 0), .7f).SetEase(Ease.InOutSine));
		highlight.InsertCallback(.35f, switchSide);
		yield return new WaitForSeconds(.7f);
		if(correct) {
			Sequence right = DOTween.Sequence();
			right.Append(transform.DOJump(new Vector3(0, 0, 10), 3, 1, .8f).SetEase(Ease.OutBack));
			right.Insert(0, transform.DOPunchRotation(new Vector3(5, 5, 5), .5f, 1, 1).SetEase(Ease.InOutSine));
			right.Insert(.2f, transform.DOPunchScale(new Vector3(1.001f, 1.001f, 1.001f), .35f, 1, .5f).SetEase(Ease.InOutSine));
			right.InsertCallback(2.5f, animateOut);
			yield return new WaitForSeconds(2f);
			gameManager.UIManager.animateBar(true, gameObject);
		}
		else {
			Sequence wrong = DOTween.Sequence();
			wrong.Append(transform.DOPunchRotation(new Vector3(4, 4, 4), 1, 10, 1).SetEase(Ease.OutBack));
			wrong.Insert(0, transform.DOPunchPosition(new Vector3(4, 4, 4), 1, 10, 1).SetEase(Ease.OutBack));
			wrong.InsertCallback(2.5f, animateOut);
			yield return new WaitForSeconds(2f);
			gameManager.UIManager.animateBar(false, gameObject);
		}
		yield return null;
	}

    //Hide text and image and show result when "flipping" card
	public void switchSide() {
		foreach(Transform child in transform) {
			child.gameObject.SetActive(false);
		}
		transform.Find("ResultBackground").gameObject.SetActive(true);
		if(right) {
			transform.Find("CorrectImage").gameObject.SetActive(true);
		}
		else {
			transform.Find("IncorrectImage").gameObject.SetActive(true);
		}
	}

	Color32 AverageColorFromTexture(Texture2D tex)
	{
        Color32[] texColors = tex.GetPixels32();

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for(int i = 0; i < total; i++)
        {

            r += texColors[i].r;

            g += texColors[i].g;

            b += texColors[i].b;

        }

        return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total), 255);

	}

	float map(float s, float a1, float a2, float b1, float b2)
{
    return b1 + (s-a1)*(b2-b1)/(a2-a1);
}
}
