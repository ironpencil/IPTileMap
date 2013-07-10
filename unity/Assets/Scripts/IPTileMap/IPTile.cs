using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTile : FSprite
{

    public IPTileData TileData;

    public IPTile(IPTileData tileData) : base(tileData.GetAssetName())
    {
        this.TileData = tileData;
        //this.anchorX = 0;
        //this.anchorY = 1;

    }

    public Rect GetRect()
    {
        //get the magnitutde of the width and height
        float widthMag = Mathf.Abs(this.width);
        float heightMag = Mathf.Abs(this.height);

        //this finds the left side of the sprite so long as the width is positive
        //finds the right side of the sprite if width is negative (untested)
        float left = this.x - (this.width * (this.anchorX));     
    
        //if the width is negative, subtract the magnitutde of the width to find the left side
        if (this.width < 0)
        {
            left -= widthMag;
        }

        float bottom = this.y - (this.height * (this.anchorY));

        if (this.height < 0)
        {
            bottom -= heightMag;
        }       

        //return a rect with the calculated bottm, left, and sizes
        return new Rect(left, bottom, widthMag, heightMag);
    }
}
