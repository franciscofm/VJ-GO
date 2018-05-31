using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class User {
	public string Name;
	public List<LevelRegistry> Registries;
}

[System.Serializable]
public class LevelRegistry {
	public string Level = "Unkown";
	public float Completion = 0f;
}

[System.Serializable]
public class Configuration {
	public string Lenguage = "English";
	public float GlobalVolume = 1f;
	public float VoiceVolume = 1f;
	public float EffectVolume = 1f;
	public float MusicVolume = 1f;
}