using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupView : MonoBehaviour
{
    List<plitka> plitki = new List<plitka>();
    public plitka[,] gems3 = new plitka[2,2];
    public void setPlitka(List<plitka> plit)
    {
        plitki = plit;
    }
    public void cler()
    {
        plitki.Clear();
    }
    public List<plitka> GetPlitkas()
    {
        return plitki;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
