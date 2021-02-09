using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathFindingManager : MonoBehaviour
{
    protected static PathFindingManager instance;
    public static PathFindingManager Instance { get { return instance; } }

    public static Queue<PathResult> resultsQueue = new Queue<PathResult>();
    PathRequest currentPathRequest;
    AStarPathfinding pathFinding;

    private void Awake()
    {
        instance = this;
        pathFinding = GetComponent<AStarPathfinding>();
    }

    private void Update()
    {
        if(resultsQueue.Count > 0)
        {
            int _iTotalResults = resultsQueue.Count;

            lock (resultsQueue)
            {
                for (int i = 0; i < resultsQueue.Count; i++)
                {
                    PathResult _result = resultsQueue.Dequeue();
                    _result.callback(_result.path, _result.bSuccess);
                }
            }
        }
    }

    public static void RequestPath(PathRequest _request)
    {
        ThreadStart _threadStart = delegate // did threading so that pathfinding requests go one by one to save game from freezes
        {
            instance.pathFinding.FindPath(_request, Instance.FinishedProcessingPath);
        };
        Thread _thread = new Thread(_threadStart);
        _thread.Start();
        //_threadStart.Invoke();
    }
    public void FinishedProcessingPath(PathResult _pathResult)
    {
        lock (resultsQueue) // locked so that it should only me accessed by one thread at a time
        {
            resultsQueue.Enqueue(_pathResult);
        }
    }

}
public struct PathResult 
{
    public Vector3[] path;
    public bool bSuccess;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] _path, bool _bSuccess, Action<Vector3[], bool> _callBack)
    {
        path = _path;
        bSuccess = _bSuccess;
        callback = _callBack;
    }

}
public struct PathRequest {
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callBack;

    public PathRequest(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[], bool> _callBack)
    {
        pathStart = _pathStart;
        pathEnd = _pathEnd;
        callBack = _callBack;
    }
}
