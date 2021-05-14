using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField]
    private int fieldWidth;

    [SerializeField]
    private int fieldHeight;

    [SerializeField]
    private GameObject tileFieldPrefab;

    [SerializeField]
    private GameObject gameBackground;

    public int Width
    {
        get { return fieldWidth; }
    }

    public int Height
    {
        get { return fieldHeight; }
    }

    void Start() {
        SetupGameField();
    }
    private void SetupGameField() {
        for (int x = 0; x < fieldWidth; x++) {
            for (int y = 0; y < fieldHeight; y++) {
                CreateTileFieldObjectAt(x, y);
            }
        }
    }

    private void CreateTileFieldObjectAt(int x, int y) {
        GameObject field = Instantiate(tileFieldPrefab, transform);
        field.transform.position = GetTileGameworldLocation(x, y);

        TileArrayIndex tile = field.AddComponent<TileArrayIndex>();//ToDo - refactor to use Vector2Int instead
        tile.SetValue(x, y);
    }

    //COPY PASTE FROM GameLogic.cs ...
    private Vector3 GetTileGameworldLocation(int x, int y)  {//ToDo - move to Helper class. Used by Both Fields and Tiles
        //pass in Field width and height to make it Static ?
        float xOffset = 0;
        float yOffset = 0;

        //using offSet in case if game field size consists of unevent numbers
        if (fieldWidth % 2 == 0) {
            xOffset = 0.5f;
        }
        if (fieldHeight% 2 == 0) {
            yOffset = 0.5f;
        }

        float xLocation = x - fieldWidth / 2 + xOffset;
        float yLocation = y - fieldHeight / 2 + yOffset;

        return new Vector3(xLocation, yLocation, gameBackground.transform.position.z);//move to parameters to make static
    }
}
