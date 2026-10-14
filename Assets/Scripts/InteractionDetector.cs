using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;

    public float interactableDistance;
    public LayerMask interactableLayer;

    Vector3 interactionRotation;

    PlayerMovement playerDirection;

    private void Start()
    {
        playerDirection = GetComponentInParent<PlayerMovement>();
        CalculateRotation();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, interactionRotation * interactableDistance, Color.red);
        CalculateRotation();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, interactionRotation, interactableDistance, interactableLayer);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
            {
                interactableInRange = interactable;
            }
        }

    }

    public void CalculateRotation()
    {
        if (playerDirection._movement.x < 0)
        {
            interactionRotation = Vector3.left;
        }
        else if (playerDirection._movement.x > 0)
        {
            interactionRotation = Vector3.right;
        }
        else if (playerDirection._movement.y > 0)
        {
            interactionRotation = Vector3.up;
        }
        else if (playerDirection._movement.y < 0)
        {
            interactionRotation = Vector3.down;
        }

        //Logic that detects if we're looking up or down
        //y being more or less than 0
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.Interact();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
        }
    }
}
