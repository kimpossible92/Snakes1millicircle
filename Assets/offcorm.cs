using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offcorm : MonoBehaviour
{
    [SerializeField] int color;
    public Vector3 strt;
    public int Color { get => color; set => color = value; }
    int pos1, pos2;
    // Start is called before the first frame update
    void Start()
    {
        //strt = transform.position;
        pos1 = Random.Range(-1, 2);
        pos2 = Random.Range(-1, 2);
        if (pos2 == 0)
        {
            pos2 = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<Road>().getpause()) return;
        if (tag == "corm")
        {
            transform.Translate(new Vector3(0.08f*pos2, -0.08f, 0));
        }
        if (tag == "enem")
        {
            transform.Translate(new Vector3(0.08f*pos2, 0, 0));
        }
        if (transform.position.x > 17)
        {
            transform.position = new Vector3(-17, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -17)
        {
            transform.position = new Vector3(17, transform.position.y, transform.position.z);
        }
        if (transform.position.y > 10f)
        {
            transform.position = new Vector3(transform.position.x, -2.5f, transform.position.z);
        }
        if (transform.position.y < -3)
        {
            transform.position = new Vector3(transform.position.x, 9, transform.position.z);
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if (gameObject.tag != "enem")
        {
            if (coll.gameObject.tag == "yad" ||
                       coll.gameObject.tag == "corm" ||
                       //coll.gameObject.tag == "enem" ||
                       coll.gameObject.tag == "fire"
                       )
            {
                if (gameObject.tag == "corm" &&coll.gameObject.tag == "fire")
                {
                    if (coll.gameObject.GetComponent<Assets.Scripts.FirePoint>().my)
                    {
                        if (transform.localScale.x == 1) { FindObjectOfType<Road>().addScore(100); }
                        if (transform.localScale.x == 2) { FindObjectOfType<Road>().addScore(50); }
                        if (transform.localScale.x == 3) { FindObjectOfType<Road>().addScore(20); }
                    }
                }
                if (transform.localScale.x == 2)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    if (pos2 == -1)
                    {
                        GameObject second = Instantiate(gameObject, transform.position + new Vector3(-2, -2, 0), Quaternion.identity);
                        transform.position = new Vector3(transform.position.x + 2, transform.position.y + 2, transform.position.z);
                        pos2 = 1;
                    }
                    if (pos2 == 0 || pos2 == 1)
                    {
                        GameObject second = Instantiate(gameObject, transform.position + new Vector3(2, 2, 0), Quaternion.identity);
                        transform.position = new Vector3(transform.position.x - 2, transform.position.y - 2, transform.position.z);
                        pos2 = -1;
                    }


                }
                else if (transform.localScale.x == 3)
                {
                    transform.localScale = new Vector3(2, 2, 2);
                    if (pos2 == -1)
                    {
                        GameObject second = Instantiate(gameObject, transform.position + new Vector3(-2, -2, 0), Quaternion.identity);
                        transform.position = new Vector3(transform.position.x + 2, transform.position.y + 2, transform.position.z);
                        pos2 = 1;
                    }
                    if (pos2 == 0 || pos2 == 1)
                    {
                        GameObject second = Instantiate(gameObject, transform.position + new Vector3(2, 2, 0), Quaternion.identity);
                        transform.position = new Vector3(transform.position.x - 2, transform.position.y - 2, transform.position.z);
                        pos2 = -1;
                    }
                }
                else if (transform.localScale.x == 1 && coll.gameObject.tag == "fire")
                {
                    FindObjectOfType<Road>().removeCorm(this);
                    Destroy(gameObject);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    if (pos2 == 0 || pos2 == -1)
                    {
                        transform.position = new Vector3(transform.position.x + 2, transform.position.y + 3, transform.position.z);
                        pos2 = 1;
                    }
                    if (pos2 == 1)
                    {
                        transform.position = new Vector3(transform.position.x - 2, transform.position.y - 3, transform.position.z);
                        pos2 = -1;
                    }

                }
            }
        }
        else
        {
            if (coll.gameObject.tag == "fire")
            {
                if (coll.gameObject.GetComponent<Assets.Scripts.FirePoint>().my)
                { FindObjectOfType<Road>().addScore(200); }
            }
        }
        if (gameObject.tag != "razor")
        {
            //if (coll.gameObject.tag == "w")
            //{
            //    Destroy(gameObject);
            //}
            //if (coll.gameObject.tag == "server")
            //{
            //    Destroy(gameObject);
            //}
        }
        if (gameObject.tag == "razor")
        {
           
        }
    }
}
