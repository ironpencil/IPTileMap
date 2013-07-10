using UnityEngine;
using System.Collections;

public class TiledExample : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        FutileParams fparams = new FutileParams(true, true, false, false);
        fparams.AddResolutionLevel(960.0f, 1.0f, 1.0f, "");
        fparams.origin = new Vector2(0.5f, 0.5f);
        fparams.backgroundColor = Color.magenta;
        Futile.instance.Init(fparams);

        // load image atlas (within Resources/Atlases folder)
        Futile.atlasManager.LoadAtlas("Atlases/survive");

        IPTileMapExample example = new IPTileMapExample();

        Futile.stage.AddChild(example);
    }

    // Update is called once per frame
    void Update()
    {

    }

}
