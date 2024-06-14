using UnityEngine;

public class TrapController : MonoBehaviour
{
	//leftRightRot
	public float rotationSpeed = 100f; // D�nd�rme h�z�
	public float rotationLimit = 70f; // D�nd�rme s�n�r�
	private float currentRotation = 0f; // Mevcut d�n�� a��s�
	private int rotationDirection = 1; // D�n�� y�n� (1: sa�a, -1: sola)


	//trapSpike
	public float moveDistance = 5f; // Yukar� ��kaca�� mesafe
	public float speed = 2f; // Hareket h�z�
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
			// �stenilen d�n�� a��s�n� hesapla
			float desiredRotation = currentRotation + rotationDirection * rotationSpeed * Time.deltaTime;

			// D�n�� s�n�rlar�n� kontrol et ve y�n� de�i�tir
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

			// GameObject'i d�nd�r
			transform.rotation = Quaternion.Euler(desiredRotation, 0f, 0f);

			// Mevcut d�n�� a��s�n� g�ncelle
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
