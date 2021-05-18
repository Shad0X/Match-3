using UnityEngine;

public class PlayerInput : MonoBehaviour {

    Camera mainCamera;
    void Start() {
        gameLogic = GetComponent<GameLogic>();
        mainCamera = Camera.main;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RemoveClickedTitle();
        }
    }

    private RaycastHit raycastHit;

    private void RemoveClickedTitle() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out raycastHit)) {
            GameObject tile = raycastHit.transform.gameObject;
            if (tile != null) {
                TileArrayIndex fieldIndex = tile.GetComponent<TileArrayIndex>();
                RemoveTile(fieldIndex.x, fieldIndex.y);
            }
        }
    }

    private GameLogic gameLogic;

    private void RemoveTile(int row, int column) {
        gameLogic.RemoveTiles(row, column);
    }
    
}
