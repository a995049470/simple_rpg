using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{

    /// <summary>
    /// 计划结果
    /// </summary>
    public enum PlanResult
    {
        //压根没法找到完成的方法
        Fail,
        //有个行为没法完成 中止了
        Cancel,
        //任务完成
        Success

    }
    /// <summary>
    /// GOAP的实体类
    /// 1.目标的安排
    /// 2.寻路
    /// 3.提供数据
    /// </summary>
    public interface IGoap 
    {

        /// <summary>
        /// 创建一个目标
        /// TODO:单一目标? 目标集合?
        /// </summary>
        /// <returns></returns>
        StateSet CreateGoalState();

        //获取世界的信息
        StateSet GetWorldState();

        //移动到目标地点
        bool Move(BaseGoapActionLeaf aciton);

        //计划完成 
        void PlanFinsih(StateSet goal, PlanResult result);
    }
}
