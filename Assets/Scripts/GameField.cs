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

#if UNITY_EDITOR
    public void SetTestData(int fieldHeight, int fieldWidth, GameObject tileFieldPrefab, GameObject gameBackground) {
        this.fieldWidth = fieldWidth;
        this.fieldHeight = fieldHeight;
        this.tileFieldPrefab = tileFieldPrefab;
        this.gameBackground = gameBackground;
    }
#endif

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
        SetGameBackgroundSize(fieldWidth, fieldHeight);
    }

    private void SetupGameField() {
        for (int x = 0; x < fieldWidth; x++) {
            for (int y = 0; y < fieldHeight; y++) {
                CreateTileFieldObjectAt(x, y);
            }
        }
    }

    private void SetGameBackgroundSize(int width, int height) {
        gameBackground.transform.localScale = new Vector3(width, height, gameBackground.transform.localScale.z);
    }

    private void CreateTileFieldObjectAt(int x, int y) {
        GameObject field = Instantiate(tileFieldPrefab, transform);
        field.transform.position = GetTileGameworldLocation(x, y);

        TileArrayIndex tile = field.AddComponent<TileArrayIndex>();//ToDo - refactor to use Vector2Int instead
        tile.SetValue(x, y);
    }

    public Vector3 GetTileGameworldLocation(int row, int column)  {//ToDo - maybe move to Helper class instead ? Used by Both Fields and Tiles
        //possibly pass in Field width and height to make it Static ?
        float xOffset = 0;
        float yOffset = 0;

        //using offSet in case if game field size consists of unevent numbers
        if (fieldWidth % 2 == 0) {
            xOffset = 0.5f;
        }
        if (fieldHeight% 2 == 0) {
            yOffset = 0.5f;
        }

        float rowLocation = row - fieldHeight / 2 + xOffset;
        float columnLocation = column - fieldWidth / 2 + yOffset;

        return new Vector3(columnLocation, -rowLocation, gameBackground.transform.position.z);//move to parameters to make static?
    }
}
