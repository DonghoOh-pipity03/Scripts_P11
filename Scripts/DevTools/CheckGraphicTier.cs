using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGraphicTier : MonoBehaviour
{
    string text = "0";
    private void Start() {
        text = QualitySettings.GetQualityLevel().ToString();
    }
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(w / 2, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
		GUI.Label(rect, text, style);
	}
}
