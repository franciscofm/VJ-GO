using UnityEngine;
using UnityEngine.UI;

using Audio;

public class ContentAssigner : MonoBehaviour {

	Image[] buttonsIm;

	public GameObject sourcePrefab;

	public AudioClip clip;

	public Color normal;
	public Color pressed;
	// Use this for initialization
	void Start () {
		ManagerSound.Init (sourcePrefab);

		buttonsIm = new Image[transform.childCount];
		for (int i = 0; i < transform.childCount; ++i) {
			Transform t = transform.GetChild (i);
			t.name = "Channel: " + i;
			t.GetComponentInChildren<Text>().text = "Channel: " + i;
			buttonsIm [i] = t.GetComponentInChildren<Image> ();
			int x = i;
			t.GetComponentInChildren<Button>().onClick.AddListener(delegate { Clicked(x); });
		}
	}

	int channel;
	bool loop;

	public void Clicked(int n) {
		for(int i = 0; i<buttonsIm.Length; ++i)
			buttonsIm[i].color = normal;
		buttonsIm[n].color = pressed;
		this.channel = n;
	}
	public void Loop(bool l) {
		loop = l;
	}

	public void PlaySound(int priority) {
		ManagerSound.PlaySound (clip, loop, channel, priority, ManagerSound.TYPE_EFFECT);
	}
}
