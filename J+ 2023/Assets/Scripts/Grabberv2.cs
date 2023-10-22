using UnityEngine;

public class Grabberv2 : MonoBehaviour {

    private GameObject selectedObject;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isDragging;

    [SerializeField] private AudioSource audioSource; // Agrega un AudioSource desde el Inspector

    [SerializeField] private AudioClip pickupClip; // Agrega un AudioClip para el sonido de recogida desde el Inspector
    [SerializeField] private AudioClip dropClip; // Agrega un AudioClip para el sonido de soltar desde el Inspector

    private void Update() {
        if (isDragging) {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(selectedObject.transform.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            selectedObject.transform.position = new Vector3(worldPosition.x, 0f, worldPosition.z);

            if (Input.GetMouseButtonDown(1)) {
                selectedObject.transform.rotation = Quaternion.Euler(new Vector3(
                    selectedObject.transform.rotation.eulerAngles.x,
                    selectedObject.transform.rotation.eulerAngles.y + 45f,
                    selectedObject.transform.rotation.eulerAngles.z));
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            if (selectedObject == null) {
                RaycastHit hit = CastRay();
                if (hit.collider != null) {
                    if (!hit.collider.CompareTag("drag")) {
                        return;
                    }

                    selectedObject = hit.collider.gameObject;
                    Cursor.visible = false;
                    originalPosition = selectedObject.transform.position;
                    originalRotation = selectedObject.transform.rotation;

                    isDragging = true;
                    
                    // Reproduce el sonido de recogida
                    audioSource.PlayOneShot(pickupClip);
                }
            }
            else {
                selectedObject.transform.position = originalPosition;
                selectedObject.transform.rotation = originalRotation;
                selectedObject = null;
                Cursor.visible = true;
                isDragging = false;

                // Reproduce el sonido de soltar
                audioSource.PlayOneShot(dropClip);
            }
        }
    }

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
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
}
