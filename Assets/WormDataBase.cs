using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts;
public class WormDataBase : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text scoreText;
    [SerializeField]
    UnityEngine.UI.Text rubyText;
    [SerializeField] GameObject goMat;
    float rubys = 0;
    protected int sec = 0;
    bool disconnectplayer = false;
    private Vector3 mDirection;
    protected int addmove = 0;
    [SerializeField]GameObject unitPrefab;
    protected List<GameObject> wList = new List<GameObject>();
    protected List<Transform> waypoints = new List<Transform>();
    [SerializeField]Vector3 ves;
    protected int newHead = 0;
    float dirspeed = 0.25f;
    [SerializeField] LayerMask rybiLayer;
    [SerializeField] LayerMask MoveOnSightLayer, MoveOnSightObstaclesLayer, DeadLayer, ButtonLayer;
    [SerializeField] List<string> roads = new List<string>();
    [SerializeField] List<float> vspos = new List<float>();
    [SerializeField] Road currentRoad;
    [SerializeField] Material GetMaterial;
    [SerializeField] GameObject[] mmats2;
    [SerializeField] Sprite[] mmats;
    [SerializeField] FirePoint fire;
    int score=0;
    Vector3 loadposition;
    Vector3 newloadpos;
    Vector3 localposit;
    public void nazad()
    {
        load3sec = true;
        StartCoroutine(GetLoseOrFinish("You Dead"));
        transform.position = loadposition;
    }
    public void setHead(int head)
    {
        newHead = head;
    }
    // Start is called before the first frame update
    void Start()
    {
        loadposition = transform.position;
        localposit = transform.localScale;
        newloadpos = transform.localScale*5;
        scoreText = GameObject.Find("Canvas").transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
        rubyText = GameObject.Find("Canvas").transform.Find("Text (1)").GetComponent<UnityEngine.UI.Text>();
        foreach (var rr in FindObjectsOfType<Road>())
        {
            vspos.Add(rr.transform.position.x);
            roads.Add(rr.name);
        }
        vspos.Sort();
        roads.Sort();
        sec = 1;
        StartCoroutine(FDay());
        //currIndex = vspos.Count - 1;
    }
    public IEnumerator FDay()
    {
        if (disconnectplayer == false)
        {
            while (sec != 0)
            {
                
                yield return new WaitForSeconds(0.000001f);
                //transform.SetDirtyBit(1);
                Vector3 v = transform.position;
                if (addmove == 1)
                {
                    if (disconnectplayer == false)
                    {
                        MoveWorm();
                        addmove = 2;
                    }
                }
                transform.Translate(mDirection);
                //transform.Translate(mDirection);
                if (newHead == 0) yield return null;
                v = transform.position;
                ves = v;
                //transform.Translate(Direction);
                if (waypoints.Count > 0)
                {
                    waypoints.Last().position = v;
                    waypoints.Insert(0, waypoints.Last());
                    waypoints.RemoveAt(waypoints.Count - 1);
                }
                sec++;
            }
        }
    }
    protected void MoveWorm()
    {
        GameObject worm = (GameObject)MonoBehaviour.Instantiate(unitPrefab, transform.position, Quaternion.identity);
        worm.GetComponent<WirmCircle>().getworm(this);
        score += 1;
        wList.Add(worm);
        waypoints.Insert(0, worm.transform);
    }
    void CmdTranslate(Vector3 v)
    {
        
        new WaitForSeconds(0.005f);
    }
    void RpcTranslate(Vector3 v)
    {
        //Direction = mDirection;
        //newdir = Direction;
    }
    public void plusindex(int mat)
    {
        //currIndex -= 1;
    }
    float xmove = 0f;
    [SerializeField]int currIndex;
    List<GameObject> fireInd = new List<GameObject>();
    private void fireRestart()
    {
        fireInd.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        rubyText.text = rubys.ToString();
        //if (transform.position.x > vspos[currIndex] - 9.2f)
        //{
        //if (transform.position.x >= vspos[currIndex] + 12f&& vspos.Count-1!=currIndex) { print(currIndex); currIndex += 1; }
        //if(vspos.Count - 1 <= currIndex) { load3sec = true; 
        //    foreach(var pworm in FindObjectsOfType<Road>()) { pworm.newStart(); }
        //    StartCoroutine(GetLoseOrFinish("You Win Finish!!!"));
        //    transform.position = loadposition;
        //    UnityEngine.SceneManagement.SceneManager.LoadScene("offlinescene4");
        //}
        //}

        if (rbs >= 4)
        {
            //load5sec = true;
            //StartCoroutine(GetEnumeratorIK());
            //rbs = 0;
        }
        if (fireInd.Count <= 3)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                GameObject fp = Instantiate(fire.gameObject, transform.position, Quaternion.identity);
                fireInd.Add(fp);
                fp.GetComponent<FirePoint>().my = true;
            }
        }
        else { Invoke("fireRestart", 1); }
        if (MMDebug.Raycast3DBoolean(transform.position, Vector3.right, 0.45f, rybiLayer, Color.red, true))
        {
            //rubys += 1;
        }
        var Xmove = Input.mousePosition.y;
        var Ymove = Input.GetAxis("Vertical");
        if (transform.position.y < -4.55f)
        {
            //newHead = 4; 
            transform.position = new Vector3(transform.position.x, 11, transform.position.z);
        }
        if (transform.position.y > 12.77f)
        {
            //newHead = 3;
            transform.position = new Vector3(transform.position.x, -4, transform.position.z);
        }
        if (Input.GetMouseButtonDown(0))
        {

        }

        if (Input.GetMouseButtonUp(0)) { }
        if (newHead == 1)
        {
            mDirection = Vector2.right * dirspeed;
        }
        else if (newHead == 2)
        {
            mDirection = -Vector2.right * dirspeed;
        }
        else if (newHead == 3)
        {
            mDirection = Vector2.up * dirspeed;
        }
        else if (newHead == 4)
        {
            mDirection = -Vector2.up * dirspeed;
        }
        else if (newHead == 0)
        {
            mDirection = Vector2.zero;
        }
        if (load5sec) { mDirection = Vector2.right * dirspeed; }
        //Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        //goMat.GetComponent<Renderer>().material = GetMaterial;
        scoreText.text = score.ToString();
    }
    bool load5sec = false;

    public bool Load5sec { get => load5sec; set => load5sec = value; }

    IEnumerator GetEnumeratorIK()
    {
        while (load5sec)
        {
            transform.localScale = newloadpos;
            yield return new WaitForSeconds(5.0f);
            transform.localScale = localposit;
            load5sec = false;
        }
    }
    bool load3sec=false;
    public bool besmertie() { return load3sec; }
    IEnumerator GetLoseOrFinish(string ttext)
    {
        while (load3sec)
        {
            GameObject.Find("Canvas").transform.Find("Text (2)").GetComponent<UnityEngine.UI.Text>().text = ttext;
            yield return new WaitForSeconds(3.0f);
            GameObject.Find("Canvas").transform.Find("Text (2)").GetComponent<UnityEngine.UI.Text>().text = "";
            currIndex = 0;
            load3sec = false;
        }
    }
    int rbs=0;
    int deadcount = 0;
    void OnCollisionEnter(Collision col)
    {
        if (load3sec == false)
        {
            deadcount++;
            if (col.gameObject.tag == "corm"||col.gameObject.tag == "fire"||col.gameObject.tag == "enem")
            {
                rbs = 0; rubys = 0f; score = 0;
                load3sec = true;
                FindObjectOfType<Road>().minuslive();
                StartCoroutine(GetLoseOrFinish("You Dead"));
                transform.position = loadposition;
            }
            if (col.gameObject.tag == "yad")
            {
                rbs = 0; rubys = 0f;score = 0;
                load3sec = true; FindObjectOfType<Road>().minuslive();
                StartCoroutine(GetLoseOrFinish("You Dead"));
                transform.position = loadposition;
               
            }
            if (col.gameObject.tag == "razor")
            {
                rbs = 0; rubys = 0f; score = 0;
                load3sec = true; FindObjectOfType<Road>().minuslive();
                StartCoroutine(GetLoseOrFinish("You Dead"));
                transform.position = loadposition;
                
            }
            if (col.gameObject.tag == "rub")
            {
                rbs += 1; rubys += 1f;
            }
        }
        //else if (load5sec == true)
        //{
        //    rbs = 0;
        //    if (col.gameObject.tag == "corm")
        //    {
        //        addmove = 1;
        //    }
        //    if (col.gameObject.tag == "yad")
        //    {
        //        score += 1;
        //    }
        //    if (col.gameObject.tag == "razor")
        //    {
                
        //    }
        //    if (col.gameObject.tag == "rub")
        //    {
        //        rubys += 1f;
        //    }
        //}
    }
}
