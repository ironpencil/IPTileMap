using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTileMap : FContainer
{

    //private MapTile[,] tiles;

    private List<IPTileLayer> tileLayers;
    public List<IPTileLayer> TileLayers { get { return tileLayers; } set { tileLayers = value; } }

    private List<IPObjectLayer> objectLayers;
    public List<IPObjectLayer> ObjectLayers { get { return objectLayers; } set { objectLayers = value; } }

    public string Name { get; set; }

    private Dictionary<int, IPTileSet> tileSets;

    //these two collections are only used to cache tile data during setup
    private List<int> tileSetFirstGIDs;
    private Dictionary<int, IPTileSet> tileSetFoundGIDs;

    public int Width { get { return TileWidth * WidthInTiles; } }
    public int Height { get { return TileHeight * HeightInTiles; } }

    public int TileWidth { get; set; }
    public int TileHeight { get; set; }

    public int WidthInTiles { get; set; }
    public int HeightInTiles { get; set; }
  
    private string mapFile;
    Dictionary<string, object> mapData;

    public IPTileMap(string name, string mapFile)
    {
        Name = name;
        this.mapFile = mapFile;
    }

    public void LoadTiles()
    {
        tileSets = new Dictionary<int, IPTileSet>();
        tileLayers = new List<IPTileLayer>();
        objectLayers = new List<IPObjectLayer>();

        tileSetFirstGIDs = new List<int>();
        tileSetFoundGIDs = new Dictionary<int, IPTileSet>();

        TextAsset mapAsset = (TextAsset)Resources.Load(mapFile, typeof(TextAsset));

        if (!mapAsset)
        {
            //map asset not found, no error trapping though...
        }

        mapData = mapAsset.text.dictionaryFromJson();

        Resources.UnloadAsset(mapAsset);

        WidthInTiles = int.Parse(mapData["width"].ToString());
        HeightInTiles = int.Parse(mapData["height"].ToString());        
        TileWidth = int.Parse(mapData["tilewidth"].ToString());
        TileHeight = int.Parse(mapData["tileheight"].ToString());      

        List<object> tileSetsData = (List<object>)mapData["tilesets"];

        foreach (Dictionary<string, object> tileSetData  in tileSetsData)
        {
            IPTileSet tileSet = new IPTileSet();

            tileSet.FirstGID = int.Parse(tileSetData["firstgid"].ToString());
            tileSet.Image = tileSetData["image"].ToString();
            tileSet.Name = tileSetData["name"].ToString();
            tileSet.TileWidth = int.Parse(tileSetData["tilewidth"].ToString());
            tileSet.TileHeight = int.Parse(tileSetData["tileheight"].ToString());

            tileSet.SetupTileProperties((Dictionary<string, object>)tileSetData["tileproperties"]);

            tileSets.Add(tileSet.FirstGID, tileSet);
            tileSetFirstGIDs.Add(tileSet.FirstGID);
        }

        tileSetFirstGIDs.Sort();

        List<object> mapLayers = (List<object>)mapData["layers"];

        foreach (Dictionary<string, object> layerData in mapLayers)
        {

            IPMapLayer mapLayer = null;

            string layerType = layerData["type"].ToString();

            if (layerType.Equals("tilelayer")) {
                mapLayer = new IPTileLayer();
            }
            else if (layerType.Equals("objectgroup"))
            {
                mapLayer = new IPObjectLayer();
            }
            else
            {
                mapLayer = new IPMapLayer();
            }

            mapLayer.Name = layerData["name"].ToString();
            mapLayer.Visible = bool.Parse(layerData["visible"].ToString());
            mapLayer.WidthInTiles = int.Parse(layerData["width"].ToString());
            mapLayer.HeightInTiles = int.Parse(layerData["height"].ToString());
            mapLayer.Opacity = int.Parse(layerData["opacity"].ToString());
            mapLayer.LayerType = layerData["type"].ToString();
            mapLayer.LayerProperties = (Dictionary<string, object>)layerData["properties"];
            
            //these are being pulled from the map, assume same tile size for whole map
            mapLayer.TileWidth = TileWidth;
            mapLayer.TileHeight = TileHeight;            

            if (mapLayer is IPTileLayer) {

                IPTileLayer tileLayer = mapLayer as IPTileLayer;

                tileLayer.InitializeTileArray(mapLayer.WidthInTiles, tileLayer.HeightInTiles);
                //load the tile layer
                List<object> tileGIDs = (List<object>) layerData["data"];

                int x = 0;
                int y = 0;

                for (int i = 0; i < tileGIDs.Count; i++)
                {
                    int tileGID = int.Parse(tileGIDs[i].ToString());

                    //GID of 0 means there is no tile?
                    if (tileGID > 0)
                    {                      
                        IPTileData tileData = new IPTileData();

                        tileData.Layer = tileLayer;
                        tileData.GID = int.Parse(tileGIDs[i].ToString());
                        tileData.TileSet = FindTileSetContainingGID(tileData.GID);

                        tileData.TileX = x;
                        tileData.TileY = tileLayer.HeightInTiles - y - 1; //TileY should count up from the bottom

                        x++;

                        if (x % mapLayer.WidthInTiles == 0)
                        {
                            x = 0;
                            y++;
                        }

                        IPTile tile = new IPTile(tileData);

                        tile.x = tileData.TileX * tileData.TileSet.TileWidth / Futile.displayScale;     // TileWidth is in pixel
                        tile.y = tileData.TileY * tileData.TileSet.TileHeight / Futile.displayScale;    // TileHeight is in pixel

                        //the tile physically resides in the TileLayer container
                        //when the tile layer is added to the container, the tile will be as well
                        tileLayer.AddTile(tile);
                    }
                }

                this.tileLayers.Add(tileLayer);
                //should now have a tile layer that contains all the layer info
                //plus a list of tiles, each referencing the TileSet that its tile comes from
                
            }
            else if (mapLayer is IPObjectLayer)
            {
                IPObjectLayer objectLayer = mapLayer as IPObjectLayer;

                //load the object group
                List<object> objectDefs = (List<object>)layerData["objects"];


                //currently only supports rectangular objects with no tile image (gid)
                foreach (Dictionary<string, object> objectDef in objectDefs)
                {                    
                    IPTiledObject tiledObject = new IPTiledObject();

                    tiledObject.Layer = objectLayer;
                    tiledObject.Name = objectDef["name"].ToString();
                    tiledObject.ObjType = objectDef["type"].ToString();
                    tiledObject.Visible = bool.Parse(objectDef["visible"].ToString());
                    tiledObject.x = float.Parse(objectDef["x"].ToString());
                    tiledObject.y = float.Parse(objectDef["y"].ToString());
                    tiledObject.ObjWidth = float.Parse(objectDef["width"].ToString());
                    tiledObject.ObjHeight = float.Parse(objectDef["height"].ToString());
                    tiledObject.ObjProperties = (Dictionary<string, object>)objectDef["properties"];

                    //adjust y value for Futile (count upwards instead of downwards like in Tiled)
                    tiledObject.y = objectLayer.Height - tiledObject.y - objectLayer.TileHeight;
                    

                    objectLayer.AddObject(tiledObject);                    

                }

                this.objectLayers.Add(objectLayer);
            }    
      
            //done loading the layer, add it to this map
            //tileLayers.Add(mapLayer);
            this.AddChild(mapLayer);
        }

        //these are only used during set-up, don't need them anymore
        tileSetFirstGIDs = null;
        tileSetFoundGIDs = null;

    }    

    private IPTileSet FindTileSetContainingGID(int gid)
    {
        if (tileSetFirstGIDs.Contains(gid))
        {
            return tileSets[gid];
        }

        if (tileSetFoundGIDs.ContainsKey(gid))
        {
            return tileSetFoundGIDs[gid];
        }

        //do not know what tileset contains this gid, so figure it out
        //add our gid to the list of firstGIDs, sort it, then take the previous value

        tileSetFirstGIDs.Add(gid);

        tileSetFirstGIDs.Sort();

        int gidIndex = tileSetFirstGIDs.IndexOf(gid);

        int tileSetFirstGID = tileSetFirstGIDs[gidIndex - 1];

        IPTileSet foundTileSet = tileSets[tileSetFirstGID];

        //found the tileset, now clean up and save this gid for later
        tileSetFirstGIDs.RemoveAt(gidIndex);

        tileSetFoundGIDs.Add(gid, foundTileSet);

        return foundTileSet;
    }    

    public IPTileLayer GetTileLayer(string layerName)
    {
        IPTileLayer requestedLayer = null;

        foreach (IPTileLayer layer in tileLayers)
        {
            if (layer.Name.Equals(layerName))
            {
                requestedLayer = layer;
                break;
            }
        }

        return requestedLayer;
    }

    public IPTileLayer GetTileLayerWithProperty(string propertyName, string propertyValue)
    {
        IPTileLayer requestedLayer = null;

        foreach (IPTileLayer layer in tileLayers)
        {
            //just return the first layer that matches the specified property value
            if (layer.GetPropertyValue(propertyName).Equals(propertyValue)) {
                requestedLayer = layer;
                break;
            }
        }

        return requestedLayer;
    }

    public List<IPTileLayer> GetTileLayersWithProperty(string propertyName, string propertyValue)
    {
        List<IPTileLayer> requestedLayers = new List<IPTileLayer>();

        foreach (IPTileLayer layer in tileLayers)
        {
            if (layer.GetPropertyValue(propertyName).Equals(propertyValue))
            {
                requestedLayers.Add(layer);
            }
        }

        return requestedLayers;
    }

    public IPObjectLayer GetObjectLayer(string layerName)
    {
        IPObjectLayer requestedLayer = null;

        foreach (IPObjectLayer layer in objectLayers)
        {
            if (layer.Name.Equals(layerName))
            {
                requestedLayer = layer;
                break;
            }
        }

        return requestedLayer;
    }

    public IPObjectLayer GetObjectLayerWithProperty(string propertyName, string propertyValue)
    {
        IPObjectLayer requestedLayer = null;

        foreach (IPObjectLayer layer in objectLayers)
        {
            //just return the first layer that matches the specified property value
            if (layer.GetPropertyValue(propertyName).Equals(propertyValue))
            {
                requestedLayer = layer;
                break;
            }
        }

        return requestedLayer;
    }

    public List<IPObjectLayer> GetObjectLayersWithProperty(string propertyName, string propertyValue)
    {
        List<IPObjectLayer> requestedLayers = new List<IPObjectLayer>();
        //TileLayer requestedLayer = null;

        foreach (IPObjectLayer layer in objectLayers)
        {
            if (layer.GetPropertyValue(propertyName).Equals(propertyValue))
            {
                requestedLayers.Add(layer);
            }
        }

        return requestedLayers;
    }

}
