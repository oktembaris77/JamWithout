using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class Character : MonoBehaviour
{
	// Hýz ve zýplama gücü parametreleri
	[SerializeField] Animator anim;
	public float moveSpeed = 3f;
	public float normalMoveSpeed = 3f;
	public float runMoveSpeed = 5f;
	public float jumpForce = 5f;
	public float mouseXSpeed = 5f;
	public float mouseYSpeed = 5f;

	public float health = 100.0f;

	[SerializeField] bool die = false;

	[SerializeField] Transform arrowSp;


	// Yer çekimi ve karakter kontrolcü referanslarý
	[SerializeField] bool isGrounded;
	private float gravity = -9.81f;
	[SerializeField] private Vector3 velocity;
	private CharacterController controller;

	[SerializeField] GameObject cam;

	public float maxLookAngle = 90f; // Maksimum bakýþ açýsý

	[SerializeField] Transform rayTarget;
	[SerializeField] List<GameObject> rayHits;

	[SerializeField] float rayMidDis = 5;

	[SerializeField] float weaponSpineTime = 0.0f;

	[SerializeField] int attackModeId = 0;
	[SerializeField] int selectedWeapon = -1; //0 spear, 1 bow

	//Weapons
	[SerializeField] GameObject spearSpine;
	[SerializeField] GameObject spearHand;

	[SerializeField] GameObject bowSpine;
	[SerializeField] GameObject bowHand;

	[SerializeField] AudioSource audioSource;

	[SerializeField] GameObject spearCollider;

	RaycastHit infHit; // Iþýnýmýzýn bir þeye çarptýðýný kontrol eden deðiþken


	

	private void Start()
	{
		StartCoroutine(WeaponSpineControl());

		// Karakter kontrolcü referansýný al
		controller = GetComponent<CharacterController>();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		if(PlayerPrefs.GetFloat("mouseSen") == 0) PlayerPrefs.SetFloat("mouseSen", 130);

		mouseXSpeed = PlayerPrefs.GetFloat("mouseSen");
		mouseYSpeed = PlayerPrefs.GetFloat("mouseSen");
	}

	private void Update()
	{
		if (die) return;

		if (Input.GetKeyDown(KeyCode.Alpha1))//spear
		{
			selectedWeapon = 0;
			Attack();

		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) //bow
		{
			selectedWeapon = 1;
			Attack();
		}

		MoveUpdate();
		RaycastUpdate();
		RaycastMidUpdate();
		RaycastInfinityUpdate();
		InputUpdate();

		Debug.Log(cam.transform.rotation.x);
	}
	IEnumerator AttackMode(int attackModeId)
	{
		//TODO: Sýrttan spear alma animasyonu

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
					bowSpine.SetActive(true);
					break;
				case 1:
					spearHand.SetActive(true);
					spearSpine.SetActive(false);
					bowSpine.SetActive(true);
					break;
				case 2:
					bowSpine.SetActive(true);
					bowHand.SetActive(false);
					spearSpine.SetActive(true);
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

				if(infHit.collider != null && !infHit.collider.CompareTag("Player"))
				{
					StartCoroutine(BowAttackIE());
				}
			}
		}
	}
	private void GetDamage(float damage)
	{
		float newHealth = health - Random.Range(damage * 0.8f, damage * 1.2f);
		SetHealth(newHealth);
	}
	private void SetHealth(float newHealth)
	{
		health = newHealth;
		Managers.instance.uiManager.playerHealthBar.value = health;

		if (health <= 0.0f)
		{
			die = true;
			AllFalse();
			Die();
			Managers.instance.soundManager.PlayOneShotSound(11, Managers.instance.soundManager.generalAudioSource, false, true);
			StartCoroutine(GoToMainMenu());
		}
	}
	IEnumerator GoToMainMenu()
	{
		yield return new WaitForSeconds(3);

		SceneManager.LoadScene(0);
	}
	private void MoveUpdate()
	{
		// Yatay ve dikey hareket vektörleri
		float horizontalMove = Input.GetAxis("Horizontal");
		float verticalMove = Input.GetAxis("Vertical");

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
		{
			Walk();

			if (Input.GetKey(KeyCode.LeftShift))
			{
				Managers.instance.soundManager.RandomWalking(0, true);
			}
			else
			{
				Managers.instance.soundManager.RandomWalking();
			}
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

		float rotX = Mathf.Clamp(cam.transform.rotation.x, -0.2f, 0.2f);

		//Mouse rotate
		transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * mouseXSpeed, 0);
		//cam.transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * mouseYSpeed, 0, 0);



		// Fare eksenlerini al
		float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseYSpeed;

		// Nesnenin dönme açýsýný hesapla
		float newRotationX = cam.transform.localEulerAngles.x - mouseY;

		// Aþýrý dönme durumunu düzelt
		if (newRotationX > 180)
			newRotationX -= 360;

		// Maksimum ve minimum bakýþ açýsýný sýnýrla
		newRotationX = Mathf.Clamp(newRotationX, -maxLookAngle, maxLookAngle);

		// Yeni dönme açýsýný uygula
		cam.transform.localEulerAngles = new Vector3(newRotationX, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
	}
	private void RaycastUpdate()
	{
		// A objesinin pozisyonu
		Vector3 origin = cam.transform.position;

		// B objesinin pozisyonu
		Vector3 direction = (rayTarget.position - origin).normalized;

		// RaycastHit bilgilerini tutmak için
		RaycastHit hit;

		// Raycast'i gerçekleþtir
		if (Physics.Raycast(origin, direction, out hit))
		{
			// Çizgiyi sahnede görmek için
			Debug.DrawRay(origin, direction * hit.distance, Color.red);

			if (hit.collider.CompareTag("wall"))
			{
				AddRayHits(hit.collider.gameObject);
			}
			else RemoveRayHits();
		}
		else
		{
			// Hit olmazsa bile çizgiyi bir mesafeye kadar çiz
			Debug.DrawRay(origin, direction * 100, Color.red);
		}
	}
	private void RaycastMidUpdate()
	{
	

		Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

		RaycastHit hit; // Iþýnýmýzýn bir þeye çarptýðýný kontrol eden deðiþken

		// 100 birim boyunca ýþýnýmýzý gönderiyoruz.
		if (Physics.Raycast(ray, out hit, rayMidDis))
		{
			// Eðer ýþýn bir þeye çarparsa, bu þeyi konsola yazdýr.
			Debug.Log("Iþýn bir þeye çarptý mid: " + hit.collider.gameObject.tag);
			Debug.DrawRay(cam.transform.position, hit.point);
		}
	}
	private void RaycastInfinityUpdate()
	{
		// Öncelikle yok saymak istediðiniz katmanýn adýný belirleyin ve LayerMask olarak alýn.
		int layerToIgnore = LayerMask.NameToLayer("InfRayMak");

		// Katmaný yok sayarak tüm katmanlarý seçin.
		int layerMask = 1 << layerToIgnore;

		// Katmaný devre dýþý býrakýn.
		layerMask = ~layerMask;

		Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));


		// 100 birim boyunca ýþýnýmýzý gönderiyoruz.
		if (Physics.Raycast(ray, out infHit, Mathf.Infinity, layerMask))
		{
			Debug.DrawRay(cam.transform.position, infHit.point);
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
		//By hariç hepsini listeden sil
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
	public void Hit()
	{
		anim.SetTrigger("hit");
	}
	public void Die()
	{
		anim.SetTrigger("die");
	}
	private void OnTriggerEnter(Collider other)
	{
		if (die) return;

		if (other.gameObject.CompareTag("spearHit"))
		{
			if(other.gameObject != spearCollider)

			Hit();
			GetDamage(2);
			Managers.instance.soundManager.PlayOneShotSound(Random.Range(2, 5), audioSource, false, true);
			other.enabled = false;
			StartCoroutine(SpearCollider(other));
		}

		if(other.gameObject.CompareTag("trap"))
		{
			GetDamage(10);
		}

		if (other.gameObject.CompareTag("end"))
		{
			SceneManager.LoadScene(0);
		}
	}
	IEnumerator SpearCollider(Collider collider)
	{
		yield return new WaitForSeconds(1);
		collider.enabled = true;
		yield return null;
	}
	IEnumerator BowAttackIE()
	{
		

		yield return new WaitForSeconds(0.7f);
		GameObject arrow = Instantiate(Managers.instance.prefabManager.prefabs[0], arrowSp.transform.position, Quaternion.identity);
		arrow.GetComponent<ArrowForce>().target = infHit.point;
	}
}