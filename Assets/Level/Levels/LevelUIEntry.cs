using UnityEngine;

[CreateAssetMenu(fileName = "Phrase", menuName = "Custom/LevelUI")]
public class LevelUIEntry : ScriptableObject {
	public string Scene;
	public string Name;
	public string Description;
	public int Bonuses;
	public Sprite Image;
}
