using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTileSet
{
    public int FirstGID { get; set; }

    public string Image { get; set; }

    public string Name { get; set; }

    private Dictionary<string, string> tileProperties = new Dictionary<string, string>();
    public Dictionary<string, string> TileProperties { get { return tileProperties; } private set { tileProperties = value; } }

    public void SetupTileProperties(Dictionary<string, object> tileProperties)
    {
        TileProperties = new Dictionary<string,string>();

        if (tileProperties != null)
        {
            // loop through each key
            foreach (string key in tileProperties.Keys)
            {
                //key will just be the id of the tile within this tileset (gid - this.FirstGID)

                object tilePropertyObj = null;

                if (tileProperties.TryGetValue(key, out tilePropertyObj))
                {
                    Dictionary<string, object> tilePropertyDict = (Dictionary<string, object>)tilePropertyObj;

                    if (tilePropertyDict != null)
                    {
                        foreach (string propertyName in tilePropertyDict.Keys)
                        {
                            //we are now in the property dictionary for the individual tile

                            string newKey = GeneratePropertyKey(int.Parse(key) + this.FirstGID, propertyName);
                            
                            object propertyValueObj;
                            string propertyValue = "";

                            if (tilePropertyDict.TryGetValue(propertyName, out propertyValueObj)) {
                                propertyValue = propertyValueObj.ToString();
                            }

                            TileProperties.Add(newKey, propertyValue);
                        }
                    }
                }
            }
        }
    }

    private string assetBase = "";
    public string GetAssetBase()
    {
        if (assetBase.Length > 0)
        {
            return assetBase;
        }

        int extensionIndex = Image.LastIndexOf('.');

        assetBase = Image.Substring(0, extensionIndex);

        return assetBase;
    }

    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
	
	public float TileWidthInPoint {
		get {return TileWidth / Futile.displayScale;}	
	}
	public float TileHeightInPoint {
		get {return TileHeight / Futile.displayScale;}	
	}

    public string GetTilePropertyDescription()
    {
        string description = "";

        foreach (string key in TileProperties.Keys)
        {
            description += "Key[" + key + "]";
            string value;
            if (TileProperties.TryGetValue(key, out value))
            {
                description += " Value = '" + value + "'";
            }
            description += "\r\n";
        }

        return description;
    }

    public string GetPropertyValue(string propertyKey)
    {
        string propertyValue = "";

        string value;
        if (TileProperties.TryGetValue(propertyKey, out value))
        {
            propertyValue = value;
        }

        return propertyValue;
    }

    public bool PropertyExists(string propertyName)
    {
        return TileProperties.ContainsKey(propertyName);
    }

    public List<string> GetPropertyNames()
    {
        List<string> propertyNames = new List<string>();

        foreach (string key in TileProperties.Keys)
        {
            propertyNames.Add(key);
        }

        return propertyNames;
    }

    public string GeneratePropertyKey(int gid, string propertyName)
    {
        string propertyKey = "";

        int tileID = gid - this.FirstGID;

        //example = "tile_forest_0:can_walk"
        propertyKey = this.GetAssetBase() + "_" + tileID + ":" + propertyName;

        return propertyKey;
    }
}
