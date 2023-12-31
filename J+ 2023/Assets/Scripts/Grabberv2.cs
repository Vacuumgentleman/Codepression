using UnityEngine;
using System.Collections;

public class Grabberv2 : MonoBehaviour
{
    private GameObject selectedObject; // El objeto seleccionado actualmente.
    private Vector3 originalPosition; // La posición original del objeto antes de arrastrarlo.
    private Quaternion originalRotation; // La rotación original del objeto antes de arrastrarlo.
    private bool isDragging; // Indica si se está arrastrando un objeto.

    [SerializeField] private AudioSource audioSource; // Referencia al componente AudioSource para reproducir sonidos.
    [SerializeField] private AudioClip pickupClip; // Sonido de recogida.
    [SerializeField] private AudioClip dropClip; // Sonido de soltar.
    [SerializeField] private AudioClip incorrectPlacementClip; // Sonido de pieza colocada incorrectamente.
    [SerializeField] private AudioClip rotationClip; // Sonido de rotación.
    [SerializeField] private float dropDistanceThreshold = 0.5f; // Define la distancia máxima permitida para soltar.

    private void Update()
    {
        if (isDragging)
        {
            // Actualiza la posición del objeto seleccionado según la posición del ratón.
            // También permite rotar el objeto con el botón derecho del ratón.
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(selectedObject.transform.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            selectedObject.transform.position = new Vector3(worldPosition.x, 0.3f, worldPosition.z);

            if (Input.GetMouseButtonDown(1))
            {
                // Rota el objeto seleccionado en 45 grados alrededor del eje Y.
                selectedObject.transform.rotation = Quaternion.Euler(new Vector3(
                    selectedObject.transform.rotation.eulerAngles.x,
                    selectedObject.transform.rotation.eulerAngles.y + 45f,
                    selectedObject.transform.rotation.eulerAngles.z));

                // Reproduce el sonido de rotación.
                audioSource.PlayOneShot(rotationClip);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                // Lanza un rayo para determinar qué objeto se ha hecho clic.
                RaycastHit hit = CastRay();
                if (hit.collider != null)
                {
                    selectedObject = hit.collider.gameObject;

                    // Solo permite arrastrar fichas "drag," no "slot."
                    if (!selectedObject.CompareTag("drag"))
                    {
                        selectedObject = null;
                        return;
                    }

                    Cursor.visible = false;
                    originalPosition = selectedObject.transform.position;
                    originalRotation = selectedObject.transform.rotation;

                    isDragging = true;

                    // Reproduce el sonido de recogida.
                    audioSource.PlayOneShot(pickupClip);
                }
            }
            else
            {
                // Comprueba si el objeto "drag" se corresponde con un "slot" por nombre.
                if (selectedObject.CompareTag("drag"))
                {
                    string[] splitName = selectedObject.name.Split('e');
                    int dragNumber = int.Parse(splitName[1]);
                    string slotName = "Surface" + dragNumber;

                    Transform slot = GameObject.Find(slotName).transform;
                    if (slot != null)
                    {
                        // Verifica si la rotación es adecuada antes de soltar la ficha.
                        //if (IsRotationValid(selectedObject, slot))
                        //{
                            // Verifica si la ficha está lo suficientemente cerca del "slot" antes de soltarla.
                            float distanceToSlot = Vector3.Distance(selectedObject.transform.position, slot.position);
                            if (distanceToSlot <= dropDistanceThreshold)
                            {
                                // Verifica si la rotación es adecuada antes de soltar la ficha.
                                if (IsRotationValid(selectedObject, slot))
                                {
                                    // Ajusta la posición del objeto al "slot" antes de bloquearlo.
                                    selectedObject.transform.position = slot.position;

                                    // Desactiva los Colliders del objeto para que no pueda interactuarse más.
                                    Collider[] colliders = selectedObject.GetComponentsInChildren<Collider>();
                                    foreach (Collider collider in colliders)
                                    {
                                        collider.enabled = false;
                                    }

                                    // Desactiva el Renderer del objeto del slot para ocultarlo.
                                    Renderer slotRenderer = slot.GetComponent<Renderer>();
                                    if (slotRenderer != null){
                                        slotRenderer.enabled = false;
                                    }

                                    isDragging = false;
                                    selectedObject = null;
                                    Cursor.visible = true;

                                    // Verificar si todos los colliders están desactivados.
                                    CheckColliders();

                                    // Reproduce el sonido de soltar.
                                    audioSource.PlayOneShot(dropClip);
                                }
                            }
                            else
                            {
                                // Reproduce el sonido de pieza colocada incorrectamente.
                                audioSource.PlayOneShot(incorrectPlacementClip);

                                selectedObject.transform.position = originalPosition;
                                selectedObject.transform.rotation = originalRotation;
                                selectedObject = null;
                                Cursor.visible = true;
                                isDragging = false;
                            }
                        //}
                    }
                }
            }
        }
    }

    private RaycastHit CastRay()
    {
        // Lanza un rayo desde la posición del ratón para determinar qué objeto se ha hecho clic.
        // Devuelve la información sobre el objeto impactado por el rayo.
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
        // Verifica si la rotación actual del objeto "drag" es adecuada en relación con la rotación del "slot."
        // Puede utilizar una lógica personalizada para determinar si la rotación es válida, por ejemplo, un ángulo máximo permitido.
        float maxRotationDifference = 10f; // Define el ángulo máximo permitido.
        Quaternion dragRotation = dragObject.transform.rotation;
        Quaternion slotRotation = slot.rotation;
        float rotationDifference = Quaternion.Angle(dragRotation, slotRotation);
        // Comprueba si la diferencia de rotación es menor o igual que el ángulo máximo permitido.
        return rotationDifference <= maxRotationDifference;
    }

    private void CheckColliders()
    {
        // Obtener todos los objetos en la escena con el tag "drag."
        GameObject[] dragObjects = GameObject.FindGameObjectsWithTag("drag");

        // Variable para rastrear si todos los colliders están desactivados.
        bool allCollidersDisabled = true;

        // Iterar a través de los objetos "drag" y verificar si sus colliders están desactivados.
        foreach (GameObject dragObject in dragObjects)
        {
            Collider[] colliders = dragObject.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider.enabled)
                {
                    // Al menos un collider está habilitado, así que no todos los colliders están desactivados.
                    allCollidersDisabled = false;
                    break;
                }
            }
        }

        // Si todos los colliders están desactivados, realiza la animación.
        if (allCollidersDisabled)
        {
            Debug.Log("Todos los colliders están desactivados.");

            // Encuentra el objeto vacío en el centro.
            GameObject centroDeMasa = GameObject.Find("CentroDeMasa");

            // Mueve las piezas al centro.
            foreach (GameObject dragObject in dragObjects)
            {
                StartCoroutine(MovePiecesToCenter(dragObject.transform.parent, centroDeMasa.transform.position));
            }
        }
    }

private IEnumerator MovePiecesToCenter(Transform parentTransform, Vector3 centerPosition)
{
    float duration = 2.0f; // Duración de la animación de movimiento.
    float elapsedTime = 0.0f;

    Vector3 initialPosition = parentTransform.position;
    Vector3 targetPosition = centerPosition;
    targetPosition.x = -5.0f; // Mueve el objeto padre a x = -5.

    while (elapsedTime < duration)
    {
        parentTransform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    parentTransform.position = targetPosition;
}

}
