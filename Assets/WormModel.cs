using System.Collections;
using UnityEngine;

namespace Assets
{
    public class WormModel : MonoBehaviour
    {
        int count = 0;
        int up = 0, down = 0;
        private void Start()
        {
            count = 0; up = 0; down = 0;
        }
        public string setUpDown()
        {
            return "up:"+up +"down:"+down;
        }
        public string setDataBase()
        {
            var Xmove = Input.mousePosition.y;
            if (Input.GetMouseButtonDown(0))
            {
                if (Xmove > 180) { GetComponent<WormDataBase>().setHead(3); up++; }
                else if (Xmove < 180) { GetComponent<WormDataBase>().setHead(4); down++; }
            }
            if (Input.GetMouseButtonUp(0)) { GetComponent<WormDataBase>().setHead(1); count++; }
            return count + "";
        }
    }
}