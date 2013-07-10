/*
- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
- THE SOFTWARE.
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IPTileMapExample : FContainer
{
   
    IPTileMap tileMap;
    FLabel textLabel;
    IPTileLayer terrainLayer;
    IPObjectLayer terrainObjects;

    public bool Paused { get; set; }

    FSprite player;
    private bool PlayerIsMoving = false;
    private Vector2 PlayerTargetPosition = Vector2.zero;
    private Vector2 PlayerTile = Vector2.zero;
    private float PlayerSpeedX;
    private float PlayerSpeedY;


    private const string PLAYER_ASSET = "player";
    private const string TILE_MAP_JSON_ASSET = "JSON/forestMapLarge";
    private const string LAYER_TYPE_PROPERTY = "LAYER_TYPE";
    private const string TERRAIN_LAYER_NAME = "TERRAIN";
    private const string EVENT_TEXT_PROPERTY = "TEXT";
    private const string CAN_WALK_PROPERTY = "CAN_WALK";
    private const string EVENT_OBJECT_TYPE = "EVENT";
    private const string EVENT_TYPE_PROPERTY = "EVENT_TYPE";
    

    public IPTileMapExample() : base() { }

    public void OnUpdate()
    {
        if (this.Paused)
        {
            return;
        }

        //only handle input if player is standing still
        if (PlayerIsMoving)
        {
            //Approach() returns true when the node is at the target position
            //so we can use that to know that we are done moving
            if (Approach(player, PlayerTargetPosition, PlayerSpeedX, PlayerSpeedY))
            {
                PlayerIsMoving = false;
                //Check Events at new position
                CheckEvents();
            }
        }
        else
        {
            //handle player movement input
            bool playerMoved = false;

            int tileYDelta = 0;
            int tileXDelta = 0;

            if (Input.GetKey("up"))
            {
                if (PlayerTile.y < tileMap.HeightInTiles - 1)
                {
                    tileYDelta += 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey("down"))
            {
                if (PlayerTile.y > 0)
                {
                    tileYDelta -= 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey("left"))
            {
                if (PlayerTile.x > 0)
                {
                    tileXDelta -= 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey("right"))
            {
                if (PlayerTile.x < tileMap.WidthInTiles - 1)
                {
                    tileXDelta += 1;
                    playerMoved = true;
                }
            }

            //if player did input a move, execute code to move him
            if (playerMoved)
            {
                int targetTileX = (int)PlayerTile.x + tileXDelta;
                int targetTileY = (int)PlayerTile.y + tileYDelta;

                bool canWalk = CanMoveToTile(targetTileX, targetTileY);

                if (canWalk)
                {
                    MovePlayerToTile(targetTileX, targetTileY, false);
                }
            }
        }
    }

    public override void HandleAddedToStage()
    {
        base.HandleAddedToStage();
        Futile.instance.SignalUpdate += OnUpdate;

        player = new FSprite(PLAYER_ASSET);

        //speed is used later in update, multiplied by Time.deltaTime.
        //this basically means the player can move this much distance in one second
        //player.width * 5 means he can move 5 tiles in one second
        PlayerSpeedX = player.width * 5;
        PlayerSpeedY = player.height * 5;

        tileMap = new IPTileMap("MyTileMapName", TILE_MAP_JSON_ASSET);
        tileMap.LoadTiles();

        this.AddChild(tileMap);
        tileMap.AddChild(player);

        terrainLayer = tileMap.GetTileLayerWithProperty(LAYER_TYPE_PROPERTY, TERRAIN_LAYER_NAME);

        if (terrainLayer == null)
        {
            //No TERRAIN layer found
            //no error trapping either...
        }

        terrainObjects = tileMap.GetObjectLayerWithProperty(LAYER_TYPE_PROPERTY, TERRAIN_LAYER_NAME);

        if (terrainObjects == null)
        {
            //No TERRAIN object layer found
            //no error trapping either...
        }

        //make the stage follow the player so he stays in the center of the screen
        player.stage.Follow(player, true, true);

        MovePlayerToTile((int)terrainLayer.WidthInTiles / 2, (int)terrainLayer.HeightInTiles / 2, true);
    }

    public override void HandleRemovedFromStage()
    {
        base.HandleRemovedFromStage();
    }

    public bool Approach(FNode node, Vector2 targetPosition, float speedX, float speedY)
    {
        //return true if we're already at our target
        if (node.GetPosition() == targetPosition) { return true; }

        Vector2 dtSpeed = new Vector2(speedX * Time.deltaTime, speedY * Time.deltaTime);

        Vector2 difference = targetPosition - node.GetPosition();

        if (difference.x > dtSpeed.x)
        {
            node.x += dtSpeed.x;
        }
        else if (difference.x < -dtSpeed.x)
        {
            node.x -= dtSpeed.x;
        }
        else
        {
            node.x = targetPosition.x;
        }

        if (difference.y > dtSpeed.y)
        {
            node.y += dtSpeed.y;
        }
        else if (difference.y < -dtSpeed.y)
        {
            node.y -= dtSpeed.y;
        }
        else
        {
            node.y = targetPosition.y;
        }

        return (targetPosition == node.GetPosition());

    }

    private void MessageEvent(IPTiledObject tileObject)
    {
        //run the code here to display a message because we moved to a tile containing a message event
        Debug.Log("Message Event: " + tileObject.GetPropertyValue(EVENT_TEXT_PROPERTY));
    }

    private bool shallowStreamTriggered = false;
    private void ShallowStreamEvent(IPTiledObject tileObject)
    {
        if (!shallowStreamTriggered)
        {
            Debug.Log("Shallow Stream Event: " + tileObject.GetPropertyValue(EVENT_TEXT_PROPERTY));
            shallowStreamTriggered = true;
        }        
    }

    public void MovePlayerToTile(int tileX, int tileY, bool doTeleport)
    {
        PlayerTile.x = tileX;
        PlayerTile.y = tileY;

        //setting the "PlayerTargetPosition" and setting PlayerIsMoving to true is enough to move the player
        //because the OnUpdate() will consantly move the player towards that position
        Vector2 newPosition = terrainLayer.GetTilePosition((int)PlayerTile.x, (int)PlayerTile.y);
        PlayerIsMoving = true;
        PlayerTargetPosition = newPosition;

        //if we just want to make them "jump" to a specific tile, then just set the player position
        if (doTeleport)
        {
            player.SetPosition(newPosition);
        }
    }

    private void CheckEvents()
    {
        //check for events at player's location
        //get all objects that intersect with player's location
        List<IPTiledObject> tileObjects = terrainObjects.GetTiledObjectsContainingPoint(player.x, player.y);

        foreach (IPTiledObject tileObject in tileObjects)
        {
            if (tileObject.ObjType.Equals(EVENT_OBJECT_TYPE))
            {
                //this is an event object, run the event
                string eventType = tileObject.GetPropertyValue(EVENT_TYPE_PROPERTY);

                //these event types could be anything, just make a function to perform the event when it is triggered
                switch (eventType)
                {
                    case "MESSAGE": MessageEvent(tileObject);
                        break;
                    case "SHALLOW_STREAM_NORTH":
                    case "SHALLOW_STREAM_SOUTH": ShallowStreamEvent(tileObject);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private bool CanMoveToTile(int targetTileX, int targetTileY)
    {
        //get the tile we're trying to move to
        IPTile targetTile = terrainLayer.GetTileAt(targetTileX, targetTileY);
        //MapTile targetTile = tileMap.GetTile(targetTileX, targetTileY);

        //get all objects that intersect with this tile
        List<IPTiledObject> tileObjects = terrainObjects.GetTiledObjectsIntersectingRect(targetTile.GetRect().CloneWithExpansion(-2.0f));

        string canWalkValue = "";
        bool canWalk = false;
        bool canWalkFound = false;

        foreach (IPTiledObject tileObject in tileObjects)
        {
            //if the object says it can be walked on, set canWalk to true
            if (tileObject.PropertyExists(CAN_WALK_PROPERTY))
            {
                canWalkValue = tileObject.GetPropertyValue(CAN_WALK_PROPERTY);

                //just use first object we find that contains a valid CAN_WALK property value then break out
                if (bool.TryParse(canWalkValue, out canWalk))
                {
                    canWalkFound = true;
                    break;
                }
            }
        }

        //only check the tile CAN_WALK if we didn't find any objects with walkable definitions
        //objects are exceptions that override tile CAN_WALK rules
        if (!canWalkFound)
        {
            //check the canWalk value of the tile
            IPTileData targetTileData = targetTile.TileData;

            canWalkValue = targetTileData.GetPropertyValue(CAN_WALK_PROPERTY);

            bool.TryParse(canWalkValue, out canWalk);
        }

        return canWalk;
    }
}
