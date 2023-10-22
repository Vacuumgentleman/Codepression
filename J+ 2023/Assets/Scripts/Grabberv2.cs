using UnityEngine;

public class Grabberv2 : MonoBehaviour
{
    // Variables para el objeto seleccionado y su estado de arrastre
    private GameObject selectedObject;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isDragging;

    // Sonidos y distancia máxima para soltar
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private float dropDistanceThreshold = 0.5f; // Define la distancia máxima permitida para soltar.

    // Método Update se llama en cada fotograma
    private void Update()
    {
        // Lógica para objetos arrastrados
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;

            // Convierte la posición del ratón en coordenadas del mundo
            mousePosition.z = Camera.main.WorldToScreenPoint(selectedObject.transform.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            selectedObject.transform.position = new Vector3(worldPosition.x, 0f, worldPosition.z);

            // Giro de objeto con clic derecho
            if (Input.GetMouseButtonDown(1))
            {
                selectedObject.transform.rotation = Quaternion.Euler(new Vector3(
                    selectedObject.transform.rotation.eulerAngles.x,
                    selectedObject.transform.rotation.eulerAngles.y + 45f,
                    selectedObject.transform.rotation.eulerAngles.z));
            }
        }

        // Lógica para seleccionar y soltar objetos con clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            // Si no hay un objeto seleccionado
            if (selectedObject == null)
            {
                // Lanza un rayo para determinar si se hizo clic en un objeto
                RaycastHit hit = CastRay();

                // Si se golpea un objeto válido, lo selecciona
                if (hit.collider != null)
                {
                    selectedObject = hit.collider.gameObject;

                    // Comprueba si el objeto es "drag," no "slot"
                    if (!selectedObject.name.StartsWith("polySurface"))
                    {
                        selectedObject = null;
                        return;
                    }

                    // Oculta el cursor, guarda la posición y rotación originales y reproduce un sonido
                    Cursor.visible = false;
                    originalPosition = selectedObject.transform.position;
                    originalRotation = selectedObject.transform.rotation;
                    isDragging = true;
                    audioSource.PlayOneShot(pickupClip);
                }
            }
            else // Si ya hay un objeto seleccionado
            {
                // Comprueba si el objeto "drag" se corresponde con un "slot" por nombre
                if (selectedObject.name.StartsWith("polySurface"))
                {
                    // Extrae el número del objeto y encuentra el "slot" correspondiente
                    string[] splitName = selectedObject.name.Split('e');
                    int dragNumber = int.Parse(splitName[1]);
                    string slotName = "Surface" + dragNumber;
                    Transform slot = GameObject.Find(slotName).transform;

                    // Verifica si la rotación es adecuada y si la distancia al "slot" es menor o igual a la distancia máxima permitida
                    if (IsRotationValid(selectedObject, slot))
                    {
                        float distanceToSlot = Vector3.Distance(selectedObject.transform.position, slot.position);
                        if (distanceToSlot <= dropDistanceThreshold)
                        {
                            selectedObject.transform.position = slot.position;
                            isDragging = false;
                            selectedObject = null;
                            Cursor.visible = true;
                            audioSource.PlayOneShot(dropClip);
                        }
                        else
                        {
                            // Devuelve el objeto a su posición y rotación originales
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

    // Lanza un rayo desde la posición del ratón en la pantalla y devuelve el objeto golpeado
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

    // Verifica si la rotación del objeto es adecuada antes de soltarlo en el "slot"
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
