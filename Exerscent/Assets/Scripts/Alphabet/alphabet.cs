using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



public class alphabet : MonoBehaviour {

	// Use this for initialization
	public bool keyboard=false;
	public string name;          
	public Text userName; 

	public Button loginBtn;
	public Button createNewBtn;

	public Button nameInputBtn;

	public GameObject imageHandlerObj;   //Instance of object to reach the player name variable in script



	void Start () {
		
		//System.Text.StringBuilder name = new System.Text.StringBuilder();

	}
	
	// Update is called once per frame
	void Update () {
		if(keyboard){


			if(Input.GetKeyUp(KeyCode.A)){
				name = name+'A';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.B)){
				name = name+'B';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.C)){
				name = name+'C';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.D)){
				name = name+'D';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.E)){
				name = name+'E';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.F)){
				name = name+'F';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.G)){
				name = name+'G';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.H)){
				name = name+'H';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.I)){
				name = name+'I';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.J)){
				name = name+'J';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.K)){
				name = name+'K';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.L)){
				name = name+'L';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.M)){
				name = name+'M';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.N)){
				name = name+'N';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.O)){
				name = name+'O';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.P)){
				name = name+'P';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.Q)){
				name = name+'Q';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.R)){
				name = name+'R';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.S)){
				name = name+'S';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.T)){
				name = name+'T';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.U)){
				name = name+'U';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.V)){
				name = name+'V';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.W)){
				name = name+'W';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.X)){
				name = name+'X';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.Y)){
				name = name+'Y';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.Z)){
				name = name+'Z';
				userName.text = name;
			}
			if(Input.GetKeyUp(KeyCode.Backspace)){
				
				if(name.Length>0){
					name = name.Remove(name.Length - 1);  //Remove the last character
				}
				userName.text = name;


			}




			imageHandlerObj.GetComponent<ImageHandler>().setPlayerName (name);  //Assign name to variable in external script


		}
	}
}
