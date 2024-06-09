using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

	[SerializeField] Transform rayTarget;
	[SerializeField] List<GameObject> rayHits;

	private void Start()
	{
		// Karakter kontrolc� referans�n� al
		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		MoveUpdate();
		RaycastUpdate();

		Debug.Log(cam.transform.rotation.x);
	}
	private void MoveUpdate()
	{
		// Yatay ve dikey hareket vekt�rleri
		float horizontalMove = Input.GetAxis("Horizontal");
		float verticalMove = Input.GetAxis("Vertical");

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
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
	private void RaycastUpdate()
	{
		// A objesinin pozisyonu
		Vector3 origin = cam.transform.position;

		// B objesinin pozisyonu
		Vector3 direction = (rayTarget.position - origin).normalized;

		// RaycastHit bilgilerini tutmak i�in
		RaycastHit hit;

		// Raycast'i ger�ekle�tir
		if (Physics.Raycast(origin, direction, out hit))
		{
			Debug.Log("Hit: " + hit.collider.name);

			// �izgiyi sahnede g�rmek i�in
			Debug.DrawRay(origin, direction * hit.distance, Color.red);

			if (hit.collider.CompareTag("wall"))
			{
				AddRayHits(hit.collider.gameObject);
			}
			else RemoveRayHits();
		}
		else
		{
			// Hit olmazsa bile �izgiyi bir mesafeye kadar �iz
			Debug.DrawRay(origin, direction * 100, Color.red);
		}
	}
	private void AddRayHits(GameObject go)
	{
		bool check = false;
		for (int i = 0; i < rayHits.Count; i++)
		{
			if (rayHits[i] == go)
			{
				check = true;
			}
			//else rayHits[i].GetComponent<MeshRenderer>().enabled = true;
		}

		if (check) return;

		go.GetComponent<MeshRenderer>().enabled = false;
		rayHits.Add(go);
		//By hari� hepsini listeden sil
		RemoveRayHits(go);
	}
	private void RemoveRayHits()
	{
		for (int i = 0; i < rayHits.Count; i++)
		{
			rayHits[i].GetComponent<MeshRenderer>().enabled = true;
			rayHits.RemoveAt(i);
		}
	}
	private void RemoveRayHits(GameObject go)
	{
		for (int i = 0; i < rayHits.Count; i++)
		{
			if (rayHits[i] != go)
			{
				rayHits[i].GetComponent<MeshRenderer>().enabled = true;
				rayHits.RemoveAt(i);
			}
		}
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