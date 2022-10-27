using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DpSequence : Node
{
    BehaviourTree dependancy;
    NavMeshAgent agent;

    public DpSequence(string n, BehaviourTree dep, NavMeshAgent ag)
    {
        name = n;
        dependancy = dep;
        agent = ag;
    }

    public DpSequence(string n, BehaviourTree dep)
    {
        name = n;
        dependancy = dep;
    }

    public override Status Process()
    {
        if(dependancy.Process() == Status.FAILURE)
        {
            if (agent)
            {
                agent.ResetPath();
            }
            foreach(Node n in children)
            {
                n.Reset();
            }
            return Status.FAILURE;
        }
        Status childstatus = children[currentChild].Process();
        if(childstatus == Status.RUNNING)
        {
            return childstatus;
        }
        if(childstatus == Status.FAILURE)
        {
            return childstatus;
        }
        currentChild++;
        if(currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }
        return Status.RUNNING;
    }
}
