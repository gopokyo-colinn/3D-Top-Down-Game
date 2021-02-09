using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public MeshRenderer worldMap; // here goes the ground object
    //public Vector2 gridSize;
    Node[,] nodesGrid;
    public float fNodeRadius;

    int iTotalNodesInX;
    int iTotalNodesInY;
    float fNodeDiameter;

    float worldMapSizeX;
    float worldMapSizeZ;
    private void Awake()
    {
        fNodeDiameter = fNodeRadius * 2;
        iTotalNodesInX = Mathf.RoundToInt(worldMap.bounds.size.x / fNodeDiameter); // total number of nodes along x direction
        iTotalNodesInY = Mathf.RoundToInt(worldMap.bounds.size.z / fNodeDiameter); // total number of nodes along z direction
        CreateGrid();
      //  player = PlayerController.Instance.transform;
    }
    public void CreateGrid()
    {
        nodesGrid = new Node[iTotalNodesInX, iTotalNodesInY];

        Vector3 worldBottonLeftPosition = new Vector3(worldMap.bounds.min.x, 1, worldMap.bounds.min.z);

        for (int x = 0; x < iTotalNodesInX; x++)
        {
            for (int y = 0; y < iTotalNodesInY; y++)
            {
                Vector3 _nodePositionInWorld = new Vector3(worldBottonLeftPosition.x + (x * fNodeDiameter + fNodeRadius), 1, worldBottonLeftPosition.z + (y * fNodeDiameter + fNodeRadius));//worldBottonLeftPosition + Vector3.right * (x * fNodeDiameter + fNodeRadius) + Vector3.forward * (y * fNodeDiameter + fNodeRadius);
                bool _bWalkable = CheckWalkable(_nodePositionInWorld, fNodeRadius);

                nodesGrid[x, y] = new Node(_bWalkable, _nodePositionInWorld, x, y);
            }
        }

        worldMapSizeX = worldMap.bounds.size.x;
        worldMapSizeZ = worldMap.bounds.size.z;
    }
    public bool CheckWalkable(Vector3 _position, float _fRadius)
    {
        Collider[] _coliLst = Physics.OverlapSphere(_position, _fRadius);
        foreach (var _coli in _coliLst)
        {
            if (_coli.gameObject.layer != LayerMask.NameToLayer("Ground") && _coli.gameObject.layer != LayerMask.NameToLayer("Item"))
            {
                if(_coli.gameObject.GetComponent<Animator>() == null)
                    return false;
            }
        }
        return true;
    }
    public Node GetNodeFromWorldPosition(Vector3 _worldPosition)
    {
        //float _fPercentX = (_worldPosition.x + worldMap.bounds.size.x / 2) / worldMap.bounds.size.x;
        //float _fPercentY = (_worldPosition.z + worldMap.bounds.size.z / 2) / worldMap.bounds.size.z;
        float _fPercentX = (_worldPosition.x / worldMapSizeX) + 0.5f;// same as above, just calculations made easy for computers
        float _fPercentY = (_worldPosition.z / worldMapSizeZ) + 0.5f;// 

        _fPercentX = Mathf.Clamp01(_fPercentX);
        _fPercentY = Mathf.Clamp01(_fPercentY);

        int _x = Mathf.FloorToInt((iTotalNodesInX)* _fPercentX);
        int _z = Mathf.FloorToInt((iTotalNodesInY) * _fPercentY);

        return nodesGrid[_x,_z];
    }

    public List<Node> GetValidNeighbourNodes(Node _currentNode)
    {
        List<Node> _neighbourLst = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) // that is the current node, we need only neighbours
                    continue;

                int _nodePosX = _currentNode.iGridX + x;
                int _nodePosY = _currentNode.iGridY + y;

                if(_nodePosX >= 0 && _nodePosX < iTotalNodesInX && _nodePosY >= 0 && _nodePosY < iTotalNodesInY)
                {
                    _neighbourLst.Add(nodesGrid[_nodePosX, _nodePosY]);
                }

            }
        }
        return _neighbourLst;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(worldMap.bounds.size.x, 1, worldMap.bounds.size.y));
        //if (nodesGrid != null)
        //{
        //    foreach (Node _node in nodesGrid)
        //    {
        //        Gizmos.color = (_node.bWalkable) ? Color.blue : Color.red;
        //        Gizmos.DrawCube(_node.nodeWorldPosition, Vector3.one * (fNodeDiameter - 0.2f));
        //    }
        //}
    }

}
