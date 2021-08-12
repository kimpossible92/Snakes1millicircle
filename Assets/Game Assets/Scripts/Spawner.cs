using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

	public GameObject[] groups;
    public int nextId;
	int spcount=0;
	public int ccount;
	List<GroupView> Views = new List<GroupView>();
	public List<plitka> GetPlitkasSpawn = new List<plitka>();
	public List<float> lpositions;
	public float AdLast=0;
	public float speed1 = 0.5f;
	[SerializeField] InputField iinput1, input2;
	public void addviews(GroupView groupView)
    {
		Views.Add(groupView);
	}
	public List<GroupView> getViews()
    {
		return Views;
    }
	//[SerializeField] GameObject SetRot;

	// Use this for initialization
	void Start () {
        nextId = Random.Range(0, groups.Length);
        spawnNext ();
	}

	// Update is called once per frame
	void Update() {
		// nothing
		if (iinput1.text == "1") { nextId = 0; }
		if (iinput1.text == "2") { nextId = 1; }
		if (iinput1.text == "3") { nextId = 2; }
		if (input2.text == "1") { speed1 = 0.05f; }
		if (input2.text == "2") { speed1 = 1f; }
		if (input2.text == "3") { speed1 = 1.5f; }
	}

    public GameObject createGroup(Vector3 v) {
        GameObject group = Instantiate(groups[nextId], v, Quaternion.identity);
      
        //group.transform.SetParent(SetRot.transform);
        //group.transform.SetParent(GameObject.FindGameObjectWithTag("Board").transform);
        //group.transform.position *= canvas.scaleFactor; bug bug bug everywhere
        // solved in another way: just adjust the UI HUD to scale and keep this shit constant
        return group;
    }

	// spawnNext group block
	public void spawnNext() {
		spcount++;
		// Spawn Group at current Position
        createGroup(transform.position);
        nextId = Random.Range(0, groups.Length);
	}
}
