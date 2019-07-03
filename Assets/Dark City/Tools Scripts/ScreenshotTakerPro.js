function Update() {
	if (Input.GetKeyDown ("p") || Input.GetKeyDown ("o")) {
		var oversizedScreenshot : boolean = Input.GetKeyDown ("o");
		
		var resWidth : int = 4300;
		var resHeight : int = 2200;
		//Set To Screensize If Not Oversized
		if (!oversizedScreenshot) {
			resWidth = Screen.width;
			resHeight = Screen.height;
		}
		
		//Gather up all cameras in the scene so a screenshot can be taken at each depth level
		var allSceneCameras : Camera[] = FindObjectsOfType(Camera) as Camera[];
		var currentImageNumber : int = 1;
		
		var reorderedCameras : Array = allSceneCameras;
		for (i=0; i<reorderedCameras.length; i++) {
			if (reorderedCameras[i].depth <= reorderedCameras[0].depth) {
				reorderedCameras.Unshift(reorderedCameras[i]);
				reorderedCameras.RemoveAt(i+1);
			}
		}
		
		allSceneCameras = reorderedCameras.ToBuiltin(Camera);
		
		for (var currentCamera : Camera in allSceneCameras) {
			if (oversizedScreenshot) {
				//Create the Oversized Render Texture
				var rt : RenderTexture = new RenderTexture(resWidth, resHeight, 24, RenderTextureFormat.RGB565);
				
				//Render Camera
				currentCamera.targetTexture = rt;
				currentCamera.Render();
			}
			
			//Create the blank texture container
			var screenShot : Texture2D;
			screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			
			if (oversizedScreenshot) {
				//Assign rt as the main render texture, so everything is drawn at the higher resolution
				RenderTexture.active = rt;
			}
			
			//Read the current render into the texture container, screenShot
			screenShot.ReadPixels(Rect(0, 0, resWidth, resHeight), 0, 0, false);
			
			//--Clean up--
			if (oversizedScreenshot) {
				RenderTexture.active = null;
			}
			
			currentCamera.targetTexture = null;
			
			if (oversizedScreenshot) {
				Destroy(rt);
			}
			//--End Clean up--
			
			//Convert to PNG file
			var bytes : byte[] = screenShot.EncodeToPNG();
			
			//Save the file
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
				System.IO.File.WriteAllBytes(Application.dataPath + "/screenshots/screen" + System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + " " + currentImageNumber + ".png", bytes);
			} else {
				System.IO.File.WriteAllBytes(Application.dataPath + "/../screenshots/screen" + System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + " " + currentImageNumber + ".png", bytes);
			}
			currentImageNumber ++;
			
			//Second Clean up
			DestroyImmediate(screenShot);
		}
	}
}