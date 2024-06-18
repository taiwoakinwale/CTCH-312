using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Vector2 _input; 
    private CharacterController _characterController;
    private Vector3 _direction;

    [SerializeField] private float smoothTime = 0.05f;
    private float _currentVelocity;
    
    [SerializeField] private float speed;
    public bool immobilized;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        immobilized = false;
    }

    // The update moves the player if there have been inputs for the player to move since the last update
    private void Update()
    {
        if (_input.sqrMagnitude == 0 || immobilized) return;
        
        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        
        _characterController.Move(_direction * speed * Time.deltaTime);
    }

    // Funtion to help interpret the inputs and to allow the player to move
    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0.0f, _input.y);
    }
}