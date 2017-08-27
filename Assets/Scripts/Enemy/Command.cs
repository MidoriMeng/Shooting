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

public class GoAndWaitCmd : Command {
    Vector3 waitPosition;
    bool evil;
    public float guiltIncreaseSpeed = 0.001f;
    public GoAndWaitCmd(Vector3 waitPos, bool evil, TypeEnum type) {
        waitPosition = waitPos;
        commandType = type;
        completion = CompletionEnum.NotStarted;
        this.evil = evil;
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
        else {
            if (evil)
                enemy.DoEvil(guiltIncreaseSpeed);//与普通等车区别在此：增加罪恶值
        }
    }
}