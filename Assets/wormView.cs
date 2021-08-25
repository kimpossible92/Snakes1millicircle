using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wormView : MonoBehaviour
{
    Text text;
    Text text2;
    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.Find("Canvas").transform.Find("Text2").GetComponent<Text>();
        text2 = GameObject.Find("Canvas").transform.Find("Text3").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "click: "+GetComponent<WormControl>().setModel();
    }
}
