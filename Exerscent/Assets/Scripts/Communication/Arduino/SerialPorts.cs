using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;//.Ports;


public class SerialPorts : MonoBehaviour {

	// Use this for initialization
	public string[] ports;
	string portName;
	string wantedPortName="usbmodem";
	string wantedPortNameWindows="COM";
	string fullPortName;
	public bool portFound=false;
	public UIManager UIManager;
	public int offerQuit = 0;


	void Start () {

		//A coroutine that checks for ports once every second and assigns it once something is connected
		//The App should then tell the user when the USB is not plugged in
		InvokeRepeating("getPortNames", 5.0F, 1.0F);


		//Debug.Log(SerialPort.GetPortNames());

	}
	
	// Update is called once per frame
	void Update () {


	}



	public void getPortNames ()
	{
		//Debug.Log("getting ports!");
		int p = (int)System.Environment.OSVersion.Platform;
		List<string> serial_ports = new List<string> ();

		portFound = false;

		// Are we on Unix? 
		if (p == 4 || p == 128 || p == 6) {
			string[] ttys = System.IO.Directory.GetFiles ("/dev/", "tty.*");
			foreach (string dev in ttys) {
				if (dev.StartsWith ("/dev/tty.*"))
					serial_ports.Add (dev);
				//Debug.Log (System.String.Format (dev));  //Prints the name of the port in use

				fullPortName = System.String.Format (dev);   //Save the name of the port in use to a string variable
				portName = fullPortName; 
				UIManager.consoleMessage("Port was found: " + portName);                   
			
				//Check if port name contains the string "usbmodem", as this is part of what virtual Arduino ports are named on Mac
				if (fullPortName.Contains (wantedPortName)) {
					print ("Choose this port: " + fullPortName);
					UIManager.consoleMessage("The following port was selected: " + fullPortName);

					this.gameObject.GetComponent<SerialCom> ().setPort (fullPortName);
					portFound = true;

					if(offerQuit==15)
					{
						UIManager.hideErrorMessage();
					}

					CancelInvoke(); //cancels Invoke repeating as port is found.


				} else {
				

				}
			}
		} else if (p == 2) {

			Debug.Log("Windows OS running");

			// Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            Debug.Log("The following serial ports were found:" + ports + ".");

            // Display each port name to the console.
            foreach(string port in ports)
            {
                Debug.Log(port);
				if(port.Contains (wantedPortNameWindows))
				{
					Debug.Log("Port was found for windows");
					UIManager.consoleMessage("The following port was selected: " + port + ".");

					this.gameObject.GetComponent<SerialCom>().setPort(port);
					portFound = true;
					
					if(offerQuit==15)
					{
						UIManager.hideErrorMessage();
					}

					CancelInvoke();
				}
            }		
		}

		if(portFound==false){
			
			//Should later use UI instead of print() to let user know the usb has not been inserted
			//print ("Please make sure you have inserted the scent platform");
			Debug.Log("Port has not been found");
			Debug.Log("OS version is " + p);


			if(offerQuit == 15)
			{
				UIManager.showErrorMessage();
				UIManager.consoleMessage("OS Version: " + p + ". No port was found.");
			}

			if(offerQuit < 15)
			{
				offerQuit++;
			}
		}

	}
}
