using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	// Hýz ve zýplama gücü parametreleri
	public float moveSpeed = 5f;
	public float jumpForce = 5f;
	public float mouseSpeed = 5f;
	

	// Yer çekimi ve karakter kontrolcü referanslarý
	[SerializeField] bool isGrounded;
	private float gravity = -9.81f;
	[SerializeField] private Vector3 velocity;
	private CharacterController controller;

	private void Start()
	{
		// Karakter kontrolcü referansýný al
		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		MoveUpdate();


	}
	private void MoveUpdate()
	{
		// Yatay ve dikey hareket vektörleri
		float horizontalMove = Input.GetAxis("Horizontal");
		float verticalMove = Input.GetAxis("Vertical");

		// Check if the character is grounded
		isGrounded = controller.isGrounded;
		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f; // Ensure the character stays grounded
		}

		// Yatay ve dikey hareket vektörlerini birleþtir
		Vector3 move = transform.right * horizontalMove + transform.forward * verticalMove;

		// Yerçekimini hesapla
		velocity.y += gravity * Time.deltaTime;

		// Hareket vektörünü karakter kontrolcüsüne uygula
		controller.Move(move * moveSpeed * Time.deltaTime);

		// Check for jump input and apply jump force
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
		}

		// Apply gravity
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);

		//Mouse rotate
		transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * mouseSpeed, 0);
	}
}
