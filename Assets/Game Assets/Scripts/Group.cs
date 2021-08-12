using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Scripting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Group : MonoBehaviour
{

    // time of the last fall, used to auto fall after 
    // time parametrized by `level`
    private float lastFall;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    [SerializeField] List<plitka> plitki = new List<plitka>();
    Vector2 currentSwipe;
    // last key pressed time, to handle long press behavior
    private float lastKeyDown;
    private float timeKeyPressed;
    public Block[,] gems3 = new Block[2, 2];
    int delcount = 0;
    [SerializeField] GameObject Outline;
    public void AlignCenter()
    {
        transform.position += transform.position - Utilits.Center(gameObject);
    }
    public List<plitka> GetPlitkas()
    {
        return plitki;
    }
    bool isvalid2()
    {
        foreach (Transform child in transform)
        {
            Vector3 v = Grid.roundVector2(child.position);

            // not inside Border?
            if (!Grid.insideBorder2(v))
            {
                return false;
            }
            // Block in grid cell (and not par of same group)?
            if (Grid.grid[(int)(v.x), (int)(v.y)] != null &&
                Grid.grid[(int)(v.x), (int)(v.y)].parent != transform)
            {
                return false;
            }
        }

        return true;
    }
    int tcount = 0;
    bool isvalidpos()
    {
        var loader = FindObjectOfType<Spawner>().getViews().Last();
        if (loader == null) { loader = FindObjectOfType<Spawner>().getViews()[FindObjectOfType<Spawner>().getViews().Count - (delcount + 1)]; }
        if (FindObjectOfType<Spawner>().getViews().Count > 0 &&
            GetComponent<RectTransform>().anchoredPosition3D.y -
            loader.GetComponent<RectTransform>().anchoredPosition3D.y <= 0.5f) { return false; }
        return true;
    }
    bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector3 v = Grid.roundVector2(child.position);

            // not inside Border?
            if (!Grid.insideBorder(v))
            {
                return false;
            }
            // Block in grid cell (and not par of same group)?
            if (Grid.grid[(int)(v.x), (int)(v.y)] != null &&
                Grid.grid[(int)(v.x), (int)(v.y)].parent != transform)
            {
                return false;
            }
        }

        return true;
    }
    public void updateleft(plitka pl)
    {
        if (pl.x1 == 0 && pl.x2 == 0) { pl.x1 = 1; pl.x2 = 0; }
        if (pl.x1 == 0 && pl.x2 == 1) { pl.x1 = 0; pl.x2 = 0; }
        if (pl.x1 == 1 && pl.x2 == 1) { pl.x1 = 0; pl.x2 = 1; }
        if (pl.x1 == 1 && pl.x2 == 0) { pl.x1 = 1; pl.x2 = 1; }
    }
    public void updateright(plitka pl)
    {
        if (pl.x1 == 0 && pl.x2 == 0) { pl.x1 = 0; pl.x2 = 1; }
        if (pl.x1 == 0 && pl.x2 == 1) { pl.x1 = 1; pl.x2 = 1; }
        if (pl.x1 == 1 && pl.x2 == 1) { pl.x1 = 1; pl.x2 = 0; }
        if (pl.x1 == 1 && pl.x2 == 0) { pl.x1 = 0; pl.x2 = 0; }
    }
    // update the grid
    void updateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < Grid.h; ++y)
        {
            for (int x = 0; x < Grid.w; ++x)
            {
                if (Grid.grid[x, y] != null &&
                    Grid.grid[x, y].parent == transform)
                {
                    Grid.grid[x, y] = null;
                }
            }
        }

        insertOnGrid();
    }

    void insertOnGrid()
    {
        // add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.roundVector2(child.position);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }
    }
    void MoveDownBlocksBelowRow(int row)
    {
        foreach (plitka bl in FindObjectOfType<Spawner>().GetPlitkasSpawn)
        {
            if (bl.GetPosition.Row > row)
            {
                bl.MoveBy(-1, 0);
            }
        }
    }
    void gameOver()
    {
        Debug.Log("GAME OVER!");
        //while (!isValidGridPos()) {
        //    //Debug.LogFormat("Updating last group...: {0}", transform.position);
        //    transform.position += new Vector3(0, 1, 0);
        //}
        //updateGrid(); // to not overleap invalid groups
        enabled = false; // disable script when dies
        UIController.gameOver(); // active Game Over panel
        Highscore.Set(ScoreManager.score); // set highscore
        //Music.stopMusic(); // stop Music
    }

    // Use this for initialization
    void Start()
    {
        lastFall = Time.time;
        lastKeyDown = Time.time;
        timeKeyPressed = Time.time;
        //if (isValidGridPos()) {
        //    insertOnGrid();
        //} else {
        //    Debug.Log("KILLED ON START");
        //    gameOver();
        //}

    }
    int widthfour = 4;
    List<plitka> GetBlocksFromRow(int row)
    {
        return FindObjectOfType<Spawner>().GetPlitkasSpawn.FindAll(plit => plit.GetPosition.Row == row);
    }
    void Remove(List<plitka> blocksToRemove)
    {
        FindObjectOfType<Spawner>().GetPlitkasSpawn.RemoveAll(plit => blocksToRemove.Contains(plit));
    }
    public int maxrows()
    {
        int rows = 0;
        //for (int row = 20 - 1; row >= 0; --row)
        //{
        //    var rowBlocks = FindObjectOfType<Spawner>().GetPlitkasSpawn.FindAll(plit => plit.GetPosition.Row == row);
        //    //rows = rowBlocks.Last().GetPosition.Row;
        //}
        return rows;
    }
    public int RemoveFullRows()
    {
        int rowsRemoved = 0;
        for (int row = 100 - 1; row >= 0; --row)
        {
            var rowBlocks = GetBlocksFromRow(row);
            if (rowBlocks.Count >= widthfour)
            {
                foreach (var rb in rowBlocks) { print(rb.GetPosition.Row); Destroy(rb.gameObject); }
                MoveDownBlocksBelowRow(row);
                Remove(rowBlocks); rowsRemoved += 1;
            }
        }
        return rowsRemoved;
    }

    float loadheight; int yc = 0;
    public void isplitType()
    {
        int settc = 0;        
        for (int tc = 0; tc < 3; tc++)
        {
            if (FindObjectOfType<Spawner>().ccount - tc >= 0)
            {
                if (plitki.Count == 3)
                {
                    if (FindObjectOfType<Spawner>().GetPlitkasSpawn.Contains(plitki[0]) &&
                        FindObjectOfType<Spawner>().GetPlitkasSpawn.Contains(plitki[1]) &&
                        FindObjectOfType<Spawner>().GetPlitkasSpawn.Contains(plitki[2])
                        ) { settc = tc; }
                }
                if (plitki.Count == 2)
                {
                    if (FindObjectOfType<Spawner>().GetPlitkasSpawn.Contains(plitki[0])
                        &&FindObjectOfType<Spawner>().GetPlitkasSpawn.Contains(plitki[1])) {
                        settc = tc;
                    }
                }
                if (plitki.Count == 1)
                {
                    if (FindObjectOfType<Spawner>().GetPlitkasSpawn.Contains(plitki[0])) {
                        settc = tc;
                    }
                }

            }
        }
        FindObjectOfType<Spawner>().ccount = FindObjectOfType<Spawner>().ccount - settc;
    }
    public bool isplitkaPosition()
    {
        int countpl = 0;
        foreach (var pltt in plitki)
        {
            if (GetBlocksFromRow(FindObjectOfType<Spawner>().ccount).Find(plk =>
            plk.GetPosition.Column == pltt.x1 &&
            plk.GetPosition.Row2 == pltt.x2))
            {

            }
            else
            {
                countpl++;
                //print(countpl);
                //print(plitki.Count);
                //print(yc);
                //print(GetBlocksFromRow(yc).Count);
            }
        }
        if (countpl == plitki.Count)
        {
            return true;
        }
        return false;
    }
    public void upcountravno0()
    {
        upcount = 0;
    }
    public void isFallDown()
    {
        if (FindObjectOfType<Spawner>().GetPlitkasSpawn.Count > 0)
        {
            loadheight = Mathf.Max(0, FindObjectOfType<Spawner>().lpositions.Last());
            if (GetComponent<RectTransform>().anchoredPosition3D.y < FindObjectOfType<Spawner>().AdLast + 0.56f)
            {
                //isrot2();
                Outline.SetActive(false);
                //if (ismousedown == false)
                //{
                isplitType();
                if (isplitkaPosition())
                {
                    yc = (int)FindObjectOfType<Spawner>().AdLast * 2;
                    foreach (var plt in plitki)
                    {
                        plt.y = FindObjectOfType<Spawner>().ccount;
                        if (plt.y < 0) { plt.y = 0; }
                        plt.SetPosition(plt.x1, plt.y, plt.x2); print(plt.GetPosition.Row);
                        FindObjectOfType<Spawner>().GetPlitkasSpawn.Add(plt);
                    }
                    if (plitki.Count + GetBlocksFromRow(FindObjectOfType<Spawner>().ccount).Count >= 4)
                    {
                        //FindObjectOfType<Spawner>().AdLast -= 0.5f;
                        GetComponent<RectTransform>().anchoredPosition3D
                            = new Vector3(GetComponent<RectTransform>().anchoredPosition3D.x,
                        FindObjectOfType<Spawner>().AdLast, GetComponent<RectTransform>().anchoredPosition3D.z);
                        FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                        //int rowsCount = RemoveFullRows();
                        FindObjectOfType<Spawner>().AdLast = FindObjectOfType<Spawner>().ccount * 0.5f;
                        int fcount = 0;
                        foreach (var pltt in GetBlocksFromRow(FindObjectOfType<Spawner>().ccount))
                        {
                            if (GetBlocksFromRow(FindObjectOfType<Spawner>().ccount - 1)
                                .Find(plk => plk.GetPosition.Column == pltt.x1 && plk.GetPosition.Row2 == pltt.x2))
                            {

                            }
                            else
                            {
                                fcount += 1;
                            }
                        }
                        if (fcount == GetBlocksFromRow(FindObjectOfType<Spawner>().ccount).Count)
                        {
                            foreach (var pltt in GetBlocksFromRow(FindObjectOfType<Spawner>().ccount))
                            {
                                pltt.y -= 1;
                                pltt.SetPosition(pltt.x1, pltt.y, pltt.x2);
                                pltt.transform.position -= new Vector3(0, 0.5f, 0);
                            }
                            var rowBlocks = GetBlocksFromRow(FindObjectOfType<Spawner>().ccount - 1);
                            if (rowBlocks.Count >= 4)
                            {
                                var rowblock2 = GetBlocksFromRow(FindObjectOfType<Spawner>().ccount);
                                int rowsCount3 = RemoveFullRows();
                                foreach (var rb in rowBlocks) { }
                                FindObjectOfType<Spawner>().ccount -= rowsCount3;
                                FindObjectOfType<Spawner>().AdLast = FindObjectOfType<Spawner>().ccount * 0.5f;
                            }
                            else
                            {
                                int rowsCount4 = RemoveFullRows();
                                FindObjectOfType<Spawner>().ccount -= rowsCount4;
                                FindObjectOfType<Spawner>().AdLast = FindObjectOfType<Spawner>().ccount * 0.5f;
                            }
                        }
                        else
                        {
                            int rowsCount4 = RemoveFullRows();
                            FindObjectOfType<Spawner>().ccount -= rowsCount4;
                            FindObjectOfType<Spawner>().AdLast = FindObjectOfType<Spawner>().ccount * 0.5f;
                        }
                        //FindObjectOfType<Spawner>().lpositions.Add(GetComponent<RectTransform>().anchoredPosition3D.y);
                        //FindObjectOfType<Spawner>().spawnNext();
                        //foreach(var gv in FindObjectOfType<Spawner>().getViews())
                        //{
                        //    if (gv.GetComponent<RectTransform>().anchoredPosition3D.y - (rowsCount * 0.5f) >= 0) { gv.GetComponent<RectTransform>().anchoredPosition3D -= new Vector3(0, rowsCount * 0.5f, 0); }
                        //    foreach(var gp in gv.GetPlitkas()) { if (gp.GetPosition.Row - rowsCount >= 0) { gp.SetPosition(gp.GetPosition.Column, gp.GetPosition.Row - rowsCount, gp.GetPosition.Row2); } }
                        //}
                        //foreach (var gv in FindObjectOfType<Spawner>().GetPlitkasSpawn)
                        //{
                        //    gv.view.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(
                        //        gv.view.GetComponent<RectTransform>().anchoredPosition3D.x,
                        //        0f,gv.view.GetComponent<RectTransform>().anchoredPosition3D.z);
                        //    gv.SetPosition(gv.GetPosition.Column, 0, gv.GetPosition.Row2);
                        //}
                        enabled = false;
                    }
                    else
                    {
                        //FindObjectOfType<Spawner>().AdLast += 0.5f;
                        GetComponent<RectTransform>().anchoredPosition3D = new Vector3(GetComponent<RectTransform>().anchoredPosition3D.x,
                       FindObjectOfType<Spawner>().AdLast, GetComponent<RectTransform>().anchoredPosition3D.z);
                        FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                        FindObjectOfType<Spawner>().lpositions.Add(GetComponent<RectTransform>().anchoredPosition3D.y);
                        int fcount = 0;
                        foreach (var pltt in GetBlocksFromRow(FindObjectOfType<Spawner>().ccount))
                        {
                            if (GetBlocksFromRow(FindObjectOfType<Spawner>().ccount - 1)
                                .Find(plk => plk.GetPosition.Column == pltt.x1 && plk.GetPosition.Row2 == pltt.x2))
                            {

                            }
                            else
                            {
                                fcount += 1;
                            }
                        }
                        if (fcount == GetBlocksFromRow(FindObjectOfType<Spawner>().ccount).Count)
                        {
                            foreach (var pltt in GetBlocksFromRow(FindObjectOfType<Spawner>().ccount))
                            {
                                pltt.y -= 1;
                                pltt.SetPosition(pltt.x1, pltt.y, pltt.x2);
                                pltt.transform.position -= new Vector3(0, 0.5f, 0);
                            }
                            var rowBlocks = GetBlocksFromRow(FindObjectOfType<Spawner>().ccount - 1);
                            if (rowBlocks.Count >= 4)
                            {
                                //foreach (var rb in rowBlocks) { Destroy(rb.gameObject); }
                                //Remove(rowBlocks);
                                var rowblock2 = GetBlocksFromRow(FindObjectOfType<Spawner>().ccount);
                                int rowsCount = RemoveFullRows();
                                foreach (var rb in rowBlocks) { }
                                FindObjectOfType<Spawner>().ccount -= rowsCount;
                                FindObjectOfType<Spawner>().AdLast -= 0.5f;
                            }
                        }
                        //FindObjectOfType<Spawner>().spawnNext();
                        enabled = false;
                    }
                }
                else
                {
                    //isplitkaPosition();
                    FindObjectOfType<Spawner>().ccount += 1;

                    FindObjectOfType<Spawner>().AdLast += 0.5f;
                    yc = (int)FindObjectOfType<Spawner>().AdLast * 2;
                    foreach (var plt in plitki)
                    {
                        plt.y = FindObjectOfType<Spawner>().ccount;
                        plt.SetPosition(plt.x1, plt.y, plt.x2); print(plt.GetPosition.Row);
                        FindObjectOfType<Spawner>().GetPlitkasSpawn.Add(plt);
                    }

                    GetComponent<RectTransform>().anchoredPosition3D = new Vector3(GetComponent<RectTransform>().anchoredPosition3D.x,
                    FindObjectOfType<Spawner>().AdLast, GetComponent<RectTransform>().anchoredPosition3D.z);
                    FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                    FindObjectOfType<Spawner>().lpositions.Add(GetComponent<RectTransform>().anchoredPosition3D.y);
                    int fcount = 0;
                    foreach (var pltt in GetBlocksFromRow(FindObjectOfType<Spawner>().ccount))
                    {
                        if (GetBlocksFromRow(FindObjectOfType<Spawner>().ccount - 1)
                            .Find(plk => plk.GetPosition.Column == pltt.x1 && plk.GetPosition.Row2 == pltt.x2))
                        {

                        }
                        else
                        {
                            fcount += 1;
                        }
                    }
                    if (fcount == GetBlocksFromRow(FindObjectOfType<Spawner>().ccount).Count)
                    {
                        foreach (var pltt in GetBlocksFromRow(FindObjectOfType<Spawner>().ccount))
                        {
                            pltt.y -= 1;
                            pltt.SetPosition(pltt.x1, pltt.y, pltt.x2);
                            pltt.transform.position -= new Vector3(0, 0.5f, 0);
                        }
                        var rowBlocks = GetBlocksFromRow(FindObjectOfType<Spawner>().ccount - 1);
                        if (rowBlocks.Count >= 4)
                        {
                            //foreach (var rb in rowBlocks) { Destroy(rb.gameObject); }
                            //Remove(rowBlocks);
                            var rowblock2 = GetBlocksFromRow(FindObjectOfType<Spawner>().ccount);
                            int rowsCount = RemoveFullRows();
                            foreach (var rb in rowBlocks) { }
                            FindObjectOfType<Spawner>().ccount -= rowsCount;
                            FindObjectOfType<Spawner>().AdLast -= 0.5f;
                        }
                    }
                    //FindObjectOfType<Spawner>().spawnNext();
                }
                //}
                enabled = false;
            }
            else
            {
                GetComponent<RectTransform>().anchoredPosition3D += new Vector3(0, -FindObjectOfType<Spawner>().speed1, 0);
            }
        }
        else
        {
            //yc = 0;
            foreach (var plt in plitki)
            {
                plt.SetPosition(plt.x1, 0, plt.x2); print(plt.GetPosition.Row);
                FindObjectOfType<Spawner>().GetPlitkasSpawn.Add(plt);
            }
            GetComponent<RectTransform>().anchoredPosition3D = new Vector3(GetComponent<RectTransform>().anchoredPosition3D.x,
                0f, GetComponent<RectTransform>().anchoredPosition3D.z);
            FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
            FindObjectOfType<Spawner>().AdLast = 0f;
            FindObjectOfType<Spawner>().lpositions.Add(0f);
            //FindObjectOfType<Spawner>().spawnNext();
            enabled = false;
        }
    }
    void tryChangePos(Vector3 v)
    {
        transform.position += v;

        // See if valid
        if (isValidGridPos())
        {
            updateGrid();
        }
        else
        {
            transform.position -= v;
        }
    }

    void fallGroup()
    {
        //Grid.count = 0;
        //// modify
        transform.position += new Vector3(0, -0.5f, 0);

        if (isValidGridPos())
        {
            // It's valid. Update grid... again
            updateGrid();
        }
        else
        {
            // it's not valid. revert
            transform.position += new Vector3(0, 0.5f, 0);

            // Clear filled horizontal lines
            Grid.deleteFullRows();
            //FindObjectOfType<Spawner>().spawnNext();


            // Disable script
            enabled = false;
        }

        lastFall = Time.time;

    }

    // getKey if is pressed now on longer pressed by 0.5 seconds | if that true apply the key each 0.05f while is pressed
    bool getKey(KeyCode key)
    {
        bool keyDown = Input.GetKeyDown(key);
        bool pressed = Input.GetKey(key) && Time.time - lastKeyDown > 0.5f && Time.time - timeKeyPressed > 0.05f;

        if (keyDown)
        {
            lastKeyDown = Time.time;
        }
        if (pressed)
        {
            timeKeyPressed = Time.time;
        }

        return keyDown || pressed;
    }
    List<float> posymax;
    [SerializeField] GroupView loader;
    void recordFirstPressPos()
    {
        //Первый Vector2 равно новый Vector2 с координатами мыши по x и y с отрицательным знаком
        firstPressPos = new Vector2(-Input.mousePosition.x, -Input.mousePosition.y);
    }
    //Метод получения координат мыши во 2 положении
    void recordSecondPressPos()
    {
        //Второй Vector2 равно новый Vector2 с координатами мыши по x и y с отрицательным знаком
        secondPressPos = new Vector2(-Input.mousePosition.x, -Input.mousePosition.y);
    }
    int l12 = 0;
    int counts = 0; bool iscounts = false;
    bool isrot = false;
    // Perform an incremental collection every time we allocate more than 8 MB
    const long kCollectAfterAllocating = 8 * 1024 * 1024;

    // Perform an instant, full GC if we have more than 128 MB of managed heap.
    const long kHighWater = 128 * 1024 * 1024;

    long lastFrameMemory = 0;
    long nextCollectAt = 0;
    public void updateCollect()
    {
        long mem = Profiler.GetMonoUsedSizeLong();
        if (mem < lastFrameMemory)
        {
            // GC happened.
            nextCollectAt = mem + kCollectAfterAllocating;
        }
        if (mem > kHighWater)
        {
            // Trigger immediate GC
            System.GC.Collect(0);
        }
        else if (mem >= nextCollectAt)
        {
            // Trigger incremental GC
            //bool v = GarbageCollector.CollectIncremental();
            lastFrameMemory = mem + kCollectAfterAllocating;
        }
        lastFrameMemory = mem;
    }
    bool left = false;
    public void isrot2()
    {
        if (plitki.Count == 1)
        {
            if (upcount == 1)
            {
                plitki[0].x1 = 0; plitki[0].x2 = 1; plitki[0].updatepos();
            }
            else if (upcount == 2)
            {
                plitki[0].x1 = 1; plitki[0].x2 = 0; plitki[0].updatepos();
            }
            else if (upcount == 3)
            {
                plitki[0].x1 = 1; plitki[0].x2 = 1; plitki[0].updatepos();
            }
            else
            {
                plitki[0].x1 = 0; plitki[0].x2 = 0; plitki[0].updatepos(); upcount = 0;
            }
        }
        if (plitki.Count == 2)
        {
            if (upcount == 1)
            {
                plitki[0].x1 = 1; plitki[0].x2 = 1; plitki[0].updatepos();
                plitki[1].x1 = 0; plitki[1].x2 = 1; plitki[1].updatepos();
            }
            else if (upcount == 2)
            {
                plitki[0].x1 = 1; plitki[0].x2 = 1; plitki[0].updatepos();
                plitki[1].x1 = 1; plitki[1].x2 = 0; plitki[1].updatepos();
            }
            else if (upcount == 3)
            {
                plitki[0].x1 = 1; plitki[0].x2 = 1; plitki[0].updatepos();
                plitki[1].x1 = 0; plitki[1].x2 = 0; plitki[1].updatepos();
            }
            else
            {
                plitki[0].x1 = 0; plitki[0].x2 = 0; plitki[0].updatepos();
                plitki[1].x1 = 1; plitki[1].x2 = 0; plitki[1].updatepos(); upcount = 0;
            }
        }
        if (plitki.Count == 3)
        {
            if (plitki.First().GetPosition.Column == plitki.Last().GetPosition.Column && plitki.First().GetPosition.Row2 == plitki.Last().GetPosition.Row2)
            {
                plitki[0].x1 = 1; plitki[0].x2 = 0; plitki[0].updatepos();
                plitki[1].x1 = 1; plitki[1].x2 = 1; plitki[1].updatepos();
                plitki[2].x1 = 0; plitki[2].x2 = 1; plitki[2].updatepos();
            }
            if (left == true)
            {
                left = false;
            }
        }
    }
    bool ismousedown = false;
    private int upcount;

    public void Swipe()
    {
        if (FindObjectOfType<Spawner>().ccount > 100)
        {
            gameOver();
        }
        if (Input.GetMouseButtonDown(0))
        {
            ismousedown = true;
            l12++;
            Outline.SetActive(true); iscounts = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            tcount += 1;
            ismousedown = false; upcount++;
            l12 = 0;
            isrot2();
            isrot = true;
            foreach (var pp in plitki) { pp.updatepos(); }
            Outline.SetActive(false); iscounts = false;
        }
        if (l12 >= 1)
        {
            //print(l12);
            //Запоминаем координаты мыши в 1 положении
            if (l12 == 1)
            {
                recordFirstPressPos();
            }
            //Запоминаем координаты мыши во 2 положении через 0.15 секунды
            Invoke("recordSecondPressPos", 1f);

            //Задаём направление свайпа, вычитая из первых координат мыши вторые.
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //Нормализуем вектор направления свайпа
            currentSwipe.Normalize();
            //print(currentSwipe);
            //Если координата свайпа по y > 0 и по x > -0.2, и по x < 0.2. Тобиш находится в промежутке между -0.2 до 0.2 по x
            if (currentSwipe.y > 0 && currentSwipe.x > -0.3f && currentSwipe.x < 0.3f)
            {

                //transform.Rotate(0, 0, -180);
                // see if valid
                if (isValidGridPos())
                {
                    // it's valid. Update grid
                    updateGrid();
                }
                else
                {
                    // it's not valid. revert
                    //transform.Rotate(0, 0, 180);
                }
                //l12++;
            }
            //Аналогично
            if (currentSwipe.y < 0 && currentSwipe.x > -0.3f && currentSwipe.x < 0.3f)
            {
                //while (enabled)
                //{ // fall until the bottom 
                //    fallGroup();
                //}
                //l12++;
            }
            if (currentSwipe.x < -0.5f && currentSwipe.y > -0.3f && currentSwipe.y < 0.3f)
            {
                if (isrot) { foreach (var pp in plitki) { updateleft(pp); pp.updatepos(); } isrot = false; }
                //transform.Rotate(0, -90, 0);
                l12++;
            }
            if (currentSwipe.x > 0.5f && currentSwipe.y > -0.3f && currentSwipe.y < 0.3f)
            {
                if (isrot)
                {
                    foreach (var pp in plitki) { updateright(pp); pp.updatepos(); }
                    isrot = false;
                }
                //transform.Rotate(0, 90, 0);
                l12++;
            }
        }
        isFallDown();
        //isFallDown222();

    }
    public void isFallDown222()
    {
        if (FindObjectOfType<Spawner>().getViews().Count > 0)//&&FindObjectOfType<Spawner>().getViews().First()!=null)
        {
            foreach (var pl in plitki)
            {
                if (pl.x1 == 0 && pl.x2 == 0) { GetComponent<GroupView>().gems3[0, 0] = pl; }
                if (pl.x1 == 1 && pl.x2 == 0) { GetComponent<GroupView>().gems3[1, 0] = pl; }
                if (pl.x1 == 0 && pl.x2 == 1) { GetComponent<GroupView>().gems3[0, 1] = pl; }
                if (pl.x1 == 1 && pl.x2 == 1) { GetComponent<GroupView>().gems3[1, 1] = pl; }
            }
            loader = FindObjectOfType<Spawner>().getViews().Last();
            var ll1 = loader.GetPlitkas().Last();
            if (loader.GetPlitkas() != null)
            {
                if (counts >= FindObjectOfType<Spawner>().getViews().Count)
                {
                    counts = 0;
                    loader = FindObjectOfType<Spawner>().getViews().Last();
                }
                else
                {
                    if (ll1 == null) { loader = FindObjectOfType<Spawner>().getViews().FirstOrDefault(); }
                    if (loader == null) { loader = FindObjectOfType<Spawner>().getViews().FirstOrDefault(); }
                }
            }
            if ((loader.GetComponent<RectTransform>().anchoredPosition3D.y -
                GetComponent<RectTransform>().anchoredPosition3D.y) < 0.5f) { }

            if (isvalidpos())
            {
                GetComponent<RectTransform>().anchoredPosition3D += new Vector3(0, -FindObjectOfType<Spawner>().speed1, 0);
            }
            else
            {
                GetComponent<GroupView>().setPlitka(plitki);
                //FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                FindObjectOfType<Spawner>().spawnNext();
                foreach (var gr in plitki)
                {
                    if (loader.gems3[gr.x1, gr.x2] == null)
                    {
                        if (
                            //loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count== 3||
                            loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 4 ||
                            loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 5 ||
                            loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 6
                            )
                        {
                            //print(loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count);
                            //if (ll1 == null)
                            if (loader.GetComponent<RectTransform>().anchoredPosition3D.y < 0.5f)
                            {
                                loader.gems3[gr.x1, gr.x2] = gr; FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                                FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                                //loader.setPlitka(plitki);
                                GetComponent<RectTransform>().anchoredPosition3D = loader.GetComponent<RectTransform>().anchoredPosition3D;
                                foreach (var go in loader.GetPlitkas())
                                {
                                    if (go != null &&
                                        loader.GetPlitkas()[0].x1 != loader.GetPlitkas()[1].x1 &&
                                        loader.GetPlitkas()[0].x2 != loader.GetPlitkas()[1].x2
                                        )
                                    {
                                        Destroy(go.gameObject);
                                    }
                                }
                                //loader.cler();
                                delcount++;
                                //FindObjectOfType<Spawner>().getViews().Remove(loader);
                                foreach (var go in GetComponent<GroupView>().GetPlitkas())
                                {
                                    if (go != null &&
                                        GetComponent<GroupView>().GetPlitkas()[0].x1 != GetComponent<GroupView>().GetPlitkas()[1].x1 &&
                                        GetComponent<GroupView>().GetPlitkas()[0].x2 != GetComponent<GroupView>().GetPlitkas()[1].x2
                                        )
                                    {
                                        Destroy(go.gameObject);
                                    }
                                }
                                enabled = false;
                            }
                            else
                            {
                                loader.gems3[gr.x1, gr.x2] = gr; FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                                FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                                //loader.setPlitka(plitki);
                                GetComponent<RectTransform>().anchoredPosition3D = loader.GetComponent<RectTransform>().anchoredPosition3D;
                                foreach (var go in loader.GetPlitkas())
                                {
                                    if (go != null)
                                    {
                                        Destroy(go.gameObject);
                                    }
                                }
                                //loader.cler();
                                delcount++;
                                //FindObjectOfType<Spawner>().getViews().Remove(loader);
                                foreach (var go in GetComponent<GroupView>().GetPlitkas())
                                {
                                    if (go != null)
                                    {
                                        Destroy(go.gameObject);
                                    }
                                }
                                enabled = false;
                            }
                        }
                        else if (
                            loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 2 ||
                            loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 1
                            )
                        {
                            print(loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count);

                            GetComponent<RectTransform>().anchoredPosition3D = loader.GetComponent<RectTransform>().anchoredPosition3D;
                            //loader.gems3[gr.x1, gr.x2] = gr;loader.setPlitka(plitki);
                            FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                            enabled = false;
                        }
                        else
                        {
                            foreach (var pp in plitki) { pp.updatepos(); }
                            plitka pl = loader.GetPlitkas().Last(); loader.setPlitka(plitki);
                            if (
                           //loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count== 3||
                           loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 4 ||
                           loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 5 ||
                           loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 6
                           )
                            {
                                //print(loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count);
                                //if (ll1 == null)
                                if (loader.GetComponent<RectTransform>().anchoredPosition3D.y < 0.5f)
                                {
                                    loader.gems3[gr.x1, gr.x2] = gr; FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                                    FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                                    //loader.setPlitka(plitki);
                                    GetComponent<RectTransform>().anchoredPosition3D = loader.GetComponent<RectTransform>().anchoredPosition3D;
                                    foreach (var go in loader.GetPlitkas())
                                    {
                                        if (go != null &&
                                            loader.GetPlitkas()[0].x1 != loader.GetPlitkas()[1].x1 &&
                                            loader.GetPlitkas()[0].x2 != loader.GetPlitkas()[1].x2
                                            )
                                        {
                                            Destroy(go.gameObject);
                                        }
                                    }
                                    //loader.cler();
                                    delcount++;
                                    //FindObjectOfType<Spawner>().getViews().Remove(loader);
                                    foreach (var go in GetComponent<GroupView>().GetPlitkas())
                                    {
                                        if (go != null &&
                                            GetComponent<GroupView>().GetPlitkas()[0].x1 != GetComponent<GroupView>().GetPlitkas()[1].x1 &&
                                            GetComponent<GroupView>().GetPlitkas()[0].x2 != GetComponent<GroupView>().GetPlitkas()[1].x2
                                            )
                                        {
                                            Destroy(go.gameObject);
                                        }
                                    }
                                    enabled = false;
                                }
                                else
                                {
                                    loader.gems3[gr.x1, gr.x2] = gr; FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                                    FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                                    //loader.setPlitka(plitki);
                                    GetComponent<RectTransform>().anchoredPosition3D = loader.GetComponent<RectTransform>().anchoredPosition3D;
                                    foreach (var go in loader.GetPlitkas())
                                    {
                                        if (go != null)
                                        {
                                            Destroy(go.gameObject);
                                        }
                                    }
                                    //loader.cler();
                                    delcount++;
                                    //FindObjectOfType<Spawner>().getViews().Remove(loader);
                                    foreach (var go in GetComponent<GroupView>().GetPlitkas())
                                    {
                                        if (go != null)
                                        {
                                            Destroy(go.gameObject);
                                        }
                                    }
                                    enabled = false;
                                }
                            }
                            FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                            if (pl == null)
                            {
                                print(pl);
                                GetComponent<RectTransform>().anchoredPosition3D = loader.GetComponent<RectTransform>().anchoredPosition3D;
                            }
                            else
                            {
                                print("11"); print(loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count);

                                if (iscounts) { counts++; iscounts = false; }
                                FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>()); loader.gems3[gr.x1, gr.x2] = gr;
                                GetComponent<RectTransform>().anchoredPosition3D = loader.GetComponent<RectTransform>().anchoredPosition3D + new Vector3(0, 0.5f, 0);
                                if (loader.GetPlitkas().Count + GetComponent<GroupView>().GetPlitkas().Count == 4)
                                {
                                    foreach (var go in loader.GetPlitkas())
                                    {
                                        if (go != null &&
                                            loader.GetPlitkas()[0].x1 != loader.GetPlitkas()[1].x1 &&
                                            loader.GetPlitkas()[0].x2 != loader.GetPlitkas()[1].x2
                                            )
                                        {
                                            Destroy(go.gameObject);
                                        }
                                    }
                                    //loader.cler();
                                    delcount++;
                                    //FindObjectOfType<Spawner>().getViews().Remove(loader);
                                    foreach (var go in GetComponent<GroupView>().GetPlitkas())
                                    {
                                        if (go != null &&
                                            GetComponent<GroupView>().GetPlitkas()[0].x1 != GetComponent<GroupView>().GetPlitkas()[1].x1 &&
                                            GetComponent<GroupView>().GetPlitkas()[0].x2 != GetComponent<GroupView>().GetPlitkas()[1].x2
                                            )
                                        {
                                            Destroy(go.gameObject);
                                        }
                                    }
                                }
                                enabled = false;
                            }
                        }
                        //GetComponent<GroupView>().cler();
                    }
                    else// if (loader.gems3[gr.x1, gr.x2] != null) 
                    {
                        print("22");

                        var maxY = Mathf.Max(0, loader.GetComponent<RectTransform>().anchoredPosition3D.y);
                        if (iscounts) { counts++; iscounts = false; }
                        if (ll1 == null)
                        {
                            FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                            GetComponent<RectTransform>().anchoredPosition3D = new Vector3(GetComponent<RectTransform>().anchoredPosition3D.x,
                            maxY + 0.5f,
                            GetComponent<RectTransform>().anchoredPosition3D.z);
                        }
                        else
                        {
                            FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                            GetComponent<RectTransform>().anchoredPosition3D = new Vector3(GetComponent<RectTransform>().anchoredPosition3D.x,
                                maxY + 0.5f,
                                GetComponent<RectTransform>().anchoredPosition3D.z);
                        }
                        GetComponent<GroupView>().gems3[gr.x1, gr.x2] = loader.gems3[gr.x1, gr.x2];
                    }
                }
                enabled = false;
            }
            var loader2 = FindObjectOfType<Spawner>().getViews().Last();
            if (loader2 == null) { loader2 = FindObjectOfType<Spawner>().getViews()[FindObjectOfType<Spawner>().getViews().Count - (delcount + 1)]; }
            if (GetComponent<RectTransform>().anchoredPosition3D.y <= loader2
                .GetComponent<RectTransform>().anchoredPosition3D.y + 0.5f)
            {

            }
            else
            {

            }
        }
        else if (FindObjectOfType<Spawner>().getViews().Count == delcount)
        {
            if (GetComponent<RectTransform>().anchoredPosition3D.y > 0)
            {
                GetComponent<RectTransform>().anchoredPosition3D += new Vector3(0, -FindObjectOfType<Spawner>().speed1, 0);
            }
            else
            {
                foreach (var pl in plitki)
                {
                    if (pl.x1 == 0 && pl.x2 == 0) { GetComponent<GroupView>().gems3[0, 0] = pl; }
                    if (pl.x1 == 1 && pl.x2 == 0) { GetComponent<GroupView>().gems3[1, 0] = pl; }
                    if (pl.x1 == 0 && pl.x2 == 1) { GetComponent<GroupView>().gems3[0, 1] = pl; }
                    if (pl.x1 == 1 && pl.x2 == 1) { GetComponent<GroupView>().gems3[1, 1] = pl; }
                }
                GetComponent<GroupView>().setPlitka(plitki);
                //FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                FindObjectOfType<Spawner>().spawnNext();
                enabled = false;
            }
        }
        else
        {
            if (iscounts) { counts++; iscounts = false; }
            if (GetComponent<RectTransform>().anchoredPosition3D.y > 0)
            {
                GetComponent<RectTransform>().anchoredPosition3D += new Vector3(0, -FindObjectOfType<Spawner>().speed1, 0);
            }
            else
            {
                foreach (var pl in plitki)
                {
                    if (pl.x1 == 0 && pl.x2 == 0) { GetComponent<GroupView>().gems3[0, 0] = pl; }
                    if (pl.x1 == 1 && pl.x2 == 0) { GetComponent<GroupView>().gems3[1, 0] = pl; }
                    if (pl.x1 == 0 && pl.x2 == 1) { GetComponent<GroupView>().gems3[0, 1] = pl; }
                    if (pl.x1 == 1 && pl.x2 == 1) { GetComponent<GroupView>().gems3[1, 1] = pl; }
                }

                FindObjectOfType<Spawner>().addviews(GetComponent<GroupView>());
                //FindObjectOfType<Spawner>().spawnNext();
                enabled = false;
            }
        }
    }
    private IEnumerator DownInvoke()
    {
        while (GetComponent<RectTransform>().anchoredPosition3D.y > 0)
        {
            yield return new WaitForSeconds(1.5f);
            transform.position += new Vector3(0, -0.1f, 0);
            //yield return new WaitForSeconds(1.5f);
        }
    }
    public bool isdownEnd()
    {
        var loader = FindObjectOfType<Spawner>().getViews().Last();
        if (loader.transform.position.y - transform.position.y < 2)
        {
            return true;
        }
        return false;
    }
    public void falldownDelete()
    {
        if (GetComponent<GroupView>().gems3.Length >= 3)
        {
            Grid.deleteFullRows2();
        }
    }
    public void road12(int lr)
    {
        if (lr == -1)
        {
            if (transform.position.x == 6) tryChangePos(new Vector3(-1, 0, 0));
            if (transform.position.x == 5) tryChangePos(new Vector3(-1, 0, 0));
            ThisButtons.This.lr = 0;
        }
        if (lr == 1)
        {
            if (transform.position.x == 5) tryChangePos(new Vector3(1, 0, 0));
            if (transform.position.x == 4) tryChangePos(new Vector3(1, 0, 0));
            ThisButtons.This.lr = 0;
        }
    }
    void LateUpdate()
    {
        //road12(ThisButtons.This.lr);
        Swipe();
        if (UIController.isPaused)
        {
            return; // don't do nothing
        }
        if (getKey(KeyCode.LeftArrow))
        {
            //if (transform.position.x == 6) tryChangePos(new Vector3(-1, 0, 0));
            //if (transform.position.x == 5) tryChangePos(new Vector3(-1, 0, 0));
            //if (GetComponent<RectTransform>().rotation.y == 90)
            //{
            //    transform.position = new Vector3(4, transform.position.y, transform.position.z);
            //}
            //tryChangePos(new Vector3(-1, 0, 0));

        }
        else if (getKey(KeyCode.RightArrow))
        {  // Move right
           //if(transform.position.x==5)tryChangePos(new Vector3(1, 0, 0));
           //if(transform.position.x==4)tryChangePos(new Vector3(1, 0, 0));

        }
        else if (getKey(KeyCode.UpArrow) && gameObject.tag != "Cube")
        { // Rotate
          //transform.Rotate(0, 0, -90);

            // see if valid
            if (isValidGridPos())
            {
                // it's valid. Update grid
                updateGrid();
            }
            else
            {
                // it's not valid. revert
                //transform.Rotate(0, 0, 90);
            }
        }
        else if (getKey(KeyCode.DownArrow) || (Time.time - lastFall) >= (float)1 / Mathf.Sqrt(LevelManager.level))
        {
            //fallGroup();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            while (enabled)
            { // fall until the bottom 
                //fallGroup();
            }
        }

    }
}
