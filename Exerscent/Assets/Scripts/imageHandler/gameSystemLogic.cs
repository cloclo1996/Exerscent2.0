using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

[System.Serializable]

//Scent data struct
public struct scentData {
		public Sprite scentSprite;
		public ScentCategory category;
		public string name;
		public Color cardColor;
		public Sprite scentPicture;
	}
	
	[System.Serializable]
	//Attempt result struct
	public struct resultSet {
		public string correct;
		public string attempt;
		public string correctScent;
		public List<string> options;
	}
public class gameSystemLogic : MonoBehaviour {
	public List<resultSet> allResults = new List<resultSet>();
	resultSet currentResult;
	public int totalScore = 0;
	//Total number of failed attempts
	public int totalFails = 0;
	public int totalAttempts = 0;
	public bool customLength = false;
	public int gameLength = 0; 
	public bool gameRunning = false;
	public bool gridSelected = false;
	public bool lengthSelected = false;
	string elapsedTime;
	//Size of scent grid
	public Vector2 gridSize = new Vector2(0, 0);
	//Scent grid spacing
	private Vector2 gridSpacing;
	//List of current active scent objects
	public List<GameObject> currentScents = new List<GameObject>();
	public List<GameObject> unusedCards = new List<GameObject>();
	public List<GameObject> usedCards = new List<GameObject>();
	public List<string> usedScents = new List<string>();
	//List of all scents to use in game
	public List<scentData> totalScents = new List<scentData>();
	//Template prefab to use for spawning new scents
	public GameObject scentTemplate;
	public GameObject selectedCard;
	//The current correct scent
	public string correctScent;
	public GameObject rightScent;
	//Reference to UI manager
	public UIManager UIManager;
	//Firebase reference
	public FirebaseManager FirebaseManager;
	//ScentBehaviour reference
	public ScentBehaviour scentBehaviour;
	//Data handler
	public GameObject dataHandlingObj;     
	//Player id
	public string playerName = "";
	bool newGame = true;
	public RectTransform scentGridRect;
	public GridLayoutGroup gridLayout;

	public SerialCom SerialCom;


	public void Start () {
        //Set game length on start
		setLength(GameObject.Find("LengthInput"));
		scentGridRect = GameObject.Find("ScentGrid").GetComponent<RectTransform>();
		gridLayout = GameObject.Find("ScentGrid").GetComponent<GridLayoutGroup>();
		scentBehaviour = GameObject.FindObjectOfType<ScentBehaviour>();
	}
	
	public void Update () {
		//Fake scents and login with keyboard for debug purposes
		// if (Input.GetKeyUp(KeyCode.W))
		// {
		// 	switchScent("");
		// }

		if (Input.GetKeyDown(KeyCode.K) && Input.GetKey(KeyCode.LeftControl))
		{	
			Debug.Log("Console toggled.");
			UIManager.toggleConsole();
		}

		// if (Input.GetKeyDown(KeyCode.O))
		// {
		// 	UIManager.consoleMessage("This is a test message");
		// }

		if(Input.GetKeyDown(KeyCode.Return)) {
			setName();
			UIManager.continueScript();
		}

		//if(gameRunning){
			if (Input.GetKeyUp(KeyCode.R))
			{
				switchScent("Lemon");
			}	
		// 	if (Input.GetKeyUp(KeyCode.T))
		// 	{
		// 		switchScent("Apple");
		// 	}
		// }
		// if(Input.GetKeyDown(KeyCode.Space)) {
		// 	playerName = "Testplayer";
		// 	UIManager.updateUIState(UIState.welcome);
		// 	gameRunning = true;
		// }

		// if(Input.GetKeyDown(KeyCode.L)) {
		// 	UIManager.updateUIState(UIState.selectGame);
		// 	playerName = "Martin_Test";
		// 	UIManager.admin.SetActive(true);
		// 	// scentBehaviour.debugFunc();
		// 	Debug.Log("This keycode is working");

		// }
		
		// if(Input.GetKeyDown(KeyCode.K)) {
		// 	//Run function that Debug.log Firebase data
		// 	exportFirebaseData();
		// }
		// if(Input.GetKeyDown(KeyCode.O)){
		// 	// restart();
		// 	Debug.Log(gridSize);
		// 	Debug.Log("This keycode is working");
		// }
	}

	//Start a new round
	public IEnumerator newRound() {
		if(newGame) {
			// UIManager.updateUIState(UIState.welcome);
			elapsedTime = "";
			newGame = false;
			gameRunning = true;
		}
		
		List <scentData> tempScents = new List<scentData>(totalScents);
		tempScents = tempScents.OrderBy(x => Random.value).ToList();
		List <scentData> newScents = new List<scentData>();
		//Find correct scent and add it to the list of new scents
		for (int i = 0; i < tempScents.Count; i++) {
			if(tempScents[i].name.ToLower() == correctScent.ToLower()) {
				newScents.Add(tempScents[i]);
				tempScents.RemoveAt(i);
			}
		}

		//Add more random scents to fill out the grid
		for(int i = 0; i < (gridSize.x * gridSize.y) - 1; i++) {
			newScents.Add(tempScents[i]);
		}

		//Shuffle the list of new scents
		newScents = newScents.OrderBy(x => Random.value).ToList();
		//Generate scent UI objects based on new scent data and place them in a grid
		int listIndex = 0;
		 for(int x = 0; x < gridSize.x; x ++) {
			for(int y = 0; y < gridSize.y; y ++) {
				GameObject newScent = Instantiate(scentTemplate, new Vector3(0, 0, 0), Quaternion.identity);
				newScent.transform.SetParent(GameObject.Find("ScentParent").transform, true);
				newScent.transform.localScale = new Vector3(1, 1, 1);
				newScent.transform.position = new Vector3(-300, 0, 0);
				newScent.GetComponentInChildren<ScentBehaviour>().setData(newScents[listIndex]);
				currentScents.Add(newScent);
				GameObject dummy = UIManager.scentsParent.transform.GetChild(listIndex).gameObject;
				newScent.GetComponentInChildren<ScentBehaviour>().animateIn(dummy.transform.position, listIndex);
				listIndex ++;
				yield return new WaitForSeconds(.15f);
			}
		}  
	}

	//Remove old scents and trigger their exit animations
	public IEnumerator destroyScents(bool correct) {
		foreach (GameObject data in currentScents.ToList()) {
			data.GetComponentInChildren<ScentBehaviour>().animateOut();
			yield return new WaitForSeconds(.15f);
			Destroy(data);
			currentScents.Remove(data);
		}	
		StartCoroutine(selectedCard.GetComponentInChildren<ScentBehaviour>().animateResult(correct));
		//Return to waiting state if game is not finished
		yield return new WaitForSeconds(3.5f);
		if(gameRunning) {
			UIManager.updateUIState(UIState.waitingForScent);
		}
		yield return null;
	}

	//Remove old scents cards, to be used with restart functions
	public IEnumerator resetCards() {
		foreach (GameObject data in currentScents.ToList()) {
			data.GetComponentInChildren<ScentBehaviour>().animateOut();
			yield return new WaitForSeconds(.15f);
			Destroy(data);
			currentScents.Remove(data);
		}	
		yield return null;
	}

    //Not in use, can probably delete
	public int getTotalScore() {
		return totalScore;
	}

	//Check if new attempt is correct
	public void checkAttempt() {
		currentResult = new resultSet();
		GameObject scentObject;
		string attempt;
		bool sent = false;

		currentResult.options = new List<string>();
		foreach(GameObject scentThing in currentScents) {
			currentResult.options.Add(scentThing.GetComponentInChildren<ScentBehaviour>().thisData.name.ToLower());
		}
        
		foreach(GameObject scentCard in currentScents) {
			if(scentCard.GetComponentInChildren<ScentBehaviour>().selected) {
				scentObject = scentCard;
				attempt = scentObject.GetComponentInChildren<ScentBehaviour>().thisData.name;
				currentResult.attempt = attempt.ToLower();
				currentResult.correctScent = correctScent.ToLower();
				usedScents.Add(correctScent);
				currentScents.Remove(scentCard);
				selectedCard = scentCard;
        //Do if correct
		if(attempt.ToLower() == correctScent.ToLower()) {
			currentResult.correct = "YES";
			allResults.Add(currentResult);
			totalScore ++;
			totalAttempts ++;
			UIManager.scoreNumber.text = totalScore.ToString();
			UIManager.focusCard(scentObject, true);
			StartCoroutine(destroyScents(true));
		}
        //Do if incorrect
		else {
			currentResult.correct = "NO";
			allResults.Add(currentResult);
			totalFails ++;
			totalAttempts ++;
			UIManager.focusCard(scentObject, false);
			StartCoroutine(destroyScents(false));
		}
		correctScent = "";
		//End game if all scents have been attempted or game length has been reached
		if(customLength && totalAttempts >= gameLength) {
			endGame();
		}
		if(!customLength && totalAttempts >= totalScents.Count) {
			endGame();
		}
				break;
			}
		}
	}

    //Add round results to list of all results
	public void storeResult(string attempt, string correct) {
		resultSet newResult = new resultSet();
		newResult.correct = correct;
		newResult.attempt = attempt.ToLower();
		newResult.correctScent = correctScent.ToLower();
		List <string> remaining = new List<string>();
		foreach(GameObject scentObject in currentScents) {
			newResult.options.Add(scentObject.GetComponentInChildren<ScentBehaviour>().name.ToLower());
		}
		allResults.Add(newResult);
	}

	//Check in database to see if user exists
	public void attemptLogin(string name) {
		playerName = name;
		UIManager.updateUIState(UIState.welcome);
	}

	//Switch scents
	public void switchScent(string scentName) {
		if(!gameRunning && !UIManager.menuOpen) {
			gameRunning = true;
		}
        //Check if UI is in correct state
		if(gameRunning && UIManager.currentState != UIState.waitingForAttempt && UIManager.currentState != UIState.endGame) {
			


			//Check if scent has already been used
			foreach(string usedScent in usedScents) {
				if(usedScent == scentName) {
					StartCoroutine(UIManager.switchInfoText("This scent has already been handled in this session. Please pick out a new scent not yet used in this session.", true));
					return;
				}
			}

            //Accept new scent if not identical to old one
			if(scentName.ToLower() != correctScent.ToLower()) {

				correctScent = scentName.ToLower();
				StartCoroutine(newRound());
				Debug.Log("switchScent recieved the variable " + scentName);
				UIManager.updateUIState(UIState.waitingForAttempt);
			}

		}
	}

	//End game and send generated data to database
	public void endGame() {
		gameRunning = false;
		UIManager.updateUIState(UIState.endGame);
		FirebaseManager.writeResults(playerName, totalScore, gameLength, elapsedTime, allResults);
	}

	//Reset all game data on restart
	public void restart () {
		gameRunning = false;
		StartCoroutine(resetCards());
		clearData();
		newGame = true;
	}

	public void playAgain(){
		gameRunning = false;
		clearData();
		newGame = true;
	}
	
	public void clearData (){
		//Clears all the data that the user have saved through gameplay back to it's original value.
		currentScents = new List<GameObject>();
		unusedCards = new List<GameObject>();
		usedCards = new List<GameObject>();
		allResults = new List<resultSet>();
		usedScents = new List<string>();
		totalScore = 0;
		totalFails = 0;
		totalAttempts = 0;
		correctScent = "";
	}

    //Quit
	public void quitGame() {
		Application.Quit();
	}

	//Export Firebase data
	public void exportFirebaseData(){
		FirebaseManager.fetchData("Testplayer");
	} 


    //Set custom game length
	public void setLength(GameObject caller) {
		Debug.Log("change!");
		gameLength = int.Parse(caller.GetComponent<TMP_InputField>().text);
		
		if(gameLength > 0)
		{
			UIManager.changeColourRed(caller.transform.GetChild(0).Find("Text").gameObject);
			lengthSelected = true;
			
			if(gridSelected)
			{
				//Change colour of Continue
				UIManager.changeColourRed(UIManager.continueBTN);
			}

		} else if (gameLength == 0) {

			UIManager.changeColourBlue(caller.transform.GetChild(0).Find("Text").gameObject);
			lengthSelected = false;

			if(gridSelected)
			{
				//Change colour of Continue
				UIManager.changeColourBlue(UIManager.continueBTN);
			}
		}
		//gameLength = int.Parse(entry.text);
	}

	public void setName ()
	{
		playerName = UIManager.nameInput.GetComponent<TMP_InputField>().text;

		if(playerName != "")
		{
			UIManager.changeColourRed(UIManager.continueLoginBTN);
		} else if (playerName == "")
		{
				UIManager.changeColourBlue(UIManager.continueLoginBTN);
		}

	}

	public void setCardSpeed(GameObject caller){
		scentBehaviour.cardSpeed = float.Parse(caller.GetComponent<TMP_InputField>().text);
	}

	public void tenOptions(){
		//Changes the gridSize to 5, allowing 10 options to appear
			gridSize.x = 5;
			gridSize.y = 2;
			//Sets to two rows instead of one
			gridLayout.constraintCount = 2;
			Debug.Log("Changed to 10 optins per smell.");

			//Changes the starting point of the grid, fitting all the cards onto the screen
			scentGridRect.localPosition = new Vector3(-662, -63, 0);

			//Changes the padding between the cards, fitting all the cards
			gridLayout.spacing = new Vector2(80, 160);
	}

	public void sixOptions(){
		//Changes the gridSize to 3, allowing 6 options to appear
			gridSize.x = 3;
			gridSize.y = 2;
			gridLayout.constraintCount = 2;
			//Set to two rows instead of one
			Debug.Log("Changed to 6 options per smell.");

			//Changes the starting point of the grid, original value
			scentGridRect.localPosition = new Vector3(-331, -63, 0);

			//Changes the padding between the cards, original value
			gridLayout.spacing = new Vector2(60, 160);
	}

	public void twoOptions(){
		//Changes the gridSize to 1, allowing 2 options to appear
			gridSize.x = 2;
			gridSize.y = 1;
			//Set to one row instead of two
			gridLayout.constraintCount = 1;
			Debug.Log("Changed to 2 options per smell.");

			//Changes the starting point of the grid, original value
			scentGridRect.localPosition = new Vector3(-360, -63, 0);

			//Changes the padding between the cards, original value
			gridLayout.spacing = new Vector2(300, 160);
	}

	//Record elapsed game time. Might be broken?
	public IEnumerator counter() {
		int seconds = 0;
		int minutes = 0;
		int hours = 0;
		while(gameRunning) {
			yield return new WaitForSeconds(1);
			seconds ++;
			if(seconds >= 60) {
				minutes ++;
				seconds = 0;
				if(minutes >= 60) {
					minutes = 0;
					hours ++;
				}
			}
		}
		elapsedTime = hours + ": " + minutes + ": " + seconds;
	}
}