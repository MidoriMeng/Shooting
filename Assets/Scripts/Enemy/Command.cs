using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Command {
    public enum TypeEnum { NormalWaitForCar, BadWaitForCar, ComeUp, Idle };
    public enum CompletionEnum { NotStarted, Doing, Finished };//TODO: delete not started?
    public TypeEnum commandType = TypeEnum.Idle;
    public CompletionEnum completion = CompletionEnum.Finished;
    public virtual void Run(Enemy enemy) { }
}

public class NormalWaitForCarCmd : Command {
    Vector3 waitPosition;
    public NormalWaitForCarCmd(Vector3 waitPos) {
        waitPosition = waitPos;
        commandType = TypeEnum.NormalWaitForCar;
        completion = CompletionEnum.NotStarted;
    }

    public override void Run(Enemy enemy) {
        //base.Action(enemy);
        if (completion != CompletionEnum.Finished) {
            NavMeshAgent agent = enemy.agent;
            agent.enabled = true;
            agent.destination = waitPosition;
            completion = CompletionEnum.Doing;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
                completion = CompletionEnum.Finished;
                agent.enabled = false;
            }
        }
    }
}