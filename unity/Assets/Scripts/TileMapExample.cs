using UnityEngine;
using System.Collections;

public class TileMapExample : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {        
		FutileParams fparams = new FutileParams(true,true,false,false);//true,shouldSupportPortraitUpsideDown);

		fparams.shouldLerpToNearestResolutionLevel = false;		// IMPORTANT: should turn to false to prevent dynamically adjust the display scale, I'll fixed it later
		
		fparams.AddResolutionLevel(480.0f,	1.0f,	1.0f,	""); // iPhone
		fparams.AddResolutionLevel(960.0f,	2.0f,	2.0f,	""); // iPhone retina
		fparams.AddResolutionLevel(1024.0f,	2.0f,	2.0f,	""); // iPad
		fparams.AddResolutionLevel(1136.0f,	2.0f,	2.0f,	""); // iPhone 5
		fparams.AddResolutionLevel(1280.0f,	2.0f,	2.0f,	""); // Nexus 7
		fparams.AddResolutionLevel(2048.0f,	4.0f,	4.0f,	""); // iPad Retina
		
        fparams.origin = new Vector2(0, 0);
        fparams.backgroundColor = Color.gray;
        Futile.instance.Init(fparams);

        // load image atlas (within Resources/Atlases folder)
        Futile.atlasManager.LoadAtlas("Atlases/game");

        GamePage page = new GamePage();

        Futile.stage.AddChild(page);
    }
}
