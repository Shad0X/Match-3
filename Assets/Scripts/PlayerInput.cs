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
                RemoveTile(tile);
            }
        }
    }

    private GameLogic gameLogic;

    private void RemoveTile(GameObject tile) {
        gameLogic.RemoveTiles(tile);
    }
}
