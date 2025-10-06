using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float moveSpeed = 6f;
	[SerializeField] private float jumpForce = 8f;
	[SerializeField] private float gravity = -9f;
	[SerializeField] private Transform cameraTransform;

	[Header("Shooting")]
	[SerializeField] private Transform firePoint;
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] private float fireCooldownSeconds = 0.15f;
	
	[Header("Aiming")]
	[SerializeField] private float aimingMoveSpeedMultiplier = 0.3f;
	[SerializeField] private float aimingAccuracyMultiplier = 0.5f;
	
	[Header("Camera")]
	[SerializeField] private Camera playerCamera;
	[SerializeField] private float normalFOV = 60f;
	[SerializeField] private float aimingFOV = 45f;
	[SerializeField] private float fovTransitionSpeed = 5f;
	[SerializeField] private Vector3 normalCameraPosition = Vector3.zero;
	[SerializeField] private Vector3 aimingCameraPosition = new Vector3(0, 0, -0.2f);
	[SerializeField] private float cameraTransitionSpeed = 8f;
	
	[Header("Mouse Look")]
	[SerializeField] private float mouseSensitivity = 2f;
	[SerializeField] private float maxLookAngle = 80f;
	[SerializeField] private bool invertY = false;
	
	[Header("Crosshair")]
	[SerializeField] private GameObject crosshairUI;
	[SerializeField] private float crosshairTransitionSpeed = 10f;

	[Header("Jump")]
	[SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

	private CharacterController characterController;
	private bool isGrounded;    
	private Vector3 velocity;
	private float lastShotTime;
	private bool isAiming;
	private Vector3 originalCameraPosition;
	private float xRotation = 0f;
	private CanvasGroup crosshairCanvasGroup;

	private const float groundDistance = 0.4f;

	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		
		// Инициализация камеры
		if (playerCamera == null)
		{
			playerCamera = Camera.main;
		}
		
		if (playerCamera != null)
		{
			originalCameraPosition = cameraTransform.localPosition;
			playerCamera.fieldOfView = normalFOV;
		}
		
		// Инициализация прицела
		if (crosshairUI != null)
		{
			crosshairCanvasGroup = crosshairUI.GetComponent<CanvasGroup>();
			if (crosshairCanvasGroup == null)
			{
				crosshairCanvasGroup = crosshairUI.AddComponent<CanvasGroup>();
			}
			crosshairCanvasGroup.alpha = 0f;
		}
	}

	private void Update()
	{
		HandleMouseLook();
		HandleAiming();
		HandleCamera();
		HandleCrosshair();
		HandleMovement();
		HandleShooting();
		HandleJump();
	}

	private void HandleMouseLook()
	{
		if (cameraTransform == null) return;
		

		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
		

		if (invertY)
		{
			mouseY = -mouseY;
		}
		

		transform.Rotate(Vector3.up * mouseX);
		

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
		cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
	}
	
	private void HandleAiming()
	{
		isAiming = Input.GetButton("Fire2");
	}
	
	private void HandleCamera()
	{
		if (playerCamera == null || cameraTransform == null) return;
		
		float targetFOV = isAiming ? aimingFOV : normalFOV;
		playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
		
		Vector3 targetPosition = isAiming ? 
			originalCameraPosition + aimingCameraPosition : 
			originalCameraPosition;
		
		cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, cameraTransitionSpeed * Time.deltaTime);
	}
	
	private void HandleCrosshair()
	{
		if (crosshairCanvasGroup == null) return;
		
		float targetAlpha = isAiming ? 1f : 0f;
		crosshairCanvasGroup.alpha = Mathf.Lerp(crosshairCanvasGroup.alpha, targetAlpha, crosshairTransitionSpeed * Time.deltaTime);
	}
	
	private void HandleMovement()
	{
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        float currentMoveSpeed = isAiming ? moveSpeed * aimingMoveSpeedMultiplier : moveSpeed;
        characterController.Move(move * currentMoveSpeed * Time.deltaTime);
        
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

	private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

	private void HandleShooting()
	{
		if (projectilePrefab == null || firePoint == null) return;
		if (Time.time - lastShotTime < fireCooldownSeconds) return;

		if (Input.GetButton("Fire1"))
		{
			lastShotTime = Time.time;
			
			GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
			
			Projectile projectileScript = projectile.GetComponent<Projectile>();
			if (projectileScript != null)
			{
				projectileScript.Initialize(transform, isAiming);
			}
		}
	}
} 