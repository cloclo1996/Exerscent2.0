using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[System.Serializable]
//UI state enumerator
public enum UIState {
	waitingForArduino,
	loginMenu, 
	enterLogin,
	selectGame,
	welcome, 
	openMenu,
	closeMenu,
	waitingForScent, 
	waitingForAttempt, 
	endGame
	};

public enum menuState {
	open,
	close,
	about,
	restart,
	exit,
	settings,
	quit
};

//Language settings. NOT IN USE.
public enum languageSettings {
	english,
	swedish
}

public class UIManager : MonoBehaviour {
	public languageSettings language;
	public GameObject canvas;
	public Vector2 screenScaled;
	public float transitionSpeed = 0.35f; //reused variable to set transition speed to switch screens 
	public float menuSpeed = .5f; //reused variable used to set speed of text/UI animations 
	private bool consoleHidden = true;
	public bool endScreen1 = false;
	//References to UI elements (they are the same ones as in the editor)
	public GameObject title;
	public GameObject menuButton;
	public GameObject mainMenu;
	public GameObject realMainMenu;
	public GameObject menuBackground;
	public GameObject about;
	public GameObject admin;
    public GameObject restart;
	public GameObject exitSession;
	public GameObject settings;
	public GameObject quit;
	public GameObject aboutWindow;
	public GameObject adminWindow;
	public GameObject exitWindow;
	public GameObject settingsWindow;
	public GameObject quitWindow;
	public GameObject restartWindow;
	public GameObject consoleWindow;
	public GameObject errorWindow;
	public GameObject scentsParent;
	public GameObject welcomeScreen;
	public GameObject selectGameScreen;
	public GameObject logInScreen;
	public GameObject twoOptions;
	public GameObject fourOptions;
	public GameObject sixOptions;
	public GameObject tenOptions;
	public GameObject selectTwo;
	public GameObject selectFour;
	public GameObject selectSix;
	public GameObject selectTen;
	public GameObject firstMsg;
	public GameObject secondMsg;
	public GameObject thirdMsg;
	public GameObject fourthMsg;
	public GameObject quitText;
	public GameObject continueBTN;
	public GameObject continueLoginBTN;
	public GameObject playAgainButton;
	public GameObject nameInput;
	public TextMeshProUGUI infoText;
	public GameObject progressBar;
	public bool windowOpen = false;
	public Image background;
	public TextMeshProUGUI scoreNumber;
	public TextMeshProUGUI WelcomeText;


	public GameObject endScreen;
	//Game manager reference
	public gameSystemLogic manager;

	public SerialPorts SerialPorts;
	//Current state of the UI
	public UIState currentState;
	public menuState currentMenuState;
	public bool menuOpen = false;
	bool aboutOpen = false;
	bool adminOpen = false;
	bool settingsOpen = false;
	bool quitOpen = false;
	bool restartOpen = false;
	bool exitOpen = false;
	bool sizeSelected = true;
	
	// Use this for initialization
	void Start () {
		manager = GameObject.FindObjectOfType<gameSystemLogic>();
		updateUIState(UIState.waitingForArduino);
		DOTween.defaultEaseType = Ease.OutBack;
        //Set initial menu object positions
		admin.SetActive(false);
		aboutWindow.transform.localPosition = new Vector3(200, 700, 0);
		quitWindow.transform.localPosition = new Vector3(200, 700, 0);
		settingsWindow.transform.localPosition = new Vector3(200, 700, 0);
		adminWindow.transform.localPosition = new Vector3(200, 700, 0);
		restartWindow.transform.localPosition = new Vector3(200, 700, 0);
		exitWindow.transform.localPosition = new Vector3(200, 700, 0);
		screenScaled = new Vector2(
			canvas.GetComponent<RectTransform>().rect.width / Screen.width,
			canvas.GetComponent<RectTransform>().rect.height / Screen.height
		);
	}
	
	// Update is called once per frame
	void Update () {

        if (!manager.gameRunning)
        {
			restart.SetActive(false);
			exitSession.SetActive(false);
        }
        else
        {
			restart.SetActive(true);
			exitSession.SetActive(true);
		}
	}



//================================== GAME STATES ========================================									
// 		Here you will find all the screens that you can go through in the game.
//=======================================================================================

	public void updateUIState(UIState newState) {
		currentState = newState;
		Debug.Log(currentState.ToString());
		switch (newState) {
			case UIState.waitingForArduino:
				Sequence titleSequence = DOTween.Sequence();
				titleSequence.Append(title.GetComponentInChildren<Image>().DOFillAmount(1, 10f).SetEase(Ease.InOutSine));
				break;
			case UIState.enterLogin:
			Debug.Log("I am waiting for a login");
				StartCoroutine(enterWait());
				break;
				//The user selects between 2, 4, 6, 10 options per smell
			case UIState.selectGame:
				// StartCoroutine(selectGameAdmin());
				//exitSession.SetActive(false);
				//restart.SetActive(false);
				selectGameScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
				StartCoroutine(switchInfoText("", true));
				enterMain();	
				welcomeScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
				break;
                //Show welcome screen
			case UIState.welcome:
				welcomeScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
				title.GetComponentInChildren<Image>().DOFillAmount(0, .1f).SetEase(Ease.InSine);
				StartCoroutine(switchInfoText("V??lj doft, lukta p?? den och placera den p?? plattformen", true));
				enterMain();
				break;
                //Show prompt asking player to put a new scent on the reader
			case UIState.waitingForScent:
				welcomeScreen.transform.DOLocalMove(new Vector3(1500,0, 0), transitionSpeed);
				StartCoroutine(switchInfoText("V??lj doft, lukta p?? den och placera den p?? plattformen", true));
				progressBar.transform.DOLocalMove(new Vector3(0, 300, 0), transitionSpeed);
				break;
			case UIState.waitingForAttempt:
				welcomeScreen.transform.DOLocalMove(new Vector3(0,-Screen.height * 2, 0), transitionSpeed);
				StartCoroutine(switchInfoText("", false));
				progressBar.transform.DOLocalMove(new Vector3(0, 300, 0), transitionSpeed);
				break;
			case UIState.endGame:
				Debug.Log(manager.allResults.Count);
				Invoke("newEndGameScript", 4.5f);
				break;
			case UIState.openMenu:
				Sequence openSequence = DOTween.Sequence();
				break;
			case UIState.closeMenu:
				Sequence closeSequence = DOTween.Sequence();
				closeSequence.Insert(.3f, menuBackground.transform.DOLocalMove(new Vector3(250, 0, 0), .5f).SetEase(Ease.InBack));
				break;
			default:
				break;
		}
	}



//================================== PLAYER SCORING =====================================
// 	These are the different messages displayed depending on the user's score. To see
//			where and how the score is counted go to gameSystemLogic.cs.
//=======================================================================================
	public void endGameScript(){
		endScreen1 = true;
		float totalScore = manager.totalScore;
		float gameLength = manager.gameLength;
		float procent = (totalScore / gameLength) *100;
		endScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
		TextMeshProUGUI endScore = GameObject.Find("EndText").GetComponent<TextMeshProUGUI>();
		playAgainButton.transform.DOLocalMove(new Vector3(0, -290, 0), transitionSpeed);
		if(procent < 1){
			endScore.text = "You've completed the session. You should see a doctor. Your score was " + manager.totalScore + " out of " + manager.gameLength + ".";
		} else if(procent > 1 && procent < 33){
			endScore.text = "You've completed the session. Just getting started? Your score was " + manager.totalScore + " out of " + manager.gameLength + ".";
		} else if(procent > 33 && procent < 66){
			endScore.text = "You've completed the session. It seems like you could use a bit more practice. Your score was "+ manager.totalScore + " out of " + manager.gameLength + ".";
		} else if (procent > 66 && procent < 99) {
			endScore.text = "You've completed the session. A few more days and you'll get them all! Your score was "+ manager.totalScore + " out of " + manager.gameLength + ".";
		} else if (procent > 99){
			endScore.text = "You've completed the session. You're an expert! Your score was "+ manager.totalScore + " out of " + manager.gameLength + ".";
		}
		Debug.Log("The player scored: " + procent + "%.");
	}



//======================== END GAME SCREEN & ERROR MESSAGES==============================
//			    End game script to tally the points and error messages.			
//=======================================================================================
	public void newEndGameScript()
	{
        if (!manager.gameRunning)
        {
			endScreen1 = true;
			float totalScore = manager.totalScore;
			float gameLength = manager.gameLength;
			float procent = (totalScore / gameLength) *100;
			endScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
			TextMeshProUGUI endScore = GameObject.Find("EndText").GetComponent<TextMeshProUGUI>();
			playAgainButton.transform.DOLocalMove(new Vector3(0, -290, 0), transitionSpeed);
			endScore.text = "F??rdig! Din po??ng blev " + manager.totalScore + " av " + manager.gameLength + ".";
        }

	}

	public void showErrorMessage() //Used in SerialPorts.cs
	{
			errorWindow.transform.DOLocalMove(new Vector3(0, 82, 0), transitionSpeed);
			quitOpen = true;
			quitText.GetComponent<TextMeshProUGUI>().text = "Vill du avsluta?";
			Sequence quitSequence = DOTween.Sequence();
			quitSequence.Append(quitWindow.transform.DOLocalMove(new Vector3(0, -158, 0), menuSpeed)).SetEase(Ease.InOutSine);			
	}

	public void hideErrorMessage() //Used in SerialPorts.cs
	{
		errorWindow.transform.DOLocalMove(new Vector3(0, 1000, 0), transitionSpeed);
		quitOpen = false;
		quitText.GetComponent<TextMeshProUGUI>().text = "??r du s??ker p?? att du vill avsluta?";
		Sequence quitSequence = DOTween.Sequence();
		quitSequence.Append(quitWindow.transform.DOLocalMove(new Vector3(200, 1000, 0), menuSpeed)).SetEase(Ease.InOutSine);
	}



//============================ OPEN & CLOSE MENU ANIMATION ==============================
//					 This is used only when opening/closing the Menu					
//=======================================================================================

	public void updateMenuState(GameObject caller) {
		Debug.Log(caller.name);
		switch(caller.gameObject.name) {
			case "MenuButton":
				if(!menuOpen) {
					menuOpen = true;
					Debug.Log("opening");
					Sequence openSequence = DOTween.Sequence();
					//fade-in background when menu opens
					openSequence.Insert(0, background.DOFade(1, menuSpeed));
					//change menu position (about,restart,exit and quit text)
					openSequence.Append(mainMenu.transform.DOLocalMove(new Vector3(-570, 200, 0), menuSpeed)).SetEase(Ease.InOutSine);
					openSequence.Insert(0, menuButton.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f));
					menuButton.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
					background.raycastTarget = true;
				}
				else {
					menuOpen = false;
					Debug.Log("closing");
					hideAll();
					Sequence closeSequence = DOTween.Sequence();
					closeSequence.Append(mainMenu.transform.DOLocalMove(new Vector3(0-1400, 145, 0), menuSpeed)).SetEase(Ease.InSine);
					closeSequence.Insert(0, background.DOFade(0, menuSpeed));
					closeSequence.Insert(0, menuButton.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f));
					menuButton.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
					background.raycastTarget = false;
				}
				break;
		}
	}



//================================== LOADING BAR ========================================
//			Exerscent loading bar animation seen when you open the game.								
//=======================================================================================

	public IEnumerator enterWait() {
		consoleMessage("Login window activated");
		Sequence enterSequence = DOTween.Sequence();
		//loading bar progress
		enterSequence.Append(title.GetComponentInChildren<Image>().DOFillAmount(1, 1f).SetEase(Ease.InSine));
		//instance of the menu
		//enterSequence.Append(menuButton.transform.DOLocalMove(new Vector3(-513, 324, 0), menuSpeed)).SetEase(Ease.InOutSine);
		enterSequence.Insert(1, title.transform.DOMove((new Vector3(GameObject.Find("TitleStop").transform.position.x, GameObject.Find("TitleStop").transform.position.y, GameObject.Find("TitleStop").transform.position.z)), 0.5f).SetEase(Ease.InOutBack));
		//loading bar position (+ layer) "exerscent"
		enterSequence.Insert(1, title.transform.DOScale(new Vector3(.6f, .6f, .6f), 0.5f).SetEase(Ease.OutBack));
		yield return enterSequence.WaitForCompletion();
		logInScreen.gameObject.transform.DOLocalMove(new Vector3(320, 0, 0), .3f).SetEase(Ease.InOutBack);

		// StartCoroutine(switchInfoText("Place your tag on the reader to log in", true));
	}

	//"Animations" for selectGame state
	public IEnumerator selectGameAdmin(){
		Sequence selectSequence = DOTween.Sequence();
		selectSequence.Append(menuButton.transform.DOLocalMove(new Vector3(-513, 324, 0), menuSpeed)).SetEase(Ease.InOutSine);
		selectSequence.Insert(1, title.transform.DOMove((new Vector3(GameObject.Find("TitleStop").transform.position.x, GameObject.Find("TitleStop").transform.position.y, GameObject.Find("TitleStop").transform.position.z)), 0.5f).SetEase(Ease.InOutBack));
		selectSequence.Insert(1, title.transform.DOScale(new Vector3(.6f, .6f, .6f), 0.5f).SetEase(Ease.OutBack));
		selectSequence.Append(title.GetComponentInChildren<Image>().DOFillAmount(1, 1f).SetEase(Ease.InSine));
		yield return selectSequence.WaitForCompletion();
		selectGameScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
		StartCoroutine(switchInfoText("Select the amount of options per smell:", true));

	}

	public void toggleConsole()
	{
		if(consoleHidden)
		{
			consoleWindow.transform.localPosition = new Vector3(-133, 268, 0);
			consoleHidden = false;
		} else if(!consoleHidden){
			consoleWindow.transform.localPosition = new Vector3(0, 1000, 0);
			consoleHidden = true;
		}
	}
	
	public void consoleMessage(string newString)
	{
		fourthMsg.GetComponent<TextMeshProUGUI>().text = thirdMsg.GetComponent<TextMeshProUGUI>().text;
		secondMsg.GetComponent<TextMeshProUGUI>().text = firstMsg.GetComponent<TextMeshProUGUI>().text;
		thirdMsg.GetComponent<TextMeshProUGUI>().text = secondMsg.GetComponent<TextMeshProUGUI>().text;
		firstMsg.GetComponent<TextMeshProUGUI>().text = newString;

	}



//=========================== CONCEAL/REVEAL MENU SELECTION ========================================
// Series of functions that conceal or reveal the different menu selections so that they do not
// overlap eachother.
//==================================================================================================

	public void showAbout() {
		if(!aboutOpen) {
			hideAll();
			aboutOpen = true;
			aboutWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence aboutSequence = DOTween.Sequence();
			aboutSequence.Append(aboutWindow.transform.DOLocalMove(new Vector3(220, -50, 0), menuSpeed)).SetEase(Ease.InOutSine);
			about.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
			about.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
		}
	}

	public void showAdmin() {
		if(!adminOpen) {
			hideAll();
			adminOpen = true;
			adminWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence adminSequence = DOTween.Sequence();
			adminSequence.Append(adminWindow.transform.DOLocalMove(new Vector3(250,-100, 0), menuSpeed)).SetEase(Ease.InOutSine);
			// admin.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
			admin.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
		}		
	}

	public void hideAdmin() {
		Sequence closeAdmin = DOTween.Sequence();
		// admin.GetComponent<TextMeshProUGUI>().DOColor(new Color32(224, 175, 29, 255), .3f);
		admin.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
		closeAdmin.Append(adminWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
		adminOpen = false;
	}

	public void showSettings() {
		if(!settingsOpen) {
			hideAll();
			settingsOpen = true;
			settingsWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence settingsSequence = DOTween.Sequence();
			settingsSequence.Append(settingsWindow.transform.DOLocalMove(new Vector3(250,-100, 0), menuSpeed)).SetEase(Ease.InOutSine);
			settings.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
			settings.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
		}		
	}

	public void hideAbout() {
		Debug.Log("About was hidden");
		Sequence closeAbout = DOTween.Sequence();
		closeAbout.Append(aboutWindow.transform.DOLocalMove(new Vector3(220, -1000, 0), menuSpeed)).SetEase(Ease.InOutSine);
		about.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
		about.GetComponent<TextMeshProUGUI>().DOColor(new Color32(223, 139, 25, 255), .3f);
		aboutOpen = false;
	}

	public void showQuit() {
		if(!quitOpen) {
			hideAll();
			quitOpen = true;
			// quitWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence quitSequence = DOTween.Sequence();
			quitSequence.Append(quitWindow.transform.DOLocalMove(new Vector3(200, 0, 0), menuSpeed)).SetEase(Ease.InOutSine);
		}
	}

	public void hideQuit() {
		Sequence closeQuit = DOTween.Sequence();
		closeQuit.Append(quitWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
		quitOpen = false;
	}

	public void showRestart() {
		if(!restartOpen) {
			hideAll();
			restartOpen = true;
			restartWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence restartSequence = DOTween.Sequence();
			restartSequence.Append(restartWindow.transform.DOLocalMove(new Vector3(200, 0, 0), menuSpeed)).SetEase(Ease.InOutSine);
		}
	}

	public void hideRestart() {
		Sequence closeRestart = DOTween.Sequence();
		closeRestart.Append(restartWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
		restartOpen = false;
	}

	public void showExit() {
		if (!manager.gameRunning && (currentState == UIState.selectGame)) {
			hideExit();
			selectGameScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
			updateUIState(UIState.selectGame);
			//hideExit();
		}

		if(endScreen1) {
			hideExit();
			endScreen1 = false;
			newEndGameScript();	
		}

		if(!exitOpen) {
			hideAll();
			exitOpen = true;
			exitWindow.transform.localPosition = new Vector3(200, 1000, 0);
			Sequence exitSequence = DOTween.Sequence();
			exitSequence.Append(exitWindow.transform.DOLocalMove(new Vector3(200, 0, 0), menuSpeed)).SetEase(Ease.InOutSine);
		}
	}

	public void hideExit() {
		Sequence closeExit = DOTween.Sequence();
		closeExit.Append(exitWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
		exitOpen = false;
	}

	public void hideSettings() {
		Sequence closeSettings = DOTween.Sequence();
		settings.GetComponent<TextMeshProUGUI>().DOColor(new Color32(224, 175, 29, 255), .3f);
		settings.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
		closeSettings.Append(settingsWindow.transform.DOLocalMove(new Vector3(200, -1000, 0), menuSpeed)).SetEase(Ease.InSine);
	}

	// hideAll() calls the hide functions of each individual menu selection, to conceal the others
	public void hideAll() {
		if(aboutOpen) {
			hideAbout();
			aboutOpen = false;
		}
		if(exitOpen) {
			hideExit();
			exitOpen = false;
		}
		if(quitOpen) {
			hideQuit();
			quitOpen = false;
		}
		if(restartOpen){
			hideRestart();
			restartOpen = false;
		}
		if(settingsOpen) {
			hideSettings();
			settingsOpen = false;
		}
		if(admin){
			if(adminOpen){
				hideAdmin();
				adminOpen = false;
			}
		}
		
	}

	//public void toggleMenu
	public void loginPressed() {
		hideAll();
		updateUIState(UIState.enterLogin);
	}

	//Quit
	public void quitPressed() {
		manager.quitGame();
	}



//=========================== UI ANIMATIONS =======================================
//			 		Animations for various UI elements.
//=================================================================================

	//Scale and change info text
	public IEnumerator switchInfoText(string newText, bool reappear) {
		Tween shrinkText = infoText.gameObject.transform.DOLocalMove(new Vector3(1000, 0, 0), .3f).SetEase(Ease.InOutBack);
		yield return shrinkText.WaitForCompletion();
		infoText.gameObject.transform.localPosition = new Vector3(-1000, 0, 0);
		infoText.text = newText;
		if(reappear) infoText.gameObject.transform.DOLocalMove(new Vector3(0, 0, 0), .3f).SetEase(Ease.OutBack);
	}

	//Set welcome text and show old data. NOT WORKING.
	public void enterMain() {
		//instance of the Menu
		Sequence enterSequence = DOTween.Sequence();
		enterSequence.Append(menuButton.transform.DOLocalMove(new Vector3(-513, 324, 0), menuSpeed)).SetEase(Ease.InOutSine);
		//Welcome text
		WelcomeText.text = "Hej, " + manager.playerName.ToUpper() + "!";
	}

	public void startPressed() {
		StartCoroutine(startAnim());
	}

	public IEnumerator startAnim() {
		GameObject startButton = GameObject.Find("Start");
		Tween buttonAnim = startButton.transform.DOPunchPosition(new Vector3(15, 15, 15), .7f, 10, 10);
		yield return buttonAnim.WaitForCompletion();
		updateUIState(UIState.waitingForScent);
	}

    //Highlight card on hover
	public IEnumerator focusCard(GameObject card, bool success) {
		Sequence cardSequence = DOTween.Sequence();
		cardSequence.Append(card.transform.DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), .5f));
		cardSequence.Insert(0, card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .5f));
		yield return null;
	}

	//Animate progress bar
	public void animateBar(bool correct, GameObject scentObject) {
		Image circleBar = progressBar.GetComponentInChildren<Image>();
		if(correct) {
			title.GetComponentInChildren<Image>().DOFillAmount((float)manager.totalAttempts / (manager.gameLength), 1f).SetEase(Ease.OutSine);
		}
		else {
			title.GetComponentInChildren<Image>().DOFillAmount((float)manager.totalAttempts / (manager.gameLength), 1f).SetEase(Ease.OutBounce);
		}
	}

	//Animate text on over
	public void onPointerEnter (GameObject caller) {	
		caller.transform.DOScale(new Vector3(1.13f, 1.13f, 1.13f), .17f).SetEase(Ease.InOutSine);
	}

	//Animate text on end hover
	public void onPointerLeave (GameObject caller) {
			caller.transform.DOScale(new Vector3(1, 1, 1), .20f).SetEase(Ease.InOutSine);
	}

	//Old code that was used when GameSize was set in settings
	/*public void updateGameSize(GameObject caller) {
		if (caller == sixOptions){
			Debug.Log("Six options clicked");
			manager.sixOptions();
			setTextColour(caller);
		} else if (caller == tenOptions){
			Debug.Log("Ten options clicked");
			manager.tenOptions();
			setTextColour(caller);
		} else if (caller == twoOptions){
			Debug.Log("Two options clicked");
			manager.twoOptions();
			setTextColour(caller);
		}
	}*/

	//Use this function if you need to delay the change up updateUIState so animations got time to finish 
	public IEnumerator delayedUIState(float time,UIState state)
	{
		yield return new WaitForSeconds(time);
		Debug.Log("UIState was updated");
		updateUIState(state);
	}


	//Continue button for the selectGameState. Button will only activate when certain conditions are met (ie. player name is entered)
	public void continueScript()
	{
		if (currentState == UIState.enterLogin)
		{
			if(manager.playerName != "")
			{	
				logInScreen.transform.DOLocalMove(new Vector3(-1300, 0, 0), transitionSpeed);
				selectGameScreen.transform.DOLocalMove(new Vector3(0, 0, 0), transitionSpeed);
				updateUIState(UIState.selectGame);

			}
		}
		if(currentState == UIState.selectGame){
			if(manager.lengthSelected && manager.gridSelected)
			{
				selectGameScreen.transform.DOLocalMove(new Vector3(-1300, 0, 0), transitionSpeed);
				updateUIState(UIState.welcome);
			}
		} 
	}
	
	//Changes the colour of select options text at SelectGame state of the game
	public void setTextColour(GameObject caller){
		selectTwo.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
		selectTwo.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f);

		selectFour.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
		selectFour.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f);

		selectSix.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
		selectSix.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f);

		selectTen.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
		selectTen.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f);

		caller.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline | FontStyles.Bold;
		caller.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
	}
	

	//Changes caller's text to red
	public void changeColourRed(GameObject caller)
	{
		caller.GetComponent<TextMeshProUGUI>().DOColor(new Color32(219, 69, 20, 255), .3f);
	}
	
	//Changes caller's text to blue
	public void changeColourBlue(GameObject caller)
	{
		caller.GetComponent<TextMeshProUGUI>().DOColor(new Color32(129, 186, 213, 255), .3f);
		
	}

	//Used in selectGame state to select either 2 - 6 - 10 options per smell
	public void selectGameSize(GameObject caller){
		if (caller == selectTwo) {
			Debug.Log("Two options clicked");
			manager.twoOptions();
			setTextColour(caller);
			manager.gridSelected = true;
		} else if (caller == selectFour){
			Debug.Log("Four options clicked");
			manager.fourOptions();
			setTextColour(caller);
			manager.gridSelected = true;
		} else if (caller == selectSix){
			Debug.Log("Six options clicked");
			setTextColour(caller);
			manager.sixOptions();
			manager.gridSelected = true;
		} else if (caller == selectTen){
			Debug.Log("Ten options clicked");
			setTextColour(caller);
			manager.tenOptions();
			manager.gridSelected = true;
		}
		
		if(manager.lengthSelected)
		{
			changeColourRed(continueBTN);
		}
	}

	//Restarts the current session
	public void restartSession(GameObject caller){

			//Hides the "Are you sure" question.
			hideRestart();
			
			//Clears all the current data and cards
			manager.reset();

			//Closes the menu
			updateMenuState(menuButton);

			//Resets the Exerscent logo
			title.GetComponentInChildren<Image>().DOFillAmount(0f ,1f).SetEase(Ease.OutSine);


			//Returns the user to the waiting for scent screen
			StartCoroutine(delayedUIState(1f, UIState.welcome));			

		
	}

	//Exits the current sessions, bring the player back to selectGame state
	public void exitSessionScript(){

			//Hides the "Are you sure" question.
			hideExit();
			
			//Clears all the current data and cards
			manager.reset();

			//Closes the menu
			updateMenuState(menuButton);

			//Resets the Exerscent logo
			title.GetComponentInChildren<Image>().DOFillAmount(0f ,1f).SetEase(Ease.OutSine);


			//Returns the user to the waiting for scent screen
			StartCoroutine(delayedUIState(1f, UIState.selectGame));			
		
	}


	//Shows up when the user has finished a session, if they would like to play another session with the same settings.
	public void playAgain(GameObject caller){
		
		//Clears all the current data
		manager.playAgain();
		
		//Close the Menu
		menuOpen=true;
		updateMenuState(menuButton);

		//Resets the Exerscent logo
		title.GetComponentInChildren<Image>().DOFillAmount(0f ,1f).SetEase(Ease.OutSine);

		//Returns the user to the waiting for scent screen
		StartCoroutine(delayedUIState(1f, UIState.waitingForScent));

		//Moves the "Play Again?" option out of the screen
		caller.transform.DOLocalMove(new Vector3(-1427, -420, 0), transitionSpeed);

		//Removes the EndScreen text
		endScreen.transform.DOLocalMove(new Vector3(-1066, 0, 0), transitionSpeed);

	}
}
