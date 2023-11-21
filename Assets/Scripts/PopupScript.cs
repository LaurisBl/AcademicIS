using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupScript : MonoBehaviour
{
	public TMP_Text Label;
	public TMP_Text Text;

	public enum PopupType { Info, Warning, Error };

	public void Setup(string title, string text, PopupType type) {
		switch(type) {
			case PopupType.Warning:
				Label.color = new Color32(129, 139, 49, 255);
				break;

			case PopupType.Error:
				Label.color = new Color32(214, 40, 40, 255);
				break;
		}

		Label.text = title;
		Text.text = text;
	}

	public void Close() {
		Destroy(gameObject);
	}
}
