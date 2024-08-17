using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MagnifyingGlass : MonoBehaviour
{
    [SerializeField]private Light MainLight;
    public GameObject beamGrowPrefab;
    public GameObject beamShrinkPrefab;
    public Transform bulletSpawn;
    public float beamSpeed = 30f;
    public GameObject player;
    private ShootingMode currentMode;
    public enum ShootingMode
    {
        grow,
        shrink
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            MainLight.tag = "Grow";
            MainLight.color = Color.green; // Use predefined colors
            MainLight.enabled = true; // Enable the light

            // Change the material color to green
            this.gameObject.GetComponent<Renderer>().material.color = Color.green;

            RaycastHit hit;
            if (Physics.Raycast(MainLight.transform.position, MainLight.transform.forward, out hit, MainLight.range))
            {
                Intercatable interactable = hit.collider.GetComponent<Intercatable>();
                if (interactable != null && interactable.Shrinkable == true)
                {
                    interactable.Grow();
                }
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            MainLight.tag = "Shrink";
            MainLight.color = Color.magenta; // Use predefined colors
            MainLight.enabled = true; // Enable the light

            // Change the material color to magenta
            this.gameObject.GetComponent<Renderer>().material.color = Color.magenta;

            RaycastHit hit;
            if (Physics.Raycast(MainLight.transform.position, MainLight.transform.forward, out hit, MainLight.range))
            {
                Intercatable interactable = hit.collider.GetComponent<Intercatable>();
                if (interactable != null && interactable.Shrinkable == true)
                {
                    interactable.Shrink();
                }
            }
        }
        else
        {
            MainLight.enabled = false; // Disable the light
            
            // Change the material color to white
            this.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }


    }



    void FireWeapon()
    {

        GameObject beam = beamGrowPrefab;
        if (currentMode == ShootingMode.grow)
            beam = beamGrowPrefab;
        if (currentMode == ShootingMode.shrink)
            beam = beamShrinkPrefab;

        // Instantiate the bullet with the same rotation as the bulletSpawn
        GameObject bullet = Instantiate(beam, bulletSpawn.position, Quaternion.identity);

        // Rotate the bullet 90 degrees on the x-axis
        bullet.transform.Rotate(90f, 0f, 0f);

        // Add force in the forward direction of the bullet (which now considers the rotation of bulletSpawn)
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * beamSpeed, ForceMode.Impulse);
    }

}
