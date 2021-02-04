using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public bool bWalkable;
    public Vector3 nodeWorldPosition;
    public int iStartCost;
    public int iEndCost;
    public int iGridX;
    public int iGridY;

    public Node parentNode;

    public int iCombinedCost { get { return iStartCost + iEndCost; } }

    public Node(bool _bWalkable, Vector3 _nodePosition, int _iGridX, int _iGridY)
    {
        bWalkable = _bWalkable;
        nodeWorldPosition = _nodePosition;

        iGridX = _iGridX;
        iGridY = _iGridY;
    }
}
