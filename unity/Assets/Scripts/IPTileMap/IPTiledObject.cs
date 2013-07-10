using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTiledObject : FContainer
{


    public IPObjectLayer Layer { get; set; }

    public string Name { get; set; }

    public string ObjType { get; set; }

    public bool Visible { get; set; }

    public float ObjWidth { get; set; }

    public float ObjHeight { get; set; }

    private Dictionary<string, object> objProperties = new Dictionary<string, object>();
    public Dictionary<string, object> ObjProperties { get { return objProperties; } set { objProperties = value; } }

    public string GetPropertyValue(string propertyName)
    {
        string propertyValue = "";

        object objValue;
        if (ObjProperties.TryGetValue(propertyName, out objValue))
        {
            propertyValue = objValue.ToString();
        }

        return propertyValue;
    }

    public bool PropertyExists(string propertyName)
    {
        return ObjProperties.ContainsKey(propertyName);
    }

    public List<string> GetPropertyNames()
    {
        List<string> propertyNames = new List<string>();

        foreach (string key in ObjProperties.Keys)
        {
            propertyNames.Add(key);
        }

        return propertyNames;
    }

    public string GetObjectPropertyDescription()
    {
        string description = "";

        foreach (string key in ObjProperties.Keys)
        {
            description += "Key[" + key + "]";
            object value;
            if (ObjProperties.TryGetValue(key, out value))
            {
                description += " Value = '" + value.ToString() + "'";
            }
            description += "\r\n";
        }

        return description;
    }

    public Rect GetRect()
    {
        float objLeft = this.x - (this.ObjWidth / 2);
        float objBottom = this.y - (this.ObjHeight / 2);

        Rect objectRect = new Rect(objLeft, objBottom, this.ObjWidth, this.ObjHeight);

        return objectRect;
    }
}
