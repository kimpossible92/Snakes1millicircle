using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public static ThisButtons This;
    void Start()
    {
        This = this;
    }
    public int lr;
    public void left()
    {
        lr = -1;
    }
    public void right()
    {
        lr = 1;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
