using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

// attach to UI Text component (with the full text already there)

public class UITextTypeWriter : MonoBehaviour
{

	[SerializeField] private float _timeBetweenChars = 0.125f;

	TextMeshProUGUI txt;
	string story;

	void Awake()
	{
		txt = GetComponent<TextMeshProUGUI>();
		story = txt.text;
		txt.text = "";

		// TODO: add optional delay when to start
		StartCoroutine(PlayText());
	}

	IEnumerator PlayText()
	{
		foreach (char c in story)
		{
			txt.text += c;
			yield return new WaitForSeconds(_timeBetweenChars);
		}
	}

}