using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTileLayer : IPMapLayer
{
    private List<IPTile> tiles = new List<IPTile>();
    public List<IPTile> Tiles { get { return tiles; } }

    private IPTile[,] tileArray = null;

    public void InitializeTileArray(int widthInTiles, int heightInTiles) {

        tileArray = new IPTile[widthInTiles, heightInTiles];        

    }

    public void AddTile(IPTile tile)
    {
        tiles.Add(tile);
        int tileX = tile.TileData.TileX;
        int tileY = tile.TileData.TileY;

        tileArray[tileX, tileY] = tile;

        this.AddChild(tile);
    }

    public void RemoveTile(IPTile tile)
    {
        tiles.Remove(tile);
        int tileX = tile.TileData.TileX;
        int tileY = tile.TileData.TileY;

        IPTile tileToRemove = tileArray[tileX, tileY];

        if (tile == tileToRemove)
        {
            tileArray[tileX, tileY] = null;
        }

        this.RemoveChild(tile);
    }

    public IPTile GetTileAt(int tileX, int tileY)
    {
        return tileArray[tileX, tileY];
    }

    public Vector2 GetTilePosition(int tileX, int tileY)
    {
        return GetTileAt(tileX, tileY).GetPosition();
    }

}
