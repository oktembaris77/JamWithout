using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public float health = 100;
    public Slider healthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetDamage(float damage)
    {
        health -= damage;
        healthBar.value = health;

		if(health <= 0)
		{
			Destroy(gameObject);
			Managers.instance.soundManager.PlayOneShotSound(12, Managers.instance.soundManager.effectAus1, false, true);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("spearHitPlayer"))
		{
			Managers.instance.soundManager.PlayOneShotSound(Random.Range(2, 5), Managers.instance.soundManager.effectAus1, false, true);
			GetDamage(Random.Range(5,20));
			other.enabled = false;
			StartCoroutine(SpearCollider(other));
		}

		if (other.gameObject.CompareTag("arrow"))
		{
			Managers.instance.soundManager.PlayOneShotSound(6, Managers.instance.soundManager.effectAus1, false, true);
			GetDamage(Random.Range(40,80));
		}
	}
	IEnumerator SpearCollider(Collider collider)
	{
		yield return new WaitForSeconds(1);
		collider.enabled = true;
		yield return null;
	}
}
