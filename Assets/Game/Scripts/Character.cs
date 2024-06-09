using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	// H�z ve z�plama g�c� parametreleri
	[SerializeField] Animator anim;
	public float moveSpeed = 5f;
	public float jumpForce = 5f;
	public float mouseXSpeed = 5f;
	public float mouseYSpeed = 5f;


	// Yer �ekimi ve karakter kontrolc� referanslar�
	[SerializeField] bool isGrounded;
	private float gravity = -9.81f;
	[SerializeField] private Vector3 velocity;
	private CharacterController controller;

	[SerializeField] GameObject cam;

	public float maxLookAngle = 90f; // Maksimum bak�� a��s�

	private void Start()
	{
		// Karakter kontrolc� referans�n� al
		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		MoveUpdate();

		Debug.Log(cam.transform.rotation.x);
	}
	private void MoveUpdate()
	{
		// Yatay ve dikey hareket vekt�rleri
		float horizontalMove = Input.GetAxis("Horizontal");
		float verticalMove = Input.GetAxis("Vertical");

		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
		{
			Walk();
		}
		else
		{
			AllFalse();
		}

		// Check if the character is grounded
		isGrounded = controller.isGrounded;
		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f; // Ensure the character stays grounded
		}

		// Yatay ve dikey hareket vekt�rlerini birle�tir
		Vector3 move = transform.right * horizontalMove + transform.forward * verticalMove;

		// Yer�ekimini hesapla
		velocity.y += gravity * Time.deltaTime;

		// Hareket vekt�r�n� karakter kontrolc�s�ne uygula
		controller.Move(move * moveSpeed * Time.deltaTime);

		// Check for jump input and apply jump force
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
		}

		// Apply gravity
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);

		float rotX = Mathf.Clamp(cam.transform.rotation.x, -0.2f, 0.2f);

		//Mouse rotate
		transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * mouseXSpeed, 0);
		//cam.transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * mouseYSpeed, 0, 0);



		// Fare eksenlerini al
		float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseYSpeed;

		// Nesnenin d�nme a��s�n� hesapla
		float newRotationX = cam.transform.localEulerAngles.x - mouseY;

		// A��r� d�nme durumunu d�zelt
		if (newRotationX > 180)
			newRotationX -= 360;

		// Maksimum ve minimum bak�� a��s�n� s�n�rla
		newRotationX = Mathf.Clamp(newRotationX, -maxLookAngle, maxLookAngle);

		// Yeni d�nme a��s�n� uygula
		cam.transform.localEulerAngles = new Vector3(newRotationX, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
	}
	public void AllFalse()
	{
		anim.SetBool("walk", false);
	}
	public void Walk()
	{
		anim.SetBool("walk", true);
	}
}