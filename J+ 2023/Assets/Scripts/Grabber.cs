using UnityEngine;

public class Grabber : MonoBehaviour {

    // Almacena el objeto actualmente seleccionado para ser arrastrado.
    private GameObject selectedObject;

    private void Update() {
        // Arrastrar con clic izquierdo
        if (Input.GetMouseButtonDown(0)) {
            if(selectedObject == null) {
                // Realiza un rayo desde la cámara para determinar si se hizo clic en un objeto etiquetado como "drag".
                RaycastHit hit = CastRay();

                if(hit.collider != null) {
                    if (!hit.collider.CompareTag("drag")) {
                        return; // Si no es un objeto válido para arrastrar, salimos de la función.
                    }

                    // Selecciona el objeto y oculta el cursor del mouse.
                    selectedObject = hit.collider.gameObject;
                    Cursor.visible = false;
                }
            } else {
                // Mueve el objeto seleccionado siguiendo el puntero del mouse en el plano del juego.
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                selectedObject.transform.position = new Vector3(worldPosition.x, 0f, worldPosition.z);

                // Suelta el objeto y muestra nuevamente el cursor del mouse.
                selectedObject = null;
                Cursor.visible = true;
            }
        }

        if(selectedObject != null) {
            // Continúa siguiendo el puntero del mouse en el plano del juego.
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedObject.transform.position = new Vector3(worldPosition.x, .25f, worldPosition.z);

            // Si se hace clic derecho, rota el objeto seleccionado 90 grados alrededor del eje Y.
            if (Input.GetMouseButtonDown(1)) {
                selectedObject.transform.rotation = Quaternion.Euler(new Vector3(
                    selectedObject.transform.rotation.eulerAngles.x,
                    selectedObject.transform.rotation.eulerAngles.y + 45f,
                    selectedObject.transform.rotation.eulerAngles.z));
            }
        }
    }

    // Realiza un rayo desde la cámara en la dirección del puntero del mouse.
    private RaycastHit CastRay() {
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        // Realiza el raycast y almacena la información de la colisión en 'hit'.
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
}
