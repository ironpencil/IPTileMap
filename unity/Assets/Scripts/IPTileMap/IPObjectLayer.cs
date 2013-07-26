using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPObjectLayer : IPMapLayer
{

    private List<IPTiledObject> objects = new List<IPTiledObject>();
    public List<IPTiledObject> Objects { get { return objects; } }

    public void AddObject(IPTiledObject tiledObject)
    {
        objects.Add(tiledObject);

        this.AddChild(tiledObject);
    }

    public void RemoveObject(IPTiledObject tiledObject)
    {
        objects.Remove(tiledObject);

        this.RemoveChild(tiledObject);
    }

    public List<IPTiledObject> GetTiledObjectsAt(float x, float y)
    {
        List<IPTiledObject> objectList = new List<IPTiledObject>();

        foreach (IPTiledObject tiledObject in objects)
        {
            if (tiledObject.x == x && tiledObject.y == y)
            {
                objectList.Add(tiledObject);
            }
        }

        return objectList;
    }

    public List<IPTiledObject> GetTiledObjectsContainingPoint(float x, float y)
    {
        List<IPTiledObject> objectList = new List<IPTiledObject>();
		
//		Vector2 checkPoint = GlobalToLocal(new Vector2(x, y));
		Vector2 checkPoint = new Vector2(x, y);
//		Debug.Log("check point " + checkPoint.ToString());
        foreach (IPTiledObject tiledObject in objects)
        {
            if (tiledObject.GetRectInPoint().Contains(checkPoint))
            {
                objectList.Add(tiledObject);
            }
        }

        return objectList;
    }

    public List<IPTiledObject> GetTiledObjectsIntersectingRect(Rect checkRect)
    {
        List<IPTiledObject> objectList = new List<IPTiledObject>();
        foreach (IPTiledObject tiledObject in objects)
        {
            if (tiledObject.GetRectInPoint().CheckIntersect(checkRect))
            {
                objectList.Add(tiledObject);
            }
        }
        return objectList;
    }

    public string GetRectDescription(Rect rect)
    {
        string desc = "{";
        desc += "xMin:" + rect.xMin + " ";
        desc += "xMax:" + rect.xMax + " ";
        desc += "yMin:" + rect.yMin + " ";
        desc += "yMax:" + rect.yMax + "}";

        return desc;
    }
    
}
