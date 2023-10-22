using UnityEngine;

public class Grabberv2 : MonoBehaviour
{
    private GameObject selectedObject;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isDragging;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private float dropDistanceThreshold = 0.5f; // Define la distancia máxima permitida para soltar.

    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(selectedObject.transform.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            selectedObject.transform.position = new Vector3(worldPosition.x, 0f, worldPosition.z);

            if (Input.GetMouseButtonDown(1))
            {
                selectedObject.transform.rotation = Quaternion.Euler(new Vector3(
                    selectedObject.transform.rotation.eulerAngles.x,
                    selectedObject.transform.rotation.eulerAngles.y + 45f,
                    selectedObject.transform.rotation.eulerAngles.z));
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                RaycastHit hit = CastRay();
                if (hit.collider != null)
                {
                    selectedObject = hit.collider.gameObject;
                    
                    // Solo permite arrastrar fichas "drag," no "slot."
                    if (!selectedObject.name.StartsWith("polySurface"))
                    {
                        selectedObject = null;
                        return;
                    }

                    Cursor.visible = false;
                    originalPosition = selectedObject.transform.position;
                    originalRotation = selectedObject.transform.rotation;

                    isDragging = true;

                    // Reproduce el sonido de recogida
                    audioSource.PlayOneShot(pickupClip);
                }
            }
            else
            {
                // Comprueba si el objeto "drag" se corresponde con un "slot" por nombre.
                if (selectedObject.name.StartsWith("polySurface"))
                {
                    string[] splitName = selectedObject.name.Split('e');
                    int dragNumber = int.Parse(splitName[1]);
                    string slotName = "Surface" + dragNumber;

                    Transform slot = GameObject.Find(slotName).transform;
                    if (slot != null)
                    {
                        // Verifica si la rotación es adecuada antes de soltar la ficha.
                        if (IsRotationValid(selectedObject, slot))
                        {
                            // Verifica si la ficha está lo suficientemente cerca del "slot" antes de soltarla.
                            float distanceToSlot = Vector3.Distance(selectedObject.transform.position, slot.position);
                            if (distanceToSlot <= dropDistanceThreshold)
                            {
                                selectedObject.transform.position = slot.position;
                                isDragging = false;
                                selectedObject = null;
                                Cursor.visible = true;

                                // Reproduce el sonido de soltar
                                audioSource.PlayOneShot(dropClip);
                            }
                            else
                            {
                                selectedObject.transform.position = originalPosition;
                                selectedObject.transform.rotation = originalRotation;
                                selectedObject = null;
                                Cursor.visible = true;
                                isDragging = false;
                            }
                        }
                    }
                }
            }
        }
    }

    private RaycastHit CastRay()
    {
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

    private bool IsRotationValid(GameObject dragObject, Transform slot)
    {
        // Agrega aquí la lógica para verificar si la rotación es adecuada.
        // Puedes comparar la rotación actual del objeto "drag" con la rotación del "slot."

        // Por ejemplo, puedes usar un ángulo máximo permitido:
        float maxRotationDifference = 10f; // Define el ángulo máximo permitido.

        Quaternion dragRotation = dragObject.transform.rotation;
        Quaternion slotRotation = slot.rotation;

        // Calcula la diferencia de rotación entre el objeto "drag" y el "slot."
        float rotationDifference = Quaternion.Angle(dragRotation, slotRotation);

        // Comprueba si la diferencia de rotación es menor o igual que el ángulo máximo permitido.
        return rotationDifference <= maxRotationDifference;
    }
}
