using UnityEngine;
using UnityEngine.UI;

namespace Heartline.App.RuntimeUi
{
    /// <summary>
    /// Sizes GridLayoutGroup cells to fill the grid's rect, so the match-3 board fits any phone
    /// aspect ratio / safe area instead of overflowing with fixed pixel cells.
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    public sealed class GridCellFitter : MonoBehaviour
    {
        public int columns = 6;
        public int rows = 8;
        public float maxAspect = 1.25f; // cap cell width/height so gems don't turn into long bars

        GridLayoutGroup _grid;
        RectTransform _rt;
        Vector2 _lastSize;

        void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
            _rt = (RectTransform)transform;
        }

        void LateUpdate()
        {
            if (_grid == null || _rt == null) return;
            var size = _rt.rect.size;
            if (size == _lastSize || size.x <= 0 || size.y <= 0) return;
            _lastSize = size;
            float w = (size.x - _grid.spacing.x * (columns - 1)) / columns;
            float h = (size.y - _grid.spacing.y * (rows - 1)) / rows;
            if (w > h * maxAspect) w = h * maxAspect;
            _grid.cellSize = new Vector2(Mathf.Floor(Mathf.Max(8f, w)), Mathf.Floor(Mathf.Max(8f, h)));
        }
    }
}
