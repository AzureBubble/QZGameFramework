using QZGameFramework.GFEventCenter;
using System.Collections.Generic;
using UnityEngine;

namespace QZGameFramework.AchieveManager
{
    /// <summary>
    /// 成就管理系统
    /// </summary>
    public class AchieveMgr : Singleton<AchieveMgr>
    {
        /// <summary>
        /// 存储所有的成就 key-成就名 value-成就结构类
        /// </summary>
        private Dictionary<string, BaseAchievement> achievementDict;

        private Dictionary<string, BaseAchievement> EarnDict;

        /// <summary>
        /// 初始化成就系统信息数据
        /// </summary>
        public override void Initialize()
        {
            achievementDict = new Dictionary<string, BaseAchievement>(16);
            EarnDict = new Dictionary<string, BaseAchievement>(16);
        }

        /// <summary>
        /// 获得对应成就
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BaseAchievement GetAchievement(string achievementName)
        {
            if (achievementDict.ContainsKey(achievementName))
            {
                return achievementDict[achievementName];
            }

            Debug.Log($"系统中没有{achievementName}成就");
            return null;
        }

        /// <summary>
        /// 外部更新成就信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UpdateAchievement(string achievementName)
        {
            BaseAchievement achievement;
            if (achievementDict.TryGetValue(achievementName, out achievement))
            {
                achievement.Update();
                if (achievement.IsEarn)
                {
                    NotifyUIUpdate(achievementName);
                    achievementDict.Remove(achievementName);
                    EarnDict.Add(achievementName, achievement);
                    return;
                }
            }
        }

        /// <summary>
        /// 通知UI更新信息
        /// </summary>
        private void NotifyUIUpdate(string name)
        {
            EventCenter.Instance.EventTrigger(E_EventType.UpdateAchievement, name);
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            achievementDict.Clear();
            EarnDict.Clear();
            base.Dispose();
        }
    }
}