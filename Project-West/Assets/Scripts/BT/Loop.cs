using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : Node
{
    BehaviourTree dependancy;

    public Loop(string n, BehaviourTree dep)
    {
        name = n;
        dependancy = dep;
    }

    public override Status Process()
    {
        if(dependancy.Process() == Status.FAILURE)
        {
            return Status.SUCCESS;
        }
        Status childstatus = children[currentChild].Process();
        if (childstatus == Status.RUNNING)
        {
            return childstatus;
        }
        if (childstatus == Status.FAILURE)
        {
            currentChild = 0;
            foreach (Node n in children)
            {
                n.Reset();
            }
            return childstatus;
        }
        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
        }
        return Status.RUNNING;
    }
}
