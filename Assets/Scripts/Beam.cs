using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.CompareTag("Grow")&&Input.GetKeyUp(KeyCode.Mouse0))
        {
            Destroy(this.gameObject);
        }
        if (this.gameObject.CompareTag("Shrink") && Input.GetKeyUp(KeyCode.Mouse1))
        {
            Destroy(this.gameObject);
        }
    }
}
