using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Queue<Action> actions = new Queue<Action>();

    private void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(actions.Count>0)
        {
            actions.Dequeue().Invoke();
        }
    }
}
