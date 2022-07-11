using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;

public class MoveController : MonoBehaviour
{
	public bool moveForward;



	public Transform vrCamera;
	public Transform cart;
	public float toggleAngle = 30.0f;
	public float speed = 1.0f;

	public bool moveBackward = false;

	public float angleX = 0.0f;
	public float angleY = 0.0f;
	public bool didHit = true;
	public GameObject rightHandObject;
	public GameObject leftHandObject;
	public GameObject rightArmObject;
	public GameObject leftArmObject;

	private CharacterController cc;


	float radius;
	float a;
	double b;
	Vector3 newPos;
	Vector3 newRot;
	UDPScript udp;


	// Use this for initialization
	void Start()
	{
		
		cc = GetComponent<CharacterController>();
		radius = cart.position.z;
		udp = GameObject.Find("dummy").transform.GetComponent<UDPScript>();
	}




	// Update is called once per frame
	void Update()
	{
		string currentData = udp.getText();
		rightHandObject.transform.position = new Vector3(vrCamera.transform.position.x, vrCamera.transform.position.y - 0.15f, vrCamera.transform.position.z);
		leftHandObject.transform.position = new Vector3(vrCamera.transform.position.x, vrCamera.transform.position.y - 0.15f, vrCamera.transform.position.z);

		var data = currentData.Split('|');


		if (data[0].Equals("start"))
        {
			var reg = GameObject.Find("dummy").GetComponent<Regression>();
			reg.Train(currentData);
		}
		if (data[1].Equals("forward"))
		{
			print("Player moving forward");
			//handObject.SetActive(false);
			moveForward = true;
		}
		if (data[1].Equals("stop"))
		{
			print("Player stopped");
			moveForward = false;
		}
        if (data[2].Equals("pick") && data[0].Equals("no place") && data[1].Equals("stop"))
        {
            rightHandObject.SetActive(true);
			leftHandObject.SetActive(true);
			//         if (data[3].Equals("right"))
			//         {
			//	rightArmObject.SetActive(true);
			//	leftArmObject.SetActive(false);
			//}
			//else if (data[3].Equals("left"))
			//{
			//	leftArmObject.SetActive(true);
			//	rightArmObject.SetActive(false);
			//}
			var reg = GameObject.Find("dummy").GetComponent<Regression>();
			//var hand = data[3];
			float gravYRight = float.Parse(data[4].Split('=')[1]);
			float yawRight = float.Parse(data[5].Split('=')[1]);
			float angleXRight = (float)reg.getAngleX("right", gravYRight, yawRight);
			float angleYRight = (float)reg.getAngleY("right", gravYRight, yawRight);

			float gravYLeft = float.Parse(data[7].Split('=')[1]);
			float yawLeft = float.Parse(data[8].Split('=')[1]);
			float angleXLeft = (float)reg.getAngleX("left", gravYLeft, yawLeft);
			float angleYLeft = (float)reg.getAngleY("left", gravYLeft, yawLeft);
			float angleZ = 0;
			GameObject.Find("rightHand").transform.eulerAngles = new Vector3(vrCamera.transform.eulerAngles.x + 90 + angleXRight, vrCamera.transform.eulerAngles.y + angleYRight, vrCamera.transform.eulerAngles.z + angleZ);
			GameObject.Find("leftHand").transform.eulerAngles = new Vector3(vrCamera.transform.eulerAngles.x + 90 + angleXLeft, vrCamera.transform.eulerAngles.y + angleYLeft, vrCamera.transform.eulerAngles.z + angleZ);
		}
		if (data[1].Equals("stop") && data[2].Equals("pick") && data[0].Equals("no place"))
		{
			
			var reg = GameObject.Find("dummy").GetComponent<Regression>();
			//var angles = currentData.Split('|');
			//var hand = data[3];
			float gravYRight = float.Parse(data[4].Split('=')[1]);
			float yawRight = float.Parse(data[5].Split('=')[1]);

			float gravYLeft = float.Parse(data[7].Split('=')[1]);
			float yawLeft = float.Parse(data[8].Split('=')[1]);

			//float yaw = float.Parse(angles[4].Split('=')[1]);
			int layerMask = 1 << 8;
			float angleXRight = (float) reg.getAngleX("right", gravYRight, yawRight);
			float angleYRight = (float) reg.getAngleY("right", gravYRight, yawRight);

			float angleXLeft = (float)reg.getAngleX("left", gravYLeft, yawLeft);
			float angleYLeft = (float)reg.getAngleY("left", gravYLeft, yawLeft);
			float angleZ = 0;
			//angle = angle * Mathf.Deg2Rad;
			RaycastHit hit;


			// Does the ray intersect any objects in the toy layer
			//Debug.DrawRay(vrCamera.position, Quaternion.Euler(angleX, angleY, 0) * Vector3.forward * 1000, Color.white);
			
			if (Physics.Raycast(new Vector3(vrCamera.transform.position.x, vrCamera.transform.position.y - 0.15f, vrCamera.transform.position.z), Quaternion.Euler(angleXRight + vrCamera.transform.eulerAngles.x, angleYRight + vrCamera.transform.eulerAngles.y, angleZ + vrCamera.transform.eulerAngles.z) * Vector3.forward, out hit, 8, layerMask))
			{
                Debug.DrawRay(new Vector3(vrCamera.transform.position.x, vrCamera.transform.position.y - 0.15f, vrCamera.transform.position.z), Quaternion.Euler(angleXRight + vrCamera.transform.eulerAngles.x, angleYRight + vrCamera.transform.eulerAngles.y, angleZ + vrCamera.transform.eulerAngles.z) * Vector3.forward * hit.distance, Color.yellow);
				//Debug.Log("Did Hit");
				print("Ray hit the object " + hit.collider.gameObject.name + " under the parent " + hit.collider.gameObject.transform.parent.name);
				var code = GameObject.Find(hit.collider.gameObject.name).GetComponent<PickObject>();
				code.OnDown();
				currentData = "init";
				didHit = true;
			}
			else
			{
				Debug.DrawRay(new Vector3(vrCamera.transform.position.x, vrCamera.transform.position.y - 0.15f, vrCamera.transform.position.z), Quaternion.Euler(angleXRight + vrCamera.transform.eulerAngles.x, angleYRight + vrCamera.transform.eulerAngles.y, angleZ + vrCamera.transform.eulerAngles.z) * Vector3.forward * 8, Color.white);
				Debug.Log("Did not Hit");
				currentData = "init";
				didHit = false;
			}

			if (Physics.Raycast(new Vector3(vrCamera.transform.position.x, vrCamera.transform.position.y - 0.15f, vrCamera.transform.position.z), Quaternion.Euler(angleXLeft + vrCamera.transform.eulerAngles.x, angleYLeft + vrCamera.transform.eulerAngles.y, angleZ + vrCamera.transform.eulerAngles.z) * Vector3.forward, out hit, 8, layerMask))
			{
				Debug.DrawRay(new Vector3(vrCamera.transform.position.x, vrCamera.transform.position.y - 0.15f, vrCamera.transform.position.z), Quaternion.Euler(angleXLeft + vrCamera.transform.eulerAngles.x, angleYLeft + vrCamera.transform.eulerAngles.y, angleZ + vrCamera.transform.eulerAngles.z) * Vector3.forward * hit.distance, Color.yellow);
				//Debug.Log("Did Hit");
				print("Ray hit the object " + hit.collider.gameObject.name + " under the parent " + hit.collider.gameObject.transform.parent.name);
				var code = GameObject.Find(hit.collider.gameObject.name).GetComponent<PickObject>();
				code.OnDown();
				currentData = "init";
				didHit = true;
			}
			else
			{
				Debug.DrawRay(new Vector3(vrCamera.transform.position.x, vrCamera.transform.position.y - 0.15f, vrCamera.transform.position.z), Quaternion.Euler(angleXLeft + vrCamera.transform.eulerAngles.x, angleYLeft + vrCamera.transform.eulerAngles.y, angleZ + vrCamera.transform.eulerAngles.z) * Vector3.forward * 8, Color.white);
				Debug.Log("Did not Hit");
				currentData = "init";
				didHit = false;
			}
		}

		// cart code
		a = vrCamera.eulerAngles.y;
		b = (a * (Math.PI)) / 180;
		newPos = new Vector3(transform.position.x + radius * (float)Math.Sin(b), cart.position.y, transform.position.z + radius * (float)Math.Cos(b));
		newRot = new Vector3(cart.eulerAngles.x, vrCamera.eulerAngles.y - 90, cart.eulerAngles.z);
		//parent.GetComponent<CharacterController>().center = new Vector3(+radius * (float)Math.Sin(b), 0f, radius * (float)Math.Cos(b));
		

		RaycastHit hit1;
		RaycastHit hit2;
		RaycastHit hit3;
		RaycastHit hit4;
		RaycastHit hit5;
		RaycastHit hit6;
		int layerMask1 = 1 << 9;
		int layerMask2 = 1 << 10;

		// Does the ray intersect any objects in the toy layer
		//Debug.DrawRay(vrCamera.position, Quaternion.Euler(angleX, angleY, 0) * Vector3.forward * 1000, Color.white);
		if (!Physics.Raycast(cart.position, Quaternion.Euler(newRot.x, newRot.y - 90, newRot.z) * cart.TransformDirection(Vector3.forward), out hit1, 4, layerMask1) && !Physics.Raycast(cart.position, Quaternion.Euler(newRot.x, newRot.y - 60, newRot.z) * cart.TransformDirection(Vector3.forward), out hit2, 4, layerMask1) && !Physics.Raycast(cart.position, Quaternion.Euler(newRot.x, newRot.y - 120, newRot.z) * cart.TransformDirection(Vector3.forward), out hit3, 4, layerMask1))
		{
			Debug.DrawRay(cart.position, Quaternion.Euler(newRot.x - 10, newRot.y - 90, newRot.z) * cart.TransformDirection(Vector3.forward) * 4, Color.white);
			Debug.DrawRay(cart.position, Quaternion.Euler(newRot.x - 10, newRot.y - 60, newRot.z) * cart.TransformDirection(Vector3.forward) * 5, Color.white);
			Debug.DrawRay(cart.position, Quaternion.Euler(newRot.x - 10, newRot.y - 120, newRot.z) * cart.TransformDirection(Vector3.forward) * 5, Color.white);
			if (Physics.Raycast(cart.position, Quaternion.Euler(newRot.x - 10, newRot.y - 90, newRot.z) * cart.TransformDirection(Vector3.forward), out hit4, 4, layerMask2) || Physics.Raycast(cart.position, Quaternion.Euler(newRot.x - 10, newRot.y - 60, newRot.z) * cart.TransformDirection(Vector3.forward), out hit4, 5, layerMask2) || Physics.Raycast(cart.position, Quaternion.Euler(newRot.x - 10, newRot.y - 120, newRot.z) * cart.TransformDirection(Vector3.forward), out hit4, 5, layerMask2))
			{
				
				//print(hit4.point);
				moveForward = false;
			}

			else
			{
				float deltaX = hit4.point.x - cart.position.x;
				float deltaY = hit4.point.y - cart.position.y;
				float deltaZ = hit4.point.z - cart.position.z;

				float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
				//print(distance);
				cart.position = newPos;
				cart.eulerAngles = newRot;
				//latitude = Input.location.lastData.latitude;
				//longitude = Input.location.lastData.longitude;
				//Vector3 newPos1 = Quaternion.AngleAxis(longitude, -Vector3.up) * Quaternion.AngleAxis(latitude, -Vector3.right) * new Vector3(0, 0, 1);
				//transform.position = new Vector3(newPos1.x, 3.2f, newPos1.y);

			}

			if (moveForward)
			{
				print("moving forward");

				Vector3 forward = vrCamera.TransformDirection(Vector3.forward);

				cc.SimpleMove(forward * speed);




				//Vector3 backward = vrCamera.TransformDirection(Vector3.back);

				//cc.SimpleMove(backward * speed);
			}
		}
       


		


		if (Input.GetKeyDown(KeyCode.Escape))
		{
            //Application.Quit();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

	}



}