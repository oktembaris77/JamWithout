using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class Character : MonoBehaviour
{
	// H�z ve z�plama g�c� parametreleri
	[SerializeField] Animator anim;
	public float moveSpeed = 3f;
	public float normalMoveSpeed = 3f;
	public float runMoveSpeed = 5f;
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

	[SerializeField] float rayMidDis = 5;

	[SerializeField] float weaponSpineTime = 0.0f;

	[SerializeField] int attackModeId = 0;
	[SerializeField] int selectedWeapon = 0; //0 spear, 1 bow

	//Weapons
	[SerializeField] GameObject spearSpine;
	[SerializeField] GameObject spearHand;

	[SerializeField] GameObject bowSpine;
	[SerializeField] GameObject bowHand;

	private void Start()
	{
		StartCoroutine(WeaponSpineControl());

		// Karakter kontrolc� referans�n� al
		controller = GetComponent<CharacterController>();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		MoveUpdate();
		RaycastUpdate();
		RaycastMidUpdate();
		InputUpdate();

		Debug.Log(cam.transform.rotation.x);
	}
	IEnumerator AttackMode(int attackModeId)
	{
		//TODO: S�rttan spear alma animasyonu

		SpineWeapon();
		yield return new WaitForSeconds(0.5f);
		WeaponActivity(attackModeId);
		yield return new WaitForSeconds(0.5f);
	}
	IEnumerator WeaponSpineControl()
	{
		while(true)
		{
			if(attackModeId == 1)
			{
				weaponSpineTime += Time.deltaTime;

				if (weaponSpineTime > 20.0f)
				{
					StartCoroutine(AttackMode(0));
					StartCoroutine(AttackMode(2));
					attackModeId = 0;
					weaponSpineTime = 0.0f;
				}
			}
			

			yield return null;
		}
	}
	private void AllFalseWeaponActivity()
	{
		spearSpine.SetActive(false);
		spearHand.SetActive(false);
		bowSpine.SetActive(false);
		bowHand.SetActive(false);
	}
	/// <summary>
	/// 0, spearSpine
	/// 1, spearHand
	/// </summary>
	/// <param name="weapon"></param>
	public void WeaponActivity(int weapon = -1)
	{
		AllFalseWeaponActivity();

		if (weapon != -1)
		{
			switch (weapon)
			{
				case 0:
					spearSpine.SetActive(true);
					spearHand.SetActive(false);
					break;
				case 1:
					spearHand.SetActive(true);
					spearSpine.SetActive(false);
					bowSpine.SetActive(true);
					break;
				case 2:
					bowSpine.SetActive(true);
					bowHand.SetActive(false);
					break;
				case 3:
					bowHand.SetActive(true);
					bowSpine.SetActive(false);
					spearSpine.SetActive(true);
					break;
			}
		}
	
	}
	private void InputUpdate()
	{
		if(Input.GetMouseButtonDown(0))
		{
			if(selectedWeapon == 0)
			{
				StartCoroutine(AttackMode(1));
				attackModeId = 1;
				weaponSpineTime = 0;
				Attack();
			}
			else if (selectedWeapon == 1)
			{
				StartCoroutine(AttackMode(3));
				attackModeId = 1;
				weaponSpineTime = 0;
				BowAttack();
			}
		}
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
			anim.speed = 1;
			AllFalse();
		}

		if (Input.GetKey(KeyCode.LeftShift))
		{
			moveSpeed = runMoveSpeed;
			anim.speed = 1.3f;
		}
		else
		{
			moveSpeed = normalMoveSpeed;
			anim.speed = 1;
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
	private void RaycastMidUpdate()
	{
		Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

		RaycastHit hit; // I��n�m�z�n bir �eye �arpt���n� kontrol eden de�i�ken

		// 100 birim boyunca ���n�m�z� g�nderiyoruz.
		if (Physics.Raycast(ray, out hit, rayMidDis))
		{
			// E�er ���n bir �eye �arparsa, bu �eyi konsola yazd�r.
			Debug.Log("I��n bir �eye �arpt� mid: " + hit.collider.gameObject.tag);
			Debug.DrawRay(cam.transform.position, hit.point);
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
	public void SpineWeapon()
	{
		anim.SetBool("spineWeapon", true);
	}
	public void Attack()
	{
		anim.SetTrigger("attack");
	}
	public void BowAttack()
	{
		anim.SetTrigger("bowAttack");
	}
}