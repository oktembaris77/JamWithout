using UnityEngine;
using UnityEngine.Audio;

public class ArrowForce : MonoBehaviour
{
    public Vector3 target;
    public Vector3 epos;
	[SerializeField] AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
			Destroy(gameObject,2);

	}

	// Update is called once per frame
	void Update()
    {
		if(Vector3.Distance(target, transform.position) > 1)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, 40.0f * Time.deltaTime);
			transform.LookAt(target);
		}
		else
		{
		}
	}
	private void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("enemy"))
        {
			other.gameObject.GetComponent<Enemy>().attackActivity = true;
			other.gameObject.GetComponent<Enemy>().GetDamage(Random.Range(20,80));
			Destroy(gameObject);
		}
	
	}
}
