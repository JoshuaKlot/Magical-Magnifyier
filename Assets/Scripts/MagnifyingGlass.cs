using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MagnifyingGlass : MonoBehaviour
{
    [SerializeField] private Light MainLight;
    [SerializeField] private Image image;
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

    void Start()
    {
        if (MainLight == null)
        {
            MainLight = GetComponentInChildren<Light>();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            MainLight.tag = "Grow";
            MainLight.color = Color.green;
            image.color = Color.green;
            MainLight.enabled = true;
            this.gameObject.GetComponent<Renderer>().material.color = Color.green;

            RaycastAndInteract();
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            MainLight.tag = "Shrink";
            MainLight.color = Color.magenta;
            image.color= Color.magenta;
            MainLight.enabled = true;
            this.gameObject.GetComponent<Renderer>().material.color = Color.magenta;

            RaycastAndInteract();
        }
        else
        {
            MainLight.enabled = false;
            this.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private void RaycastAndInteract()
    {
        // Get the center of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        // Adjust the spotlight direction to follow the camera
        MainLight.transform.position = Camera.main.transform.position;
        MainLight.transform.rotation = Camera.main.transform.rotation;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, MainLight.range))
        {
            Intercatable interactable = hit.collider.GetComponent<Intercatable>();
            if (interactable != null)
            {
                if (MainLight.tag == "Grow" && interactable.Shrinkable)
                {
                    interactable.Grow();
                }
                else if (MainLight.tag == "Shrink" && interactable.Shrinkable)
                {
                    interactable.Shrink();
                }
            }
        }
    }

}
