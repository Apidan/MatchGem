using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Block : MonoBehaviour {
    private GameController gameController;
    private Grid grid;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        gameController = GameController.Instance;
        grid = Grid.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (transform.position.y < grid.Height)
            spriteRenderer.enabled = true;
    }
    private void SelectedBlock()
    {
        grid.Selected.transform.position = transform.position;
        grid.Selected.SetActive(true);
    }

    private void UnSelectedBlock()
    {
        grid.Selected.SetActive(false);
    }
    private void OnMouseUpAsButton()
    {
        if (!grid.CanClick)
            return;

        grid.IsClickedFirst = !grid.IsClickedFirst;

        if (grid.IsClickedFirst)
        {
            // 添加选择图标
            SelectFirstBlock();
        }
        else
        {
            // 取消选择图标
            UnSelectedBlock();

            // 判断是否相邻
            if (!IsNeighbor())
            {
                grid.IsClickedFirst = !grid.IsClickedFirst;
                SelectFirstBlock();
                return;
            }

            grid.CanClick = false;

            // 交换宝石位置
            SwapBlocks(false);
        }
    }
    private bool IsNeighbor()
    {
        if (Mathf.Abs(transform.position.x - grid.FirstBlockPos.x) +
            Mathf.Abs(transform.position.y - grid.FirstBlockPos.y) == 1)
            return true;
        return false;
    }
    private void SwapArray(int x1, int y1, int x2, int y2)
    {
        int n = grid.MapNum[x1, y1];
        grid.MapNum[x1, y1] = grid.MapNum[x2, y2];
        grid.MapNum[x2, y2] = n;

        Block b = grid.Map[x1, y1];
        grid.Map[x1, y1] = grid.Map[x2, y2];
        grid.Map[x2, y2] = b;
    }
    private void SwapBlocks(bool isReset)
    {
        SwapArray((int)transform.position.x, (int)transform.position.y,
            (int)grid.FirstBlockPos.x, (int)grid.FirstBlockPos.y);

        Vector2 thisPos = transform.position;
        Vector2 thatPos = grid.FirstBlockPos;
        grid.FirstBlockPos = thisPos;

        Sequence seq = DOTween.Sequence();
        Tweener move1 = transform.DOMove(thatPos, 0.5f);
        Tweener move2 = grid.FirstBlock.transform.DOMove(thisPos, 0.5f);
        seq.Insert(0f, move1);
        seq.Insert(0f, move2);

        if (!isReset)
            seq.AppendCallback(CleanBlocks);
        else
            seq.AppendCallback(() => { grid.CanClick = true; });
    }
    private void SelectFirstBlock()
    {
        grid.FirstBlock = this;
        grid.FirstBlockPos = new Vector2(transform.position.x, transform.position.y);
        SelectedBlock();
    }
    private void CleanBlocks()
    {
        // 判断是否可消除
        if (grid.IsValid((int)transform.position.x, (int)transform.position.y))
        {
            grid.PlayDestroyAnimation();

            if (grid.IsDead())
            {
                gameController.GameOver();
            }
        }
        else
        {
            SwapBlocks(true);
        }
    }

}
