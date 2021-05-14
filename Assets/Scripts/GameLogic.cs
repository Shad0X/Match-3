using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    //has a lot more than just Logic.. also takes care of entire game grid and all the functionality that affects it.. etc, etc.. 
    [SerializeField]
    private int fieldWidth;
    
    [SerializeField]
    private int fieldHeight;

    [SerializeField]
    private GameObject tileFieldPrefab;

    private TileArrayIndex[,] tileGameObjects;
    private GameObject tileParentObject;

    [SerializeField]
    private GameObject gameBackground;

    [SerializeField]
    [Tooltip("1 of each Tile Type that should be used to populate the tile field")]
    private List<GameObject> tileVariations;

    void Start() {
        tileGameObjects = new TileArrayIndex[fieldWidth, fieldHeight];
        tileParentObject = new GameObject("Tiles");

        SetGameBackgroundSize(fieldWidth, fieldHeight);
        FillFieldWithRandomTiles();
    }

    private void SetGameBackgroundSize(int width, int height) {
        gameBackground.transform.localScale = new Vector3(width, height, gameBackground.transform.localScale.z);
    }

    private void FillFieldWithRandomTiles() {
        for (int x = 0; x < fieldWidth; x++) {
            for (int y = 0; y < fieldHeight; y++) {

                tileGameObjects[x, y] = GenerateTileArrayPositionAt(x, y);
                tileGameObjects[x, y].transform.position = GetTileGameworldLocation(x, y);
            }
        }
    }

    private Vector3 GetTileGameworldLocation(int x, int y) {
        float xOffset = 0;
        float yOffset = 0;
        
        //using offSet in case if game field size consists of unevent numbers
        if (fieldWidth % 2 == 0) {
            xOffset = 0.5f; 
        }
        if (fieldHeight % 2 == 0) {
            yOffset = 0.5f;
        }

        float xLocation = x - fieldWidth / 2 + xOffset;
        float yLocation = y - fieldHeight / 2 + yOffset;

        return new Vector3(xLocation, yLocation, gameBackground.transform.position.z);
    }

    private void SetupGameField()
    {
        //instantiate Empty GameObject Field WIDTH x HEIGHT times 
        // assign TileArrayIndex to each 1 (will never change -- meaning can be public static Vector2Int ?)

    }

    //ToDo - Generate a field where there are no Matches (3 or more tiles of same type in a horizontal line)
    private TileArrayIndex GenerateTileArrayPositionAt(int x, int y) {
        int tileIndex = Random.Range(0, tileVariations.Count);//0 magic nr?
        GameObject tileObject = Instantiate(tileVariations[tileIndex], tileParentObject.transform);

        TileArrayIndex tile = tileObject.AddComponent<TileArrayIndex>();
        tile.SetValue(x, y);

        return tile;
    }

    public void RemoveTiles(GameObject tile) {//could use better name.. wont always be multiples, wont always be a single 1.. 
        TileArrayIndex tileArrayIndex = tile.GetComponent<TileArrayIndex>();//problematic, since it accepts a GameObject, but has no mentions of making use of TileArrayIndex scripts.. 
        RemoveMatchingTilesAroundPosition(tileArrayIndex.x, tileArrayIndex.y);
    }

    private void RemoveTileAt(int x, int y) {
        tileGameObjects[x, y].transform.gameObject.SetActive(false);
        MoveAllTilesAboveThisPositionDown(x, y);
    }

    private void MoveAllTilesAboveThisPositionDown(int x, int y) {
        if (y + 1 >= fieldHeight) {
            return;
        }

        TileArrayIndex tileAbove = tileGameObjects[x, y + 1];
        if (tileAbove == null) {
            return;
        }

        MoveTileDownOneField(x, y + 1);
        
        MoveAllTilesAboveThisPositionDown(x, y + 1);
    }

    private void MoveTileDownOneField(int x, int y) {
        TileArrayIndex tile = tileGameObjects[x, y];
        tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y - 1, tile.transform.position.z);
        tile.SetValue(x, y - 1);

        tileGameObjects[x, y - 1] = tile;
        tileGameObjects[x, y] = null;
    }

    private void RemoveMatchingTilesAroundPosition(int x, int y) {
        string expectedName = tileGameObjects[x, y].name;
        RemoveTileAt(x, y);

        int matchingTileCount = GetHorizontallyMatchingTileCount(x, y, expectedName);
        if (matchingTileCount >= 2) {
            RemoveHorizontallyMatchingTiles(x, y, expectedName);
        }
    }

    private int GetHorizontallyMatchingTileCount(int x, int y, string expectedName) {
        int count = 0;

        count += GetMatchingTileCountOnLeft(x, y, expectedName);
        count += GetMatchingTileCountOnRight(x, y, expectedName);

        return count;
    }

    private int GetMatchingTileCountOnLeft(int x, int y, string expectedName) {
        int count = 0;

        if (DoesTileGameObjectNameMatch(x - 1, y, expectedName)) {
            count += 1;
            count += GetMatchingTileCountOnLeft(x - 1, y, expectedName);
        }

        return count;
    }

    private int GetMatchingTileCountOnRight(int x, int y, string expectedName) {
        int count = 0;

        if (DoesTileGameObjectNameMatch(x + 1, y, expectedName)) {
            count += 1;
            count += GetMatchingTileCountOnRight(x + 1, y, expectedName);
        }

        return count;
    }

    private void RemoveHorizontallyMatchingTiles(int x, int y, string expectedName) {
        RemoveLeftTilesIfPossible(x, y, expectedName);
        RemoveRightTilesIfPossible(x, y, expectedName);
    }

    private void RemoveLeftTilesIfPossible(int x, int y, string expectedName) {
        ArrayIndex leftTileIndex = new ArrayIndex(x - 1, y);

        if (DoesTileGameObjectNameMatch(leftTileIndex.x, leftTileIndex.y, expectedName)) {
            RemoveTileAt(leftTileIndex.x, leftTileIndex.y);
            RemoveLeftTilesIfPossible(leftTileIndex.x, leftTileIndex.y, expectedName);
        }
    }

    private void RemoveRightTilesIfPossible(int x, int y, string expectedName) {
        ArrayIndex rightTileIndex = new ArrayIndex(x + 1, y);

        if (DoesTileGameObjectNameMatch(rightTileIndex.x, rightTileIndex.y, expectedName)) {
            RemoveTileAt(rightTileIndex.x, rightTileIndex.y);
            RemoveRightTilesIfPossible(rightTileIndex.x, rightTileIndex.y, expectedName);
        }
    }

    private bool DoesTileGameObjectNameMatch(int x, int y, string expectedName) {
        return IsXLocationInsideField(x)
                && tileGameObjects[x, y] != null
                && tileGameObjects[x, y].name.Equals(expectedName);
    }
    
    private bool IsXLocationInsideField(int x) {
        return x >= 0 && x < fieldWidth;
    }

}
