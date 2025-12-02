// BasicFPSController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicFPSController : MonoBehaviour
{
    [Header("Камера игрока")]
    public Camera playerCamera;
    
    [Header("Настройки движения")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 7f;
    public float mouseSensitivity = 2f;
    
    [Header("Настройки камеры")]
    public float lookLimit = 90f;
    
    // Компоненты
    private CharacterController characterController;
    private Vector3 velocity;
    private float rotationX = 0;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Блокируем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("🎮 FPS Controller запущен");
    }
    
    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }
    
    void HandleMovement()
    {
        // Проверяем землю
        bool isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        // Движение WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        
        // Бег/ходьба
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // Применяем движение
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        
        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }
        
        // Гравитация
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Вертикальный поворот камеры
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookLimit, lookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        
        // Горизонтальный поворот игрока
        transform.Rotate(Vector3.up * mouseX);
    }
}