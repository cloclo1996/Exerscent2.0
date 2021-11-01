using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;



public class ImageHandler : MonoBehaviour {
	
	//Variables used to handle image names and scent types
	private string scentName="";   
	public string imgName;               //Example: orange, pine, broccoli
	public string scentType="";          //Example: fruit, plant, vegetable
	private string correctScentImg="";   //Used to compare strings

	public GameObject gameSystemObj;     //Game system object

	public Sprite[] fruitImgArray;       //Sprite Array of fruit images - assigned in Inspector window
	public Sprite[] vegetableImgArray;   //Sprite Array of fruit images - assigned in Inspector window
	public Sprite[] plantImgArray;       //Sprite Array of fruit images - assigned in Inspector window

	public Sprite[] allImgArray;                                //Sprite Array of ALL images - automatically generated in code
	public List<Sprite> allScentsList = new List<Sprite>();     //List of all scents - to enable dynamic removal of objects
	public List<Sprite> alternativesList = new List<Sprite>();  //List of alternatives generated based on category
	public Sprite[] alternativeSpriteArray;                     //Sprite Array of alternatives
	Sprite correctSprite;                                       //Variable to store correct sprite
	Sprite tempSprite;

	public int gameType;     //1: Explore  2:Quiz  3:Currently not defined
	public int arrayIndex;   //integer for looping through
	public int randSeed;     //Variable to store random seed number
	public int difficultyLevel=2;  //Difficulty level for quiz - only one currently existing
	public int uniqueItemIdCount;  //Variable to store number of elements in arraylist
	public int tryNbr = 0;    //REMOVE???
	public int correctPosition;     //REMOVE???

	//UI Elements
	public GameObject UICanvasObject;    //UI Canvas for UI components
	public Image largeImgComponent;      //UI element
	public Text bigScentTitle;           //UI Element
	public Button[] imgBtnArray;         //Image buttons for quiz mode
	private Button tempBtn;
	private Image theImage;
	public Text scoreText;
	public GameObject UIexplore;         //Parent object for Explore mode UI objects
	public GameObject UIquiz;            //Parent object for Quiz mode UI objects
	public GameObject scoreUI;           //UI score object


	//Input 
	public GameObject introObj;
	public Button userLogin;
	public Button createNewLogin;
	public GameObject inputNameObj;
	public Button inputNameDoneButton;
	private bool newLogin;        //User input
	private bool firstScorePost=false; //First score with new user
	private bool newUser = false; //Database Checked
	private bool exists;


	//Menu
	public Button exploreBtn;
	public Button quizBtn;
	public GameObject startMenu;
	public GameObject instructImg;

	//Quiz Mode variables
	public int totalScents;        //Total number of scents
	public int randomCorrect;      //Variable for random array index nbr
	private int randomSprite;      //Variable for randomized sprite for quiz alternatives
	public int[] shuffledIntArray; //Array to shuffle alternatives and use first indexes in loop
	public int nbrOfAlternatives = 6;  //Number of quiz alternatives
	private int maximum;               //Maximum integer value to randomly generate
	private int totalScore;            //To save total score
	public bool notClicked=true;       //Boolean for programming logic
	public int counter = 0;            //Counter used as array index in shuffled array
	public bool notAssigned = true;    //Checks if the correct scent index has been added to alternative array
	public GameObject quizDescriptionText;  //UI object

	//Explore Mode variables
	public GameObject exploreDescriptionText; //UI object
	public GameObject introText;              //UI object
	public GameObject flaskInstructionText;   //UI object

	//DatabaseScore
	public GameObject dataHandlingObj;        //Database handler object used to reach database handling script
	public string playerName="ann";           //Variable to store user name - will later be input based


	void Start () {

		largeImgComponent = UICanvasObject.GetComponent<Image>(); //Our image component is the one attached to this gameObject
		addAllScents();                       //Add all public arrays(assigned through inspector) into one array

		totalScents = allImgArray.Length;     //Count total amount of available scents
		shuffledIntArray = new int[totalScents]; //Create array of all scents for later shuffle

		totalScore = 0;                          //Assign total score to 0
		string tempText=totalScore.ToString ();  //Convert int to string
		scoreText.text = tempText;               //Add converted string to UI Text object

		//Activate the correct UI game objects 
		UIquiz.SetActive(false);
		scoreUI.SetActive(false);
		UIexplore.SetActive(false);

		//Show UI buttons
		//initMenuButtons ();
		initLoginUI ();
		inputNameDoneButton.onClick.AddListener (TaskOnClick);


		//Activate the correct UI game objects 
		exploreDescriptionText.SetActive (false);
		quizDescriptionText.SetActive (false);
		introText.SetActive (true);
		flaskInstructionText.SetActive (false);
	}
	
	void Update () {

		//Keys used for testing only
		if (Input.GetKeyUp(KeyCode.E))
		{
			setScentName("lemon", "fruit");
		}

		if (Input.GetKeyUp(KeyCode.R))
		{
			setScentName("mushroom", "plant");
		}
		if (Input.GetKeyUp(KeyCode.T))
		{
			setScentName("rose", "plant");
		}

	}


	//Function to show login UI 
	public void initLoginUI (){
		//Show user login
		introObj.SetActive(true);
		createNewLogin.onClick.AddListener (TaskOnClick);
		userLogin.onClick.AddListener (TaskOnClick);

    }



	//Function to show menu buttons
	public void initMenuButtons ()
	{
		introObj.SetActive(false);
		startMenu.SetActive (true);
		exploreBtn.onClick.AddListener (TaskOnClick);
		quizBtn.onClick.AddListener (TaskOnClick);

	}


	//Uses scent name and type to pass to explore(1) or quiz(2) mode 
	public void setScentName(string _scentName, string _scentType){

		instructImg.SetActive (false);

		scentName = _scentName;
		scentType = _scentType;

		//checkWhatGameType
		if(gameType==1){
			
			//Exploration mode

			//UI settings
			UIquiz.SetActive(false);
			scoreUI.SetActive(false);
			UIexplore.SetActive(true);
			exploreDescriptionText.SetActive (true);
			quizDescriptionText.SetActive (false);
			introText.SetActive (false);
			flaskInstructionText.SetActive (false);

			//Function used for exploration mode
			displayOneImg(scentName, scentType);
		}
		else if(gameType==2){
			
			//Quiz mode

			//UI settings
			UIexplore.SetActive(false);
			UIquiz.SetActive(true);
			scoreUI.SetActive(true);

			notClicked=true; 

			//Function used for quiz mode
			displayAlternatives(scentName, scentType);

			//UI settings
			exploreDescriptionText.SetActive (false);
			quizDescriptionText.SetActive (true);
			introText.SetActive (false);
			flaskInstructionText.SetActive (false);

		}
		else if(gameType==3){
			//Matching game

		}
	}
		

	private void addAllScents(){
		
		//Add all arrays into one list
		allScentsList.AddRange(fruitImgArray);
		allScentsList.AddRange(vegetableImgArray);
		allScentsList.AddRange(plantImgArray);

		allImgArray = allScentsList.ToArray();

	}


	//This function is triggered when the user clicks an UI button
	public void TaskOnClick ()
	{
		//print ("Clicked!");

		string tempBtnName = EventSystem.current.currentSelectedGameObject.name; //Get name of clicked button


		if (tempBtnName == "OldLogin") {
			inputNameObj.SetActive (true);
			introObj.SetActive(false);
			newLogin = false;


			//initMenuButtons ();

		} else if (tempBtnName == "NewLogin") {
			inputNameObj.SetActive (true);
			introObj.SetActive(false);
			newLogin = true;


		} else if (tempBtnName == "Done") {
			//Save name to user name variable
			StartCoroutine (dataHandlingObj.GetComponent<databaseHandler> ().CheckIfUserExists(playerName));  //Post data to online MySQL database

		

		}

		//Select game type(1 or 2) based on name of clicked button
		if (tempBtnName == "menuBtn1") {
			
			gameType = 1; //Exploration mode selected

			//UI settings
			startMenu.SetActive (false);
			instructImg.SetActive (true);
			flaskInstructionText.SetActive (true);
			introText.SetActive (false);


		} else if (tempBtnName == "menuBtn2") {

			gameType = 2; //Quiz mode selected

			//UI settings
			startMenu.SetActive (false);
			instructImg.SetActive (true);
			flaskInstructionText.SetActive (true);
			introText.SetActive (false);


		} else if(tempBtnName == "btnStart"){


		}
		
		else{

			//If in Quiz mode, the scent alternative buttons are handled here

			GameObject selectedBtn = EventSystem.current.currentSelectedGameObject; //Get selected button gameobject reference
			Image selectedImg = selectedBtn.GetComponent<Image> ();                 //Get selected button gameobject Image component
			string selectedImgName = selectedImg.sprite.name;                       //Get selected button gameobject Image component sprite name

			//print ("?: " + EventSystem.current.currentSelectedGameObject.name);     //Print the name for bug testing
			//print ("Correct: " + correctScentImg + " Selected: " + selectedImgName);  //Print the correct scent and selected name for bug testing

			//Check if selected image scent name is the same as correct scent name
			if (correctScentImg == selectedImgName && notClicked) {
				//print ("Answer correct!");

				//gameSystemObj.GetComponent<gameSystemLogic> ().addPoint (); //Add one point to other script score variable
				totalScore = gameSystemObj.GetComponent<gameSystemLogic> ().getTotalScore (); //Add to local score variable




				if (firstScorePost&&newUser) {
					StartCoroutine (dataHandlingObj.GetComponent<databaseHandler> ().PostScores (playerName, totalScore));  //Post data to online MySQL database
					firstScorePost=false;
					newUser = false;
				} else if(!newUser&&!firstScorePost){
					StartCoroutine (dataHandlingObj.GetComponent<databaseHandler> ().UpdateScores (playerName, totalScore));  //Post data to online MySQL database

				}



				string tempString = totalScore.ToString (); //Convert integer score to string
				scoreText.text = tempString;    //Show string score in UI Text object
				notClicked = false;
			} else {
				print ("Not correct...");
				notClicked = false;


				if (firstScorePost&&newUser) {

					totalScore = gameSystemObj.GetComponent<gameSystemLogic> ().getTotalScore (); //Add to local score variable

					StartCoroutine (dataHandlingObj.GetComponent<databaseHandler> ().PostScores (playerName, totalScore));  //Post data to online MySQL database
					firstScorePost=false;
					newUser = false;
				} 

			}

			// It might be a good idea to add the 'number of trials' to the database here, inspired by the updateScores function above
			// 1.Copy the php file, rename it and change the variable names 2.Copy the updateScores method in the databasehandler script - rename it+variables - done!

		}
	}

	public void userFound(bool _exists){
		exists = _exists;

		//print ("checking login type and if user exists - for UI Exists: "+exists);

		//Checks if this is a new login
		if(newLogin){
			
			if (exists) {
				
				//Further dev: This should activate the input UI, along with a message to the user
				//print ("This user already exists. Please choose another nickname or log in if you are already registered.");

			} else {
				inputNameObj.SetActive (false);      //Remove input UI
				initMenuButtons ();                  //Show Menu buttons
				newUser = true;                      //Keep track of this being a new user
				firstScorePost = true;               //Keep track of fact that this is the first score post - should create new database record later
			}


		}else if(newLogin==false){
			

			if (exists) {
				//Add or make user redo - give feedback
				newUser=false;                     //Keep track of this NOT being a new user
				firstScorePost = false;            //Keep track of fact that this is NOT the first score post - should NOT create new database record later
				inputNameObj.SetActive (false);    //Remove input UI
				initMenuButtons ();                //Show Menu buttons

			} else {
				//Further dev: This should activate the input UI, along with a message to the user
				//print ("This user does not exist. Please type your name again, or create a new user.");

			}

		}

	}


	public string getScentName(){

		return scentName;
	}




	//Function for Explore mode (1)
	public void displayOneImg(string _scentName, string _scentType){

		scentType = _scentType;
		scentName = _scentName;

		imgName = "img";
		string newWord = imgName+=scentName;   //adding 'img' as prefix to scanned scent name
		correctScentImg = newWord;             //storing string for later comparison with image names

	
		//Function for checking scent type, and then checking against all names in array type
		if (scentType == "fruit") {

			//Looping through all fruit images
			for (int i = 0; i < fruitImgArray.Length; i++) {
				string currentScent;
				currentScent = fruitImgArray [i].name;
				currentScent.ToLower ();   //make string lower case for comparison
				print (newWord + " " + currentScent);


				//If statement for bug testing
				if (string.Compare (newWord, currentScent) == 0) {
					print ("The same");	
				} else {
					print ("Noooo");	
				}


				if (newWord == currentScent) {
					print ("Same name!");
					print (newWord + " " + currentScent);

					setFruitImage (i);
				} else {

				}
			}

		//Function for checking scent type, and then checking against all names in array type
		} else if (scentType == "vegetable") {

			//Looping through all vegetable images
			for (int j = 0; j < vegetableImgArray.Length; j++) {
				string currentScent;
				currentScent = vegetableImgArray [j].name;
				currentScent.ToLower ();     //make string lower case for comparison
				print (newWord + " " + currentScent);

				//If statement for bug testing
				if (string.Compare (newWord, currentScent) == 0) {
					print ("The same yeees");	
				} else {
					print ("Noooo");	

				}


				if (newWord == currentScent) {
					print ("Same name!");
					print (newWord + " " + currentScent);

					setPlantImage (j);

				} else {

				}
			}
		//Function for checking scent type, and then checking against all names in array type
		} else if (scentType == "plant") {

			//Looping through all plant images
			for (int k = 0; k < plantImgArray.Length; k++) {
				string currentScent;
				currentScent = plantImgArray [k].name;
				currentScent.ToLower ();       //make string lower case for comparison
				print (newWord + " " + currentScent);

				//If statement for bug testing
				if (string.Compare (newWord, currentScent) == 0) {
					print ("The same yeees");	
				} else {
					print ("Noooo");	

				}

				if (newWord == currentScent) {
					print ("Same name!");
					print (newWord + " " + currentScent);

					setPlantImage (k);

				} else {

				}
			}
		}

		bigScentTitle.text = scentName.ToUpper ();

	}


	//Function for Quiz mode (2)
	public void displayAlternatives (string _scentName, string _scentType)
	{

		scentType = _scentType;
		scentName = _scentName;

		imgName = "img";
		string newWord = imgName += scentName;
		correctScentImg = newWord;


		counter = 0;   //REMOVE??
		notAssigned = true;
		allScentsList.Clear ();
		addAllScents ();

		//Assing current scent randmonly to array 
		//save current scent index for array


		//Placeholder for simpler level of difficulty - progression
		if (difficultyLevel == 1) {

			//   New array of alternatives (3)
			//   search all categories
			//   randomize into new array

		}

		if (difficultyLevel == 2) {


			alternativeSpriteArray = new Sprite[nbrOfAlternatives];


			//Unique random number generation
			int currSecond = System.DateTime.Now.Millisecond; 
			float currMousPosX = Input.mousePosition.x;
			randSeed = currSecond + (int)currMousPosX + tryNbr; //Unique random seed based on combo of time and mouse position
			Random.seed = randSeed;                             //Assigning the random seed
			randomCorrect = Random.Range (0, 5);                //Generating the random number

			//SHUFFLE ARRAY
			Sprite tempSprite;


			for (int o = 0; o < shuffledIntArray.Length; o++) {
				shuffledIntArray [o] = o;
			}


			ShuffleArray (shuffledIntArray);   //Shuffle array of integers

			for (int a = 0; a < nbrOfAlternatives; a++) {

				int nbr = shuffledIntArray [a];             //Loop through shuffled array to get random number

				tempSprite = (Sprite)allScentsList [nbr];   //Use received random number to select a sprite

				alternativeSpriteArray [a] = tempSprite;    //Add selected sprite to array of sprite alternatives

				tempBtn = imgBtnArray [a];                   //Assign current button[a] to tempBtn variable
				theImage = tempBtn.gameObject.GetComponent<Image> ();     //Assign selected buttons image component to Image variable
				theImage.sprite = alternativeSpriteArray [a];   //Assign the sprite from the array to the current button Image component


			}
				

			//Fetch the correct sprite, by name
			for (int x = 0; x < allScentsList.Count; x++) {
				if (correctScentImg == allScentsList [x].name) {
					correctSprite = (Sprite)allScentsList [x];     
				}
			}

			//If the array of sprite alternatives does not already contain the current sprite
			if (!alternativeSpriteArray.Contains (correctSprite)) {
				
				alternativeSpriteArray [randomCorrect] = correctSprite; //Add sprite to array of sprite alternatives
				theImage.sprite = alternativeSpriteArray [randomCorrect];  //Assign selected sprite to Image
			}

			for (int s = 0; s < imgBtnArray.Length; s++) {
				imgBtnArray [s].onClick.AddListener (TaskOnClick); //Tie buttons to TaskOnClick function
			}

			counter = 0;   //REMOVE??
			notAssigned = true;
			allScentsList.Clear ();
			addAllScents ();

		}

		//Placeholder for harder level of difficulty - progression
		if (difficultyLevel == 3) {


		}
	}
		


	public void setFruitImage (int _arrayIndex) //method to set our first image
	{
		arrayIndex = _arrayIndex;
		largeImgComponent.sprite = fruitImgArray [arrayIndex];
	}

	public void setVegetableImage (int _arrayIndex) //method to set our first image
	{
		arrayIndex = _arrayIndex;
		largeImgComponent.sprite = vegetableImgArray [arrayIndex];
	}


	public void setPlantImage (int _arrayIndex) //method to set our first image
	{
		arrayIndex = _arrayIndex;
		largeImgComponent.sprite = plantImgArray [arrayIndex];
	}


	public void setPlayerName(string _playerName){
		playerName = _playerName;
	}


	//M that allows you to pass an array to shuffle it
	public static void ShuffleArray<T> (T[] arr)
	{
		for (int i = arr.Length - 1; i > 0; i--) {
			int r = Random.Range (0, i);
			T tmp = arr [i];
			arr [i] = arr [r];
			arr [r] = tmp;
		}
	}

public static List<GameObject> Fisher_Yates_CardDeck_Shuffle (List<GameObject>aList) {
 
         System.Random _random = new System.Random ();
 
         GameObject myGO;
 
         int n = aList.Count;
         for (int i = 0; i < n; i++)
         {
             // NextDouble returns a random number between 0 and 1.
             // ... It is equivalent to Math.random() in Java.
             int r = i + (int)(_random.NextDouble() * (n - i));
             myGO = aList[r];
             aList[r] = aList[i];
             aList[i] = myGO;
         }
 
         return aList;
     }
}




