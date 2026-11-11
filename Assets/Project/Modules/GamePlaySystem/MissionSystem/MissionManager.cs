using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.GameplaySystem.MissionSystem
{
    public class MissionManager : MonoBehaviour
    {
        /// <summary>
        /// HeadMissionNodes 用于存储游戏中的首个任务（例如主线任务的第一章节任务），其余的任务由首任务进行检索，
        /// 当某个首任务被完成后，其将会被保留在该列表中，以方便显示任务记录等。
        /// </summary>
        [SerializeField] private List<MissionNode> HeadMissionNodes = new List<MissionNode>();

        void OnEnable()
        {
            // 读取并存储所有任务节点至节点中
            HeadMissionNodes.Clear();
            
            MissionNode[] missionNodes = Resources.LoadAll<MissionNode>("ScriptableObjects/MissionSO/MissionNodeSO");
            foreach (var node in missionNodes)
            {
                if (node.bIsHeadMissionNode)
                {
                    HeadMissionNodes.Add(node);
                }
            }
        }
    }
}
