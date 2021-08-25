using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
public class firest : MonoBehaviour
{
    [SerializeField] FirePoint fire;
    GameObject[] fp = new GameObject[5];
    int rcount=0;
    // Start is called before the first frame update
    void Start()
    {
        rcount = Random.Range(2, fp.Length);
        InvokeRepeating("fireRepeating", 2f, 2f);
    }
    private void fireRepeating()
    {
        int Randomize = Random.Range(0, 2);
        if (Randomize == 0)
        {
            for (int i = 0; i < rcount; i++)
            {
                fp[i] = Instantiate(fire.gameObject, transform.position, Quaternion.identity);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
