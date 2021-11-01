using UnityEngine;
using System.Collections;

public class databaseHandler : MonoBehaviour
{
	//This code is partially based on the example at: http://wiki.unity3d.com/index.php?title=Server_Side_Highscores

	private string saveNewDataURL = "http://by.biip.cc/nosewise/saveDataNew.php?"; //Create a completely new record in database
	private string updateScoreURL="http://by.biip.cc/nosewise/updateScore.php?";   //Update an existing record
	private string getScoreURL="http://by.biip.cc/nosewise/getData.php";           //Get the current score
	private string checkIfUserExistsURL="http://by.biip.cc/nosewise/checkIfNameExists.php?";

	string post_url;      //Just a variable to store full URLs
	public string score;   //Score string
	public bool userExists=false;   //boolean to store if user exists
	//public GameObject imgHandlerObj;     //Reference to external game object
	public gameSystemLogic gameManager;

	void Start()
	{
		//For bug testing

		// StartCoroutine(PostScores("BONNIE", 91));
		// StartCoroutine(GetScores());
		// StartCoroutine(CheckIfUserExists("CLYDE"));
	}

	// remember to use StartCoroutine when calling this function!
	// Pass along two variables to create a record for the user
	public IEnumerator PostScores(string name, int score)
	{
		post_url = saveNewDataURL + "name=" + WWW.EscapeURL(name) + "&score=" + score; //Add name and score variables to URL
		//print (post_url);     

		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);   //Use URL with WWW class 
		yield return hs_post;              // Wait until the download is done

		if (hs_post.error != null)
		{
			print("There was an error posting the score: " + hs_post.error);
		}
	}

	// remember to use StartCoroutine when calling this function!
	// Get scores from database and print in console(can later be used to fetch total score and display it) 
	public IEnumerator GetScores()
	{
		// Post the URL to the site and create a download object to get the result
		WWW hs_get = new WWW(getScoreURL);  //Use URL with WWW class 
		yield return hs_get;                //Wait(yield) until download is finished
	 	print("progress: "+hs_get.progress);

		if (hs_get.error != null)
		{
			print("There was an error getting the high score: " + hs_get.error);
			print ("score: "+hs_get.text);
		}
		else
		{
			print(hs_get.text);
			score = hs_get.text;
		
		}
	}

	// remember to use StartCoroutine when calling this function!
	// Pass along to variables to update the score for the correct user name
	public IEnumerator UpdateScores(string name, int score){

		post_url = updateScoreURL + "name=" + WWW.EscapeURL(name) + "&totalScore=" + score; //Add name and score variables to URL
		//print (post_url);

		// Post the URL to the site and create a download object to get the result
		WWW hs_post = new WWW(post_url);  //Use URL with WWW class 
		yield return hs_post; // Wait until the download is done

		if (hs_post.error != null)
		{
			print("There was an error posting the high score: " + hs_post.error);
		}
	}

	// remember to use StartCoroutine when calling this function!
	// Pass along name variable to search through database for name
	public IEnumerator CheckIfUserExists(string name){

		post_url = checkIfUserExistsURL + "name=" + WWW.EscapeURL(name); 
		//print (post_url);

		// Post the URL to the site and create a download object to get the result.
		WWW hs_get = new WWW(post_url);  //Use URL with WWW class 
		yield return hs_get;  // Wait until the download is done

		if (hs_get.error != null)
		{
			print("There was an error checking for user name: " + hs_get.error);
		}
		//print(hs_get.text);

		string myText=hs_get.text.ToString();   //Make sure the text is treated like a string
		string trimmed = myText.Trim ();        //Make sure the text does not have spaces

		//The php script returns yes if the name has been found, and no if it hasn't
		//Check if string equals "yes" or "no"
		if(trimmed.Equals("yes")){
			userExists = true;
			//imgHandlerObj.GetComponent<ImageHandler> ().userFound (userExists);   //Let main script know if user was found
			//print ("Inside if: exists? "+trimmed); 
			Debug.Log("Exists");
		}
		else if(trimmed.Equals("no")){
			userExists = false;
			//imgHandlerObj.GetComponent<ImageHandler> ().userFound (userExists);   //Let main script know if user was found
			//print ("Inside if: exists? "+trimmed);
			Debug.Log("Does not exist");
		}
	}
}