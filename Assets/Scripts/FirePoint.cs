using System.Collections;
using UnityEngine;
namespace Assets.Scripts
{
    public class FirePoint : MonoBehaviour
    {
        public bool my = false;
        float rspeed;
        // Use this for initialization
        void Start()
        {
            rspeed = Random.Range(0.6f, 1.2f);
            Invoke("detsroyer", 2f);
        }
        private void detsroyer()
        {
            Destroy(gameObject);
        }
        // Update is called once per frame
        void Update()
        {
            if (my == true) transform.Translate(Vector3.up);
            else transform.Translate(Vector3.down*rspeed);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (
                //collision.gameObject.tag == "yad" ||
                //(collision.gameObject.tag == "corm" && my == true) ||
                (collision.gameObject.tag == "enem" && my == true)
                )
            {
                FindObjectOfType<Road>().removeEnem(collision.gameObject.GetComponent<offcorm>());
                Destroy(collision.gameObject);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
        }
    }
}