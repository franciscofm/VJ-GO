using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

public static class DataManager {

	public static User SaveState;
	public static Configuration ConfState;
	public static bool Loaded;

	public static User LoadData() {
		//Debug.Log ("Loading data...");
		string PersistantPath = Application.persistentDataPath;
		string DirectoryPath = Path.Combine (PersistantPath, "Saves");
		string SavePath = Path.Combine (DirectoryPath, "User.binary");
		string ConfPath = Path.Combine (DirectoryPath, "Cond.json");

		if (!Directory.Exists (DirectoryPath) || !File.Exists(SavePath))
			return CreateData ();
			
		BinaryFormatter Formatter = new BinaryFormatter ();
		FileStream FileReader = File.Open (SavePath, FileMode.Open);
		SaveState = (User)Formatter.Deserialize (FileReader);
		FileReader.Close ();

		string json = File.ReadAllText (ConfPath);
		ConfState = JsonUtility.FromJson<Configuration> (json);

		Loaded = true;
		return SaveState;
	}
	public static User CreateData() {
		//Debug.Log ("Creating data...");
		SaveState = new User ();
		SaveState.Registries = new List<LevelRegistry> ();

		ConfState = new Configuration ();
		ConfState.Lenguage = GetLenguage ();

		SaveData ();
		return SaveState;
    }
	public static void SaveData() {
		Debug.Log ("Saving data...");
		string PersistantPath = Application.persistentDataPath;
		string DirectoryPath = Path.Combine (PersistantPath, "Saves");
		string SavePath = Path.Combine (DirectoryPath, "User.binary");
		string ConfPath = Path.Combine (DirectoryPath, "Cond.json");

		if (!Directory.Exists (DirectoryPath))
			Directory.CreateDirectory (DirectoryPath);

		BinaryFormatter Formatter = new BinaryFormatter ();
		FileStream FileWriter = File.Create (SavePath);
		Formatter.Serialize (FileWriter, SaveState);
		FileWriter.Close ();
		string json = JsonUtility.ToJson (ConfState);
		File.WriteAllText (ConfPath, json);
  	}
	public static bool EraseData() {
		string PersistantPath = Application.persistentDataPath;
		string DirectoryPath = Path.Combine (PersistantPath, "Saves");
		string SavePath = Path.Combine (DirectoryPath, "User.binary");
		string ConfPath = Path.Combine (DirectoryPath, "Cond.json");

		//Debug.Log (SavePath);
		if (!File.Exists(SavePath))
			return false;
		File.Delete (SavePath);
		//Debug.Log ("Deleted data...");

		if (!File.Exists(ConfPath))
			return false;
		File.Delete (ConfPath);
		//Debug.Log ("Deleted config...");

		return true;
	}

	public static string GetLenguage() {
		string Lenguage = "English";
		SystemLanguage sysLeng = Application.systemLanguage;
		switch (sysLeng) {
		case SystemLanguage.Catalan:
			Lenguage = "Catalan";
			break;
		case SystemLanguage.Spanish:
			Lenguage = "Spanish";
			break;
		default:
			Lenguage = "English";
			break;
		}
		return Lenguage;
	}
}
