using UnityEngine;

public class TrapController : MonoBehaviour
{
	//leftRightRot
	public float rotationSpeed = 100f; // Döndürme hýzý
	public float rotationLimit = 70f; // Döndürme sýnýrý
	private float currentRotation = 0f; // Mevcut dönüþ açýsý
	private int rotationDirection = 1; // Dönüþ yönü (1: saða, -1: sola)


	//trapSpike
	public float moveDistance = 5f; // Yukarý çýkacaðý mesafe
	public float speed = 2f; // Hareket hýzý
	private Vector3 initialPosition;
	private bool movingUp = true;

	//booleans
	public bool leftRightRot = true;
	public bool trapSpike = false;
	void Start()
    {
		if (trapSpike)
			initialPosition = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
		if(leftRightRot)
		{
			// Ýstenilen dönüþ açýsýný hesapla
			float desiredRotation = currentRotation + rotationDirection * rotationSpeed * Time.deltaTime;

			// Dönüþ sýnýrlarýný kontrol et ve yönü deðiþtir
			if (desiredRotation >= rotationLimit)
			{
				desiredRotation = rotationLimit;
				rotationDirection = -1;
			}
			else if (desiredRotation <= -rotationLimit)
			{
				desiredRotation = -rotationLimit;
				rotationDirection = 1;
			}

			// GameObject'i döndür
			transform.rotation = Quaternion.Euler(desiredRotation, 0f, 0f);

			// Mevcut dönüþ açýsýný güncelle
			currentRotation = desiredRotation;
		}
		else if(trapSpike)
		{
			float step = speed * Time.deltaTime;

			if (movingUp)
			{
				transform.position = Vector3.MoveTowards(transform.position, initialPosition + Vector3.up * moveDistance, step);
				if (Vector3.Distance(transform.position, initialPosition + Vector3.up * moveDistance) < 0.001f)
				{
					movingUp = false;
				}
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position, initialPosition, step);
				if (Vector3.Distance(transform.position, initialPosition) < 0.001f)
				{
					movingUp = true;
				}
			}
		}
	}

}
