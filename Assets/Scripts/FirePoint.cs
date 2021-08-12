using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class FirePoint : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.right*15);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "yad")
            {
                Destroy(collision.gameObject);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "yad")
            {
                Destroy(gameObject);
            }
        }
    }
}