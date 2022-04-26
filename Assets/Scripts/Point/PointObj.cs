namespace PointData
{
    public enum ColorType { Green, Yellow, Orange, Red, Pink, Empty, Super }

    public class PointObj
    {
        public int tier { get; set; }
        public ColorType color { get; set; }
        public Index index { get; set; }
        public bool isEmpty { get; set; }
        public bool isControl { get; set; }
        public bool isCombine { get; set; }
        public bool isBomb { get; set; }

        public PointObj(int tier, ColorType color, Index index, bool isEmpty, bool isControl, bool isCombine, bool isBomb)
        {
            this.tier = tier;
            this.color = color;
            this.index = index;
            this.isEmpty = isEmpty;
            this.isControl = isControl;
            this.isCombine = isCombine;
            this.isBomb = isBomb;
        }

        public PointObj(Point point)
        {
            tier = point.Tier;
            color = point.color;
            index = point.index;
            isEmpty = point.IsEmpty;
            isControl = point.IsControl;
            isCombine = point.IsCombine;
            isBomb = point.IsBomb;
        }
    }
}