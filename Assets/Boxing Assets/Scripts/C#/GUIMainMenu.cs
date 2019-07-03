using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GUIMainMenu : MonoBehaviour
{
    public Texture2D controlsTexture;
   
    void OnGUI(){
	
	GUI.depth = 0;
	
	if(GUI.Button(new Rect(30,210,(Screen.width / 2) - 30,100),"Start New Game")){
		Application.LoadLevel("scene");
	}
	
	GUI.Box(new Rect(0,Screen.height-110,Screen.width,110),"");
	GUI.DrawTexture(new Rect(20,Screen.height-controlsTexture.height,controlsTexture.width,controlsTexture.height),controlsTexture);
}
}
