using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelScript : MonoBehaviour {
	public TMP_Text Label;

	public Transform ItemList;

	public void Setup (string label, List<PanelButton> buttons) {
		Label.text = label;

		foreach (Transform button in ItemList) {
			Destroy(button.gameObject);
		}

		foreach (var button in buttons) {
			CreateButton(button);
		}
	}

	public void Close() {
		Destroy(gameObject);
	}

	public void CreateButton (PanelButton reference) {
		var menuButton = new GameObject(reference.text);

		var tr = menuButton.AddComponent<RectTransform>();
		var im = menuButton.AddComponent<Image>();
		var le = menuButton.AddComponent<LayoutElement>();
		var bu = menuButton.AddComponent<Button>();

		tr.SetParent(ItemList);
		tr.localScale = Vector3.one;
		im.color = new Color(1, 1, 1, 0.07f);
		le.minHeight = 95;
		bu.onClick.AddListener(reference.onClick);

		var text = new GameObject("Text");

		var trt = text.AddComponent<RectTransform>();
		var tet = text.AddComponent<TextMeshProUGUI>();

		trt.SetParent(tr);
		trt.localScale = Vector3.one;
		trt.anchorMin = Vector2.zero;
		trt.anchorMax = Vector2.one;
		trt.pivot = Vector3.one / 2;
		trt.sizeDelta = Vector2.one;
		tet.text = reference.text;
		tet.raycastTarget = false;
		tet.alignment = TextAlignmentOptions.Center;
		tet.fontSize = 36;
	}
}

[System.Serializable]
public class PanelButton {
	public string text;
	public UnityAction onClick;
}


public class PanelBuilder {
	public List<PanelButton> panelButtons = new List<PanelButton>();

	public static PanelBuilder BuildPanel () { return new PanelBuilder(); }

	public PanelBuilder AddButton (string _text, UnityAction _onClick) {
		panelButtons.Add(new PanelButton { text = _text, onClick = _onClick });

		return this;
	}

	public List<PanelButton> GetPanelButtons () { return panelButtons; }
}