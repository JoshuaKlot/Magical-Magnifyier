using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagnifyingGlass : MonoBehaviour
{
    [SerializeField] private Light MainLight;
    [SerializeField] private Image image;
    [SerializeField] private AudioSource Grow;
    [SerializeField] private AudioSource Shrink;
    [SerializeField] private AudioSource GrowHit;
    [SerializeField] private AudioSource ShrinkHit;

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
            MuteAllSoundsExcept(Grow);
            MainLight.tag = "Grow";
            MainLight.color = Color.green;
            image.color = Color.green;
            MainLight.enabled = true;
            this.gameObject.GetComponent<Renderer>().material.color = Color.green;

            RaycastAndInteract();
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            MuteAllSoundsExcept(Shrink);
            MainLight.tag = "Shrink";
            MainLight.color = Color.magenta;
            image.color = Color.magenta;
            MainLight.enabled = true;
            this.gameObject.GetComponent<Renderer>().material.color = Color.magenta;

            RaycastAndInteract();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse2)) // Check if middle mouse button is pressed
        {
            MuteAllSoundsExcept(null);
            PerformShortRaycast();
        }
        else
        {
            MuteAllSoundsExcept(null);
            MainLight.enabled = false;
            this.gameObject.GetComponent<Renderer>().material.color = Color.white;
            image.color = Color.white;
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
                    MuteAllSoundsExcept(GrowHit);
                    interactable.Grow();
                    if (!GrowHit.isPlaying)
                    {
                        GrowHit.Play();
                    }
                }
                else if (MainLight.tag == "Shrink" && interactable.Shrinkable)
                {
                    MuteAllSoundsExcept(ShrinkHit);
                    interactable.Shrink();
                    if (!ShrinkHit.isPlaying)
                    {
                        ShrinkHit.Play();
                    }
                }
            }
            else
            {
                if (MainLight.tag == "Grow")
                {
                    MuteAllSoundsExcept(Grow);
                    if (!Grow.isPlaying)
                    {
                        Grow.Play();
                    }
                }
                else if (MainLight.tag == "Shrink")
                {
                    MuteAllSoundsExcept(Shrink);
                    if (!Shrink.isPlaying)
                    {
                        Shrink.Play();
                    }
                }
            }
        }
    }


    private void PerformShortRaycast()
    {
        // Raycast from the camera to the center of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, MainLight.range))
        {
            Intercatable interactable = hit.collider.GetComponent<Intercatable>();
            if (interactable != null)
            {
                // Simulate a collision with the player
                interactable.SimulateCollision(player);
            }
        }
    }

    private void MuteAllSoundsExcept(AudioSource activeSound)
    {
        Grow.mute = activeSound != Grow;
        Shrink.mute = activeSound != Shrink;
        GrowHit.mute = activeSound != GrowHit;
        ShrinkHit.mute = activeSound != ShrinkHit;
    }
}

