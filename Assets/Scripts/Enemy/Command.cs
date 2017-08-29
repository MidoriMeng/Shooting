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

public class GotoAndWait : Command {
    Vector3 waitPosition;
    public float guiltIncreaseSpeed = 0.001f;
    public GotoAndWait(Vector3 waitPos, TypeEnum type) {
        waitPosition = waitPos;
        commandType = type;
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
        RunTemplate(enemy);
    }

    public virtual void RunTemplate(Enemy enemy) { }
}

public class GotoAndDisappear : GotoAndWait {
    bool disappeared = false;
    public GotoAndDisappear(Vector3 waitPos, TypeEnum type) : base(waitPos, type) { }
    public override void RunTemplate(Enemy enemy) {
        if (!disappeared) {
            if (completion == CompletionEnum.Finished) {
                EnemyManager.Instance.Recycle(enemy);
                disappeared = true;
            }
        }
    }
}

public class BadGotoAndWait : GotoAndWait {
    bool evil;
    public BadGotoAndWait(Vector3 waitPos, TypeEnum type)
        : base(waitPos, type) {
        this.evil = type == TypeEnum.BadWaitForCar ? true : false;
    }

    public override void RunTemplate(Enemy enemy) {
        if (completion == CompletionEnum.Finished) {
            if (evil)
                enemy.DoEvil(guiltIncreaseSpeed);//与普通等车区别在此：增加罪恶值
        }
    }
}