using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;

public class AStarPathfinding : MonoBehaviour
{
    AStarGrid nodesGrid;

    private void Awake()
    {
        nodesGrid = GetComponent<AStarGrid>();
    }
    public void FindPath(PathRequest _pathRequest, Action<PathResult> _callBack)
    {
        Vector3[] _wayPoints = new Vector3[0];
        bool _bPathFoundSuccess = false;

        Node _startNode = nodesGrid.GetNodeFromWorldPosition(_pathRequest.pathStart);
        Node _targetNode = nodesGrid.GetNodeFromWorldPosition(_pathRequest.pathEnd);
        _startNode.parentNode = _startNode;

        List<Node> _openNodesSet = new List<Node>();
        HashSet<Node> _closedNodesSet = new HashSet<Node>();

        _openNodesSet.Add(_startNode);

        while (_openNodesSet.Count > 0)
        {
            Node _currentNode = _openNodesSet[0];
            for (int i = 1; i < _openNodesSet.Count; i++) // i = 1 because 0 is already current node
            {
                if(_openNodesSet[i].iCombinedCost < _currentNode.iCombinedCost)
                {
                    _currentNode = _openNodesSet[i];
                }
                else if(_openNodesSet[i].iCombinedCost == _currentNode.iCombinedCost)
                {
                    if (_openNodesSet[i].iEndCost < _currentNode.iEndCost)
                        _currentNode = _openNodesSet[i];
                }
            }
            _openNodesSet.Remove(_currentNode);
            _closedNodesSet.Add(_currentNode);
          
            if(_currentNode == _targetNode)
            {
                _bPathFoundSuccess = true;
                break;
            }

            List<Node> _neighboursNodesLst = nodesGrid.GetValidNeighbourNodes(_currentNode);

            foreach (Node _neighbourNode in _neighboursNodesLst)
            {
                if (!_neighbourNode.bWalkable || _closedNodesSet.Contains(_neighbourNode))
                    continue;

                int _iNewMoveCostToNeighbour = _currentNode.iStartCost + GetDistance(_currentNode, _neighbourNode);

                if(_iNewMoveCostToNeighbour < _neighbourNode.iStartCost || !_openNodesSet.Contains(_neighbourNode)) // opening neighbour for first time
                {
                    Node _oldNeighbourNode = new Node(_neighbourNode.bWalkable, _neighbourNode.nodeWorldPosition, _neighbourNode.iGridX, _neighbourNode.iGridY);

                    _neighbourNode.iStartCost = _iNewMoveCostToNeighbour;
                    _neighbourNode.iEndCost = GetDistance(_neighbourNode, _targetNode);
                    _neighbourNode.parentNode = _currentNode;

                    if (!_openNodesSet.Contains(_neighbourNode))
                    {
                        _openNodesSet.Add(_neighbourNode);
                    }
                    else // Updating existing item
                    {
                        for (int i = 0; i < _openNodesSet.Count; i++)
                        {
                            if (_openNodesSet[i] == _oldNeighbourNode)
                                _openNodesSet[i] = _neighbourNode;
                        }
                    }
                }
            }
        }
        if (_bPathFoundSuccess)
        {
            _wayPoints = RetracePath(_startNode, _targetNode); // If path is found then this returns the path to the pathfinding manager
            _bPathFoundSuccess = _wayPoints.Length > 0;
        }
        _callBack(new PathResult(_wayPoints, _bPathFoundSuccess, _pathRequest.callBack));
    }
    public Vector3[] RetracePath(Node _startNode, Node _endNode) // Retracing path from the resulting node to the start node and reversing the direction.
    {
        List<Node> _finalPath = new List<Node>();
        Node _currentNode = _endNode;

        while(_currentNode != null)
        {
            _finalPath.Add(_currentNode);
            _currentNode = _currentNode.parentNode;
        }
        _finalPath.Reverse();
        Vector3[] _wayPoints = SimplifyPath(_finalPath);
        return _wayPoints;
    }

    public Vector3[] SimplifyPath(List<Node> _path) // Simplifying path so that if the resulting nodes are in same direction it should only return one waypoint, when the direction changes then it only it should add other waypoint
    {
        List<Vector3> _wayPoints = new List<Vector3>();
       // Vector3 _oldDirection = Vector3.zero;
        for (int i = 1; i < _path.Count; i++)
        {
           // Vector3 _newDirection = new Vector3(_path[i-1].iGridX - _path[i].iGridX, 0, _path[i - 1].iGridY - _path[i].iGridY);
          //  if(_oldDirection != _newDirection)
            {
                _wayPoints.Add(_path[i].nodeWorldPosition);
            }
          //  _oldDirection = _newDirection;
        }
        return _wayPoints.ToArray();
    }
    public int GetDistance(Node _nodeA, Node _nodeB)
    {
        int distX = Mathf.Abs(_nodeA.iGridX - _nodeB.iGridX);
        int distY = Mathf.Abs(_nodeA.iGridY - _nodeB.iGridY);

        if(distX > distY)
        {
           return 14 * distY + 10 * (distX - distY); // just a formula to assign costs to the nodes Formula: 14x + 10(x-y) and vice versa for opposite axis
        }
        return 14 * distX + 10 * (distY - distX);
    }


    public Vector3[] FindPathV2(Vector3 _startPos, Vector3 _endPos)
    {
        Vector3[] _wayPoints = new Vector3[0];
        bool _bPathFoundSuccess = false;

        Node _startNode = nodesGrid.GetNodeFromWorldPosition(_startPos);
        Node _targetNode = nodesGrid.GetNodeFromWorldPosition(_endPos);

        List<Node> _openNodesSet = new List<Node>();
        HashSet<Node> _closedNodesSet = new HashSet<Node>();

        _openNodesSet.Add(_startNode);

        while (_openNodesSet.Count > 0)
        {
            Node _currentNode = _openNodesSet[0];
            for (int i = 1; i < _openNodesSet.Count; i++) // i = 1 because 0 is already current node
            {
                if (_openNodesSet[i].iCombinedCost < _currentNode.iCombinedCost)
                {
                    _currentNode = _openNodesSet[i];
                }
                else if (_openNodesSet[i].iCombinedCost == _currentNode.iCombinedCost)
                {
                    if (_openNodesSet[i].iEndCost < _currentNode.iEndCost)
                        _currentNode = _openNodesSet[i];
                }
            }
            _openNodesSet.Remove(_currentNode);
            _closedNodesSet.Add(_currentNode);

            if (_currentNode == _targetNode)
            {
                _bPathFoundSuccess = true;
                break;
            }

            List<Node> _neighboursNodesLst = nodesGrid.GetValidNeighbourNodes(_currentNode);

            foreach (Node _neighbourNode in _neighboursNodesLst)
            {
                if (!_neighbourNode.bWalkable || _closedNodesSet.Contains(_neighbourNode))
                    continue;

                int _iNewMoveCostToNeighbour = _currentNode.iStartCost + GetDistance(_currentNode, _neighbourNode);

                if (_iNewMoveCostToNeighbour < _neighbourNode.iStartCost || !_openNodesSet.Contains(_neighbourNode)) // opening neighbour for first time
                {
                    Node _oldNeighbourNode = new Node(_neighbourNode.bWalkable, _neighbourNode.nodeWorldPosition, _neighbourNode.iGridX, _neighbourNode.iGridY);

                    _neighbourNode.iStartCost = _iNewMoveCostToNeighbour;
                    _neighbourNode.iEndCost = GetDistance(_neighbourNode, _targetNode);
                    _neighbourNode.parentNode = _currentNode;

                    if (!_openNodesSet.Contains(_neighbourNode))
                    {
                        _openNodesSet.Add(_neighbourNode);
                    }
                    else // Updating existing item
                    {
                        for (int i = 0; i < _openNodesSet.Count; i++)
                        {
                            if (_openNodesSet[i] == _oldNeighbourNode)
                                _openNodesSet[i] = _neighbourNode;
                        }
                    }
                }
            }
        }
        if (_bPathFoundSuccess)
        {
            _wayPoints = RetracePath(_startNode, _targetNode); // If path is found then this returns the path to the pathfinding manager
        }

        return _wayPoints;

    }
}
