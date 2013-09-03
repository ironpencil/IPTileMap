using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePage : FContainer {

	IPTileMap tileMap;
	
	
	public GamePage () {
		// 1. load the tile map.
		tileMap = new IPTileMap("random name", "JSON/platform");
		tileMap.LoadTiles();
		
		AddChild(tileMap);
		
		// 2. load the object layer (optional)
		IPObjectLayer terrain = tileMap.GetObjectLayer("Terrain");
		List<IPTiledObject> objects = terrain.Objects;
		foreach (IPTiledObject obj in objects) {
			Rect rect = obj.GetRectInPoint();	// instead of using GetRect, which returns the rect whose size is in pixel
			FSprite rectSprite = CreateSpriteForRect(rect, Color.red);
			AddChild(rectSprite);
		}
	}
	
	private static FSprite CreateSpriteForRect (Rect rect, Color color) {
		float left = rect.x;
		float bottom = rect.y;
		float width = rect.width;
		float height = rect.height;
		
		FSprite sprite = new FSprite(Futile.whiteElement);
		sprite.color = color;
		sprite.SetAnchor(0, 0);
		sprite.x = left;
		sprite.y = bottom;
		sprite.width = width;
		sprite.height = height;
		
		return sprite;
	}
}