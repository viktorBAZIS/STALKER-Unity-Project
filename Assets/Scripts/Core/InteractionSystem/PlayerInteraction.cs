using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public LayerMask interactionLayerMask = -1;
    
    private Camera playerCamera;
    private IInteractable currentInteractable;
    
    void Start()
    {
        playerCamera = Camera.main;
    }
    
    void Update()
    {
        FindInteractable();
        
        if (Input.GetKeyDown(interactionKey) && currentInteractable != null && currentInteractable.CanInteract())
        {
            currentInteractable.Interact(gameObject);
        }
    }
    
    void FindInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null && interactable.CanInteract())
            {
                currentInteractable = interactable;
                // TODO: Показать UI с текстом взаимодействия
                Debug.Log(interactable.GetInteractionText());
                return;
            }
        }
        
        currentInteractable = null;
    }
    
    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 rayEnd = playerCamera.transform.position + playerCamera.transform.forward * interactionDistance;
            Gizmos.DrawLine(playerCamera.transform.position, rayEnd);
        }
    }
}