using System.Collections.Generic;
using PointData;

public class TutorialTask
{
    public List<PointObj>[] controlPointsPool = new List<PointObj>[4];
    public List<TutorialPoint> pointsOrder;
    public PointObj[] preloadPoints;

    public TutorialTask(List<PointObj>[] controlPointsPool, List<TutorialPoint> pointsOrder, PointObj[] preloadPoints)
    {
        this.controlPointsPool = controlPointsPool;
        this.pointsOrder = pointsOrder;
        this.preloadPoints = preloadPoints;
    }
}

public class TutorialPoint
{
    public int control;
    public Index target;

    public TutorialPoint(int control, Index target)
    {
        this.control = control;
        this.target = target;
    }
}



public class TutorialPoint1
{
    public int cpIndex;
    public Index targetIndex;
    public PointObj point;

    public TutorialPoint1(int cpIndex, Index targetIndex, PointObj point)
    {
        this.cpIndex = cpIndex;
        this.targetIndex = targetIndex; 
        this.point = point;
    }
}