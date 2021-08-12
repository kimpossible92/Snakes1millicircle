using System.Collections;
using UnityEngine;

namespace Assets
{
    public class WormControl : MonoBehaviour
    {

        public string setModel()
        {
            return GetComponent<WormModel>().setDataBase();
        }
        public string getupdown()
        {
            return GetComponent<WormModel>().setUpDown();
        }
    }
}