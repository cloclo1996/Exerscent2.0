using UnityEngine;
using System.Collections;
using System.IO.Ports;//.Ports;
using System.Threading;

public class SerialCom : MonoBehaviour {


//	SerialPort stream = new SerialPort("/dev/cu.usbmodem1411", 9600); //Set the port (com4) and the baud rate (9600, is standard on most devices)
	SerialPort stream; 

	public string value;
	public string scentName;
	public string scentType;

	public GameObject imgHandler;
	public gameSystemLogic gameManager;
	bool portOpen=false;

	void Start () {



	}

	public void setPort(string portName){
		
		stream = new SerialPort (portName, 9600); //Assign port name and baud rate (based on "SerialPorts" script)
		stream.Open();                            //Open the Serial Stream
		stream.ReadTimeout = 6;                 //Timeout for stream NEEDS TO BE SAME AS FRAME RATE IN ORDER TO NOT MURDER PERFORMANCE
		portOpen = true;       
		gameManager.UIManager.updateUIState(UIState.enterLogin);
		StartCoroutine(ReadTwoStrings());
		//Bool to keep track of port state
		gameManager.UIManager.consoleMessage("SerialCom recieved " + portName + " as port.");
	}


	void Update () {

		//Read stream if the port is open
		if(portOpen){
			
		}
	}

	public IEnumerator ReadTwoStrings(){   
		//scentName.ToLower ();
		while (stream.IsOpen) {

			try
			{
				string value2 = stream.ReadLine (); //Read the information
				
				//print (value2);

				if (value2.Length > 0) {
					//assign scent type and scent name to string array
					string[] inVals = value2.Split (','); //Split incoming string at comma (example string: "vegetable,carrot")

					//Check that array slots are not empty
					if (inVals [0] != "" && inVals [1] != "") {

						//print (inVals [0]+":"+inVals [1]);

						//Assign array content to string variables
						scentName = inVals [1];
						}
					else if(inVals [0] != "" && inVals[1] =="") {
						scentName = inVals [0];
					}
						/* if(inVals.Length > 0) {
							scentName = inVals [1];
						} 
						else scentName = inVals[0];
*/
						//Make sure the texts are treated as strings
						//scentType=scentType.ToString();
						scentName=scentName.ToString();

						//Trim the strings to get rid of spaces
						//scentType=scentType.Trim(); 
						//scentName=scentName.Trim(); 
						gameManager.UIManager.consoleMessage("The following tag was scanned: " + scentName + ".");
						
						// //Send scent name to game manager LEGACY FUNCTION
						// if(gameManager.UIManager.currentState == UIState.enterLogin) {
						// 	gameManager.playerName = scentName;
						// 	gameManager.UIManager.updateUIState(UIState.selectGame);
						// }				
						//Debug.Log(gameManager.UIManager.currentState);
						if(gameManager.UIManager.currentState == UIState.waitingForScent || gameManager.UIManager.currentState == UIState.welcome) {
							gameManager.switchScent(scentName);
							Debug.Log("You scanned '" + scentName + "' to be used as the correct answer.");
						}
						

						print(scentType.ToLower()+" called "+scentName.ToLower()); //Print for bug testing
						

						stream.BaseStream.Flush (); //Clear serial stream to assure we get new information
					

				}
				else {
					//gameManager.destroyScents();
				}
				
			}
			catch(System.Exception) {}
			//function where on could print error messages
			yield return new WaitForSeconds(0.06f);
		}
		
	}




}