using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class User {

	public string Name;
	public string Lenguage;
	public List<LevelRegistry> Registries;

}

[System.Serializable]
public class LevelRegistry {
	public string Level = "Unkown";
	public float Completion = 0f;
}