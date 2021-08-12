using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Around : MonoBehaviour
{
    public GameObject anchor;
    public float velocidad;
    void Start()
    {
        velocidad = 50f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        transform.RotateAround(anchor.transform.localPosition, Vector3.back, Time.deltaTime * velocidad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
