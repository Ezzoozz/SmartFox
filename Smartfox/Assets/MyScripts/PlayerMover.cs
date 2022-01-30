using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
	
	public float forwardSpeed = 10;
	public float backwardSpeed = 8;
	public float rotationSpeed = 40;

    [SerializeField]  float distanceToGround = 1f;

	[SerializeField] float jumpSpeed = 1f;

	Collider collider;

	bool isGrounded = true;

	// Dirty flag for checking if movement was made or not
	public bool MovementDirty { get; set; }

	Rigidbody rb;
  

    void Start()
	{
		DontDestroyOnLoad(gameObject);
		rb = GetComponent<Rigidbody>();
		MovementDirty = false;

		collider = GetComponent<Collider>();

		distanceToGround = collider.bounds.extents.y;
	}

	
	void Update()
	{
		// Forward/backward makes player model move
		float translation = Input.GetAxis("Vertical");
		if (translation != 0)
		{
			this.transform.Translate(0, 0, translation * Time.deltaTime * forwardSpeed);
			MovementDirty = true;
		}

		// Left/right makes player model rotate around own axis
		float rotation = Input.GetAxis("Horizontal");
		if (rotation != 0)
		{
			this.transform.Rotate(Vector3.up, rotation * Time.deltaTime * rotationSpeed);
			MovementDirty = true;
		}

		if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

			StartCoroutine(Jump());
        }

	}

	IEnumerator Jump()
    {
		isGrounded = false;
		for(int i = 0; i < 10; i++)
        {
			transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

			yield return new WaitForSeconds(0.02f);

        }
		isGrounded = true;
    }
}
