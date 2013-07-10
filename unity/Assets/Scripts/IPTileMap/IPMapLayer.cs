using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPMapLayer : FContainer
{
    public string Name { get; set; }

    public bool Visible { get; set; }

    public int WidthInTiles { get; set; }

    public int HeightInTiles { get; set; }

    public int TileWidth { get; set; }

    public int TileHeight { get; set; }

    public int Width { get { return WidthInTiles * TileWidth; } }

    public int Height { get { return HeightInTiles * TileHeight; } }

    public string LayerType { get; set; }

    public int Opacity { get; set; }

    public Dictionary<string, object> LayerProperties { get; set; }

    public string GetPropertyValue(string propertyName)
    {
        string propertyValue = "";

        if (LayerProperties != null)
        {

            if (LayerProperties.ContainsKey(propertyName))
            {

                object objValue;
                if (LayerProperties.TryGetValue(propertyName, out objValue))
                {
                    propertyValue = objValue.ToString();
                }

            }
        }

        return propertyValue;
    }

    public List<string> GetPropertyNames()
    {
        List<string> propertyNames = new List<string>();

        foreach (string key in LayerProperties.Keys)
        {
            propertyNames.Add(key);
        }

        return propertyNames;
    }

}
