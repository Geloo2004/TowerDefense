using System;
using System.Collections.Generic;
using System.Numerics;

public partial class DetectionCell
{
    List<Unit> unitsOnCell;
    public void AddUnits(List<Unit> unitsOnCell)
    {
        throw new NotImplementedException();
        //	unitsOnCell.Add();
    }

    public List<Unit> GetUnits()
    {
        return unitsOnCell;
    }

    DetectionCell()
    {
        unitsOnCell = new List<Unit>();
    }
}
