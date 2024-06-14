using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent nav;

	public bool attackActivity = false;

	[SerializeField] float followDistance = 30.0f;
	[SerializeField] float attackDistance = 2.0f;

	[SerializeField] int attackModeId = 0;

	[SerializeField] bool die = false;

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
		if (die) return;

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
	public void GetDamage(float damage)
	{
		float newHealth = health - Random.Range(damage * 0.8f, damage * 1.1f);
		SetHealth(newHealth);
	}
	private void SetHealth(float newHealth)
	{
		health = newHealth;
		healthBar.value = health;

		if(health <= 0.0f)
		{
			die = true;
			nav.enabled = false;
			AllFalse();
			Die();
			Managers.instance.soundManager.PlayOneShotSound(11, audioSource, false, true);
			spearHand.GetComponent<BoxCollider>().enabled = false;
			Destroy(gameObject, 10);
		}
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
	public void Die()
	{
		anim.SetTrigger("die");
	}
	private void OnTriggerEnter(Collider other)
	{
		if (die) return;

		if (other.gameObject.CompareTag("spearHitPlayer"))
		{
			attackActivity = true;
			Hit();
			GetDamage(80);
			Managers.instance.soundManager.PlayOneShotSound(Random.Range(2, 5), audioSource, false, true);
			Managers.instance.soundManager.PlayOneShotSound(Random.Range(7, 11), Managers.instance.soundManager.effectAus2, false, true);
			other.enabled = false;
			StartCoroutine(SpearCollider(other));
		}

		if(other.gameObject.CompareTag("arrow"))
		{
			Managers.instance.soundManager.PlayOneShotSound(6, Managers.instance.soundManager.effectAus1, false, true);
			Managers.instance.soundManager.PlayOneShotSound(Random.Range(7, 11), Managers.instance.soundManager.effectAus2, false, true);
		}

		if (other.gameObject.CompareTag("trap"))
		{
			//GetDamage(10);
		}
	}
	IEnumerator SpearCollider(Collider collider)
	{
		yield return new WaitForSeconds(1);
		collider.enabled = true;
		yield return null;
	}
}
