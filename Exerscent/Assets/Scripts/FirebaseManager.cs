using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


public class data {
	string name;
	int score;
}

public struct dataSet {
	public string date;
	public string elapsedTime;
	public string scents;
	public string score;
	public string time;
}

public class FirebaseManager : MonoBehaviour {
	//Reference to firebase database
	DatabaseReference dataref;

	//Reference to gamesystemlogics.cs
	public gameSystemLogic manager;

	// Use this for initialization
	void Start () {
		//Set database URL
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://exerscent-2f698.firebaseio.com/");
		dataref = FirebaseDatabase.DefaultInstance.RootReference;

		//set gameSystemLogic to manager
		manager = GameObject.FindObjectOfType<gameSystemLogic>();
	}

	//Write new game results to database
	public void writeResults(string name, int totalScore, int totalScents, string elapsedTime, List<resultSet> allResults) {
		int resultIndex = 1;
		DatabaseReference newRef = dataref.Push();
        //Write basic session data
		newRef.Child("Name").SetValueAsync(name);
		newRef.Child("Score").SetValueAsync(totalScore);
		newRef.Child("Scents").SetValueAsync(totalScents);
		newRef.Child("Date").SetValueAsync(System.DateTime.Now.ToString("yyyy-MM-dd"));
		newRef.Child("Time").SetValueAsync(System.DateTime.Now.ToString("HH:mm:ss"));
		newRef.Child("Elapsed time").SetValueAsync(elapsedTime);
		DatabaseReference result = newRef.Child("Results");
        //Write all round results
		foreach(resultSet currentSet in allResults) {
			DatabaseReference currentResult = result.Child(resultIndex.ToString());
			currentResult.Child("Correct").SetValueAsync(currentSet.correct);
			currentResult.Child("Attempt").SetValueAsync(currentSet.attempt);
			currentResult.Child("CorrectScent").SetValueAsync(currentSet.correctScent);
			DatabaseReference allOptions = currentResult.Child("Options");
			if(manager.gridSize.x == 5){
				allOptions.Child("Option01").SetValueAsync(currentSet.options[0]);
				allOptions.Child("Option02").SetValueAsync(currentSet.options[1]);
				allOptions.Child("Option03").SetValueAsync(currentSet.options[2]);
				allOptions.Child("Option04").SetValueAsync(currentSet.options[3]);
				allOptions.Child("Option05").SetValueAsync(currentSet.options[4]);
				allOptions.Child("Option06").SetValueAsync(currentSet.options[5]);
				allOptions.Child("Option07").SetValueAsync(currentSet.options[6]);
				allOptions.Child("Option08").SetValueAsync(currentSet.options[7]);
				allOptions.Child("Option09").SetValueAsync(currentSet.options[8]);
				allOptions.Child("Option10").SetValueAsync(currentSet.options[9]);
				resultIndex ++;
			} else if (manager.gridSize.x == 3){
				allOptions.Child("Option01").SetValueAsync(currentSet.options[0]);
				allOptions.Child("Option02").SetValueAsync(currentSet.options[1]);
				allOptions.Child("Option03").SetValueAsync(currentSet.options[2]);
				allOptions.Child("Option04").SetValueAsync(currentSet.options[3]);
				allOptions.Child("Option05").SetValueAsync(currentSet.options[4]);
				allOptions.Child("Option06").SetValueAsync(currentSet.options[5]);
				resultIndex ++;
			} else if (manager.gridSize.x == 2){
				allOptions.Child("Option01").SetValueAsync(currentSet.options[0]);
				allOptions.Child("Option02").SetValueAsync(currentSet.options[1]);
				resultIndex ++;
			}
		}
	}

	//Get data for all old play sessions by this user
	public List<dataSet> fetchData(string name) {
		Debug.Log("fetching");
		List<dataSet> newData = new List<dataSet>();
		dataref.OrderByChild("Name").EqualTo("Testplayer").GetValueAsync().ContinueWith( task =>{
			if(task.IsFaulted){
				Debug.Log("Task error.");
			} else if (task.IsCompleted){
				DataSnapshot snapshot = task.Result;
				Debug.Log(snapshot);
			}
		});
		// Debug.Log( dataref.GetReference(name).OrderbyChild("Name").EqualTo(name));
		return newData;
	}

	public void retriveData(){
		FirebaseDatabase.DefaultInstance.GetReference("Name").ValueChanged += Script_ValueChanged;
	}
	// public void exportData(){
	// 	Firebase.Database.FirebaseDatabase dbInstance = Firebase.Database.FirebaseDatabase.DefaultInstance;
	// 	dbInstance.GetReference("Name").GetValueAsync().ContinueWith( task =>{
	// 		if (task.IsFaulted) {
	// 			//Handle Error
	// 		} else if (task.IsCompleted){
	// 			DataSnapshot snapshot = task.Result;
	// 			foreach (DataSnapshot Name in snapshot.Children){
	// 				IDictionary dictUser = (IDictionary)name.Value;
	// 				Debug.Log("" + dictUser["Name"] + " - " + dictUser["Score"]);
	// 		}}});
	// }

	private void Script_ValueChanged(object sender, ValueChangedEventArgs e){
		string text = e.Snapshot.Child("Testplayer").GetValue(true).ToString();
		Debug.Log(text);
	}
}
