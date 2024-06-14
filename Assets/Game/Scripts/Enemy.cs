using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent nav;

	[SerializeField] bool attackActivity = false;

	[SerializeField] float followDistance = 10.0f;
	[SerializeField] float attackDistance = 2.0f;

	[SerializeField] int attackModeId = 0;

	public float health = 100.0f;
	[SerializeField] Slider healthBar;
	[SerializeField] GameObject canvas;

	//Weapons
	[SerializeField] GameObject spearSpine;
	[SerializeField] GameObject spearHand;

	[SerializeField] AudioSource audioSource;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

	// Update is called once per frame
	void Update()
	{
		AnimUpdate();

		if (attackActivity)
			RaycastUpdate();


		if (PlayerDistance() > 15)
		{
			if (attackModeId == 1)
				StartCoroutine(AttackMode(0));
		}

		canvas.transform.LookAt(Managers.instance.gameplayManager.playerCam.transform.position);
	}
	public void SetAttackActivity(bool activity)
	{
		attackActivity = activity;
	}
	/// <summary>
	/// 0, spearSpine
	/// 1, spearHand
	/// </summary>
	/// <param name="weapon"></param>
	public void WeaponActivity(int weapon = -1)
	{
		if(weapon != -1)
		{
			switch (weapon)
			{
				case 0:
					spearSpine.SetActive(true);
					spearHand.SetActive(false);
					attackModeId = 0;
					break;
				case 1:
					spearHand.SetActive(true);
					spearSpine.SetActive(false);
					attackModeId = 1;
					break;
			}
		}
		else
		{
			if(spearSpine.activeSelf)
			{
				spearHand.SetActive(true);
				spearSpine.SetActive(false);
			}
			else
			{
				spearSpine.SetActive(true);
				spearHand.SetActive(false);
			}
		}
	}
    private void AnimUpdate()
    {
        if(nav.hasPath)
        {
            Walk();
		}
        else
        {
            AllFalse();
			
		}
    }
    public float PlayerDistance()
    {
        return Vector3.Distance(transform.position, Managers.instance.gameplayManager.player.transform.position);
    }
    public bool IsFollowDistance()
    {
        if(PlayerDistance() < followDistance) return true;

		return false;
    }
	public bool IsAttackDistance()
	{
		if (PlayerDistance() < attackDistance) return true;

		return false;
	}
	private void RaycastUpdate()
	{
		// A objesinin pozisyonu
		Vector3 origin = transform.position;

		// B objesinin pozisyonu
		Vector3 direction = (Managers.instance.gameplayManager.player.transform.position - origin).normalized;

		// RaycastHit bilgilerini tutmak için
		RaycastHit hit;

		// Raycast'i gerçekleþtir
		if (Physics.Raycast(origin, direction, out hit))
		{
			Debug.Log(hit.collider.tag);
			// Çizgiyi sahnede görmek için
			Debug.DrawRay(origin, direction * hit.distance, Color.red);

			if (hit.collider.CompareTag("Player") && IsAttackDistance())
			{
				nav.SetDestination(transform.position);
				transform.LookAt(Managers.instance.gameplayManager.player.transform.position, new Vector3(0,1,0));
				Attack();
			}
			else if (hit.collider.CompareTag("Player") && IsFollowDistance())
			{
				nav.SetDestination(Managers.instance.gameplayManager.player.transform.position);
				
				if (attackModeId == 0)
					StartCoroutine(AttackMode(1));
			}
		}
		else
		{
			// Hit olmazsa bile çizgiyi bir mesafeye kadar çiz
			Debug.DrawRay(origin, direction * 100, Color.red);
		}
	}
	private void GetDamage(float damage)
	{
		float newHealth = health - Random.Range(damage * 0.8f, damage * 1.1f);
		SetHealth(newHealth);
	}
	private void SetHealth(float newHealth)
	{
		health = newHealth;
		healthBar.value = health;
	}
	IEnumerator AttackMode(int attackModeId)
	{
		//TODO: Sýrttan spear alma animasyonu

		SpineWeapon();
		yield return new WaitForSeconds(0.5f);
		WeaponActivity(attackModeId);
		yield return new WaitForSeconds(0.5f);
	}
	public void AllFalse()
	{
		anim.SetBool("walk", false);
		anim.SetBool("spineWeapon", false);
	}
	public void Walk()
	{
		anim.SetBool("walk", true);
	}
	public void SpineWeapon()
	{
		anim.SetTrigger("spineWeapon");
	}
	public void Attack()
	{
		anim.SetTrigger("attack");
	}
	public void Hit()
	{
		anim.SetTrigger("hit");
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("spearHit"))
		{
			Hit();
			GetDamage(20);
			Managers.instance.soundManager.PlayOneShotSound(Random.Range(2, 5), audioSource, false, true);
			other.enabled = false;
			StartCoroutine(SpearCollider(other));
		}
	}
	IEnumerator SpearCollider(Collider collider)
	{
		yield return new WaitForSeconds(1);
		collider.enabled = true;
		yield return null;
	}
}
