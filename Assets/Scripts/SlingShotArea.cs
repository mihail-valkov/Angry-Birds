using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotArea : MonoBehaviour
{
    [SerializeField] private LayerMask _slingshotAreaMask;

    public bool IsWithinSlingshotArea()
    {
        //get world posisiont of the mouse
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);
        //use the physics engine to detect if the mouse is within the slingshot area
        if (Physics2D.OverlapPoint(worldPosition, _slingshotAreaMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
