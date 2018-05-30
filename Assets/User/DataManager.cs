using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

public static class DataManager {

	public static User SaveState;
	public static bool Loaded;

	public static User LoadData() {
		Debug.Log ("Loading data...");
		string PersistantPath = Application.persistentDataPath;
		string DirectoryPath = Path.Combine (PersistantPath, "Saves");
		string FilePath = Path.Combine (DirectoryPath, "User.binary");

		if (!Directory.Exists (DirectoryPath) || !File.Exists(FilePath))
			return CreateData ();
			
		BinaryFormatter Formatter = new BinaryFormatter ();
		FileStream FileReader = File.Open (FilePath, FileMode.Open);
		SaveState = (User)Formatter.Deserialize (FileReader);
		FileReader.Close ();
		Loaded = true;
		return SaveState;
	}
	public static User CreateData() {
		Debug.Log ("Creating data...");
		SaveState = new User ();
		SaveState.Registries = new List<LevelRegistry> ();
		SaveState.Lenguage = GetLenguage ();

		SaveData ();
		return SaveState;
      	}
	public static void SaveData() {
		Debug.Log ("Saving data...");
		string PersistantPath = Application.persistentDataPath;
		string DirectoryPath = Path.Combine (PersistantPath, "Saves");
		string FilePath = Path.Combine (DirectoryPath, "User.binary");
		if (!Directory.Exists (DirectoryPath))
			Directory.CreateDirectory (DirectoryPath);

		BinaryFormatter Formatter = new BinaryFormatter ();
		FileStream FileWriter = File.Create (FilePath);
		Formatter.Serialize (FileWriter, SaveState);
		FileWriter.Close ();
  	}
	public static bool EraseData() {
		Debug.Log ("Erasing data...");
		string PersistantPath = Application.persistentDataPath;
		string DirectoryPath = Path.Combine (PersistantPath, "Saves");
		string FilePath = Path.Combine (DirectoryPath, "User.binary");
		if (!File.Exists(FilePath))
			return false;
		File.Delete (FilePath);
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
