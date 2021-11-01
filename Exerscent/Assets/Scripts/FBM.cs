using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FBM : MonoBehaviour
{

    DatabaseReference dataRef;

    public class User {
        public string name;
        public string date;
        public int scents;
        public int score;
        public string elapsedTime;
        public string time;
        public List<string> Results;

        public User (string username, string email){
            this.name = name;
            this.score = score;
        }
    }


    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://exerscent-2f698.firebaseio.com/");
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void writeResults(string name, int totalScore, int totalScents, string elapsedTime, List<resultSet> allResults){

    }
}
