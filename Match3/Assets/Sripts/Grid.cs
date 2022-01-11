using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Grid : MonoBehaviour {
    public bool IsClickedFirst = false;
    public Block FirstBlock;
    public Vector2 FirstBlockPos;
    private bool[,] visited;
    public GameObject[] Blocks;
    public bool CanClick = true;
    public GameObject Selected;
    public Text disNumberText;
    private List<Vector2> rowRemoveList = new List<Vector2>();
    private List<Vector2> colRemoveList = new List<Vector2>();
    private List<Vector2> removeList = new List<Vector2>();
    public int Width = 8;
    public int Height = 8;
    public int disNumber = 0;
    public static Grid Instance;
    public Block[,] Map;
    public int[,] MapNum;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        visited = new bool[Width, Height];
        Map = new Block[Width, 2 * Height];
        MapNum = new int[Width, 2 * Height];
        Selected = Instantiate(Selected, transform.position, Quaternion.identity) as GameObject;
        Selected.SetActive(false);
        InitMapNum();
        InitMap();
    }
   
    private void InitMapNum()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < 2*Height; j++)
            {
                int num = Random.Range(0, Blocks.Length);
                MapNum[i, j] = num;
                while (IsLine(i, j))
                {
                    num = Random.Range(0, Blocks.Length);
                    MapNum[i, j] = num;
                }
            }
        }

        if (IsDead())
        {
            InitMapNum();
        }
    }

    private void InitMap()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < 2 * Height; j++)
            {
                GameObject obj = Instantiate(Blocks[MapNum[i, j]], new Vector2(i, j), Quaternion.identity) as GameObject;
                Map[i, j] = obj.GetComponent<Block>();
                if (j >= Height)
                    obj.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
    private bool IsLine(int i, int j)
    {
        if (i >= 2 && MapNum[i, j] == MapNum[i - 1, j] && MapNum[i, j] == MapNum[i - 2, j])
            return true;
        if (j >= 2 && MapNum[i, j] == MapNum[i, j - 1] && MapNum[i, j] == MapNum[i, j - 2])
            return true;
        return false;
    }
    /*
 * O O
 *  X
 * O O
 */
    private bool IsFirstLineCast(int x, int y)
    {
        int lx = x - 1;
        int ly = y - 1;
        int tx = x + 1;
        int ty = y + 1;
        bool isLeftBottomSame = (lx >= 0 && ly >= 0 && MapNum[lx, ly] == MapNum[x, y]);
        bool isLeftTopSame = (lx >= 0 && ty < Height && MapNum[lx, ty] == MapNum[x, y]);
        bool isRightBottomSame = (tx < Width && ly >= 0 && MapNum[tx, ly] == MapNum[x, y]);
        bool isRightTopSame = (tx < Width && ty < Height && MapNum[tx, ty] == MapNum[x, y]);

        // 左下角与右下角
        if (isLeftBottomSame && isRightBottomSame)
            return true;
        // 左下角与左上角
        if (isLeftBottomSame && isLeftTopSame)
            return true;
        // 左上角与右上角
        if (isLeftTopSame && isRightTopSame)
            return true;
        // 右上角与右下角
        if (isRightTopSame && isRightBottomSame)
            return true;

        return false;
    }

    /*
     * O O
     *  X
     *  X
     * O O
     */
    private bool IsSecondLineCast(int x, int y)
    {
        int lx = x - 1;
        int ly = y - 2;
        int tx = x + 1;
        int ty = y + 1;
        bool isLeftBottomSame = (lx >= 0 && ly >= 0 && MapNum[lx, ly] == MapNum[x, y]);
        bool isLeftTopSame = (lx >= 0 && ty < Height && MapNum[lx, ty] == MapNum[x, y]);
        bool isRightBottomSame = (tx < Width && ly >= 0 && MapNum[tx, ly] == MapNum[x, y]);
        bool isRightTopSame = (tx < Width && ty < Height && MapNum[tx, ty] == MapNum[x, y]);

        if (y - 1 >= 0 && MapNum[x, y - 1] == MapNum[x, y])
        {
            // 左上角
            if (isLeftTopSame)
                return true;
            // 右上角
            if (isRightTopSame)
                return true;
            // 左下角
            if (isLeftBottomSame)
                return true;
            // 右下角
            if (isRightBottomSame)
                return true;
        }
        return false;
    }

    /*
     * O  O
     *  XX
     * O  O
     */
    private bool IsThirdLineCast(int x, int y)
    {
        int lx = x - 1;
        int ly = y - 1;
        int tx = x + 2;
        int ty = y + 1;
        bool isLeftBottomSame = (lx >= 0 && ly >= 0 && MapNum[lx, ly] == MapNum[x, y]);
        bool isLeftTopSame = (lx >= 0 && ty < Height && MapNum[lx, ty] == MapNum[x, y]);
        bool isRightBottomSame = (tx < Width && ly >= 0 && MapNum[tx, ly] == MapNum[x, y]);
        bool isRightTopSame = (tx < Width && ty < Height && MapNum[tx, ty] == MapNum[x, y]);

        if (x + 1 < Width && MapNum[x + 1, y] == MapNum[x, y])
        {
            // 左上角
            if (isLeftTopSame)
                return true;
            // 右上角
            if (isRightTopSame)
                return true;
            // 左下角
            if (isLeftBottomSame)
                return true;
            // 右下角
            if (isRightBottomSame)
                return true;
        }
        return false;
    }
    // 遍历地图判断是否是死图
    public bool IsDead()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (IsFirstLineCast(i, j))
                    return false;
                if (IsSecondLineCast(i, j))
                    return false;
                if (IsThirdLineCast(i, j))
                    return false;
            }
        }
        return true;
    }
    private bool IsValidedLine(int x, int y, int maxLength)
    {
        rowRemoveList.Clear();
        colRemoveList.Clear();
        rowRemoveList.Add(new Vector2(x, y));
        colRemoveList.Add(new Vector2(x, y));

        // 左
        for (int i = -1; i >= -maxLength; i--)
        {
            if (x + i >= 0 && MapNum[x, y] == MapNum[x + i, y])
                rowRemoveList.Add(new Vector2(x + i, y));
            else
                break;
        }
        // 右
        for (int i = 1; i <= maxLength; i++)
        {
            if (x + i < Width && MapNum[x, y] == MapNum[x + i, y])
                rowRemoveList.Add(new Vector2(x + i, y));
            else
                break;
        }
        // 上
        for (int j = 1; j <= maxLength; j++)
        {
            if (y + j < Height && MapNum[x, y] == MapNum[x, y + j])
                colRemoveList.Add(new Vector2(x, y + j));
            else
                break;
        }
        // 下
        for (int j = -1; j >= -maxLength; j--)
        {
            if (y + j >= 0 && MapNum[x, y] == MapNum[x, y + j])
                colRemoveList.Add(new Vector2(x, y + j));
            else
                break;
        }

        if (rowRemoveList.Count >= 3 || colRemoveList.Count >= 3)
            return true;
        return false;
    }
    public bool IsValid(int x, int y)
    {
        removeList.Clear();

        bool isValid = false;
        if (IsValidedLine((int)FirstBlockPos.x, (int)FirstBlockPos.y, 2))
        {
            isValid = true;
            AddValidBlocks();
        }
        if (IsValidedLine(x, y, 2))
        {
            isValid = true;
            AddValidBlocks();
        }

        return isValid;
    }
    private void AddValidBlocks()
    {
        bool isRowValid = rowRemoveList.Count >= 3;
        bool isColValid = colRemoveList.Count >= 3;
        List<Vector2> list = new List<Vector2>();

        if (isRowValid && isColValid)
            colRemoveList.RemoveAt(0);
        if (isRowValid)
            list.AddRange(rowRemoveList);
        if (isColValid)
            list.AddRange(colRemoveList);

        foreach (var one in list)
        {
            visited[(int)one.x, (int)one.y] = true;
        }

        removeList.AddRange(list);
    }
    // 销毁动画
    public void PlayDestroyAnimation()
    {
        Sequence seq = DOTween.Sequence();

        for (int i = removeList.Count - 1; i >= 0; i--)
        {
            int x = (int)removeList[i].x;
            int y = (int)removeList[i].y;
            seq.Insert(0, Map[x, y].transform.DORotate(new Vector3(0, 90, 0), 0.3f));
            disNumber++;
        }
        disNumberText.text = "分数：" + disNumber;

        seq.AppendCallback(DestroyValidBlocks);
    }
    // 销毁已找到的可消除行
    private void DestroyValidBlocks()
    {
        for (int i = removeList.Count - 1; i >= 0; i--)
        {
            int x = (int)removeList[i].x;
            int y = (int)removeList[i].y;
            MapNum[x, y] = -1;
            removeList.RemoveAt(i);
            Destroy(Map[x, y].gameObject);
        }

        BlocksDown();
    }
    // 宝石下落
    public void BlocksDown()
    {
        Sequence seq = DOTween.Sequence();

        for (int x = 0; x < Width; x++)
        {
            int upBlock = 0;
            int upSpace = 0;
            for (int y = 0; y < Height; y++)
            {
                if (MapNum[x, y] == -1)
                {
                    upBlock = y + 1;
                    while (upBlock < 2 * Height && MapNum[x, upBlock] == -1)
                    {
                        upBlock++;
                    }

                    upSpace = upBlock + 1;
                    while (upSpace < 2 * Height && MapNum[x, upSpace] != -1)
                    {
                        upSpace++;
                    }

                    if (upBlock >= 0 && upBlock < 2 * Height)
                    {
                        for (int k = upBlock; k < upSpace; k++)
                        {
                            seq.Insert(0, Map[x, k].transform.DOMoveY(y + k - upBlock, (k - y) * 0.1f));
                            MapNum[x, y + k - upBlock] = MapNum[x, k];
                            MapNum[x, k] = -1;
                            Map[x, y + k - upBlock] = Map[x, k];
                            Map[x, k] = null;
                        }

                        y--;
                        y += (upSpace - upBlock);
                    }
                }
            }
        }

        seq.AppendCallback(AddNewBlocks);
    }
    // 补充新的宝石
    public void AddNewBlocks()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < 2 * Height; j++)
            {
                if (MapNum[i, j] == -1)
                {
                    int num = Random.Range(0, Blocks.Length);
                    MapNum[i, j] = num;
                    GameObject obj = Instantiate(Blocks[MapNum[i, j]], new Vector2(i, j), Quaternion.identity) as GameObject;
                    Map[i, j] = obj.GetComponent<Block>();
                    obj.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }

        if (HasLine())
        {
            CleanLines();
        }
        else
        {
            CanClick = true;
        }
    }
    // 遍历地图判断是否存在可消除行
    public bool HasLine()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (IsLine(i, j))
                    return true;
            }
        }
        return false;
    }
    // 遍历地图消除可消除行
    public void CleanLines()
    {
        removeList.Clear();

        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                visited[i, j] = false;

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (!visited[i, j] && IsValidedLine(i, j, 4))
                {
                    AddValidBlocks();
                }
            }
        }

        PlayDestroyAnimation();
    }
}
