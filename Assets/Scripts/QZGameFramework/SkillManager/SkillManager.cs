using QZGameFramework.PersistenceDataMgr;
using System.Collections.Generic;

/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : Singleton<SkillManager>
{
    /// <summary>
    /// 存放所有技能的字典 key:技能名字 键:具体的技能类
    /// </summary>
    private Dictionary<string, BaseSkill> skills = new Dictionary<string, BaseSkill>();

    /// <summary>
    /// 获得对应的技能
    /// </summary>
    /// <typeparam name="T">技能的类型</typeparam>
    /// <returns></returns>
    public T GetSkill<T>() where T : BaseSkill
    {
        string skillName = typeof(T).Name;

        if (skills.ContainsKey(skillName))
        {
            return skills[skillName] as T;
        }

        return null;
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    /// <param name="skill">技能</param>
    /// <returns></returns>
    public BaseSkill AddSkill(BaseSkill skill)
    {
        string skillName = skill.name;

        if (!skills.ContainsKey(skillName))
        {
            skills.Add(skillName, skill);
        }

        return skills[skillName];
    }

    /// <summary>
    /// 删除技能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public bool RemoveSkill<T>() where T : BaseSkill
    {
        string skillName = typeof(T).Name;

        if (skills.ContainsKey(skillName))
        {
            skills.Remove(skillName);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 加载本地技能数据
    /// </summary>
    public void LoadSkills()
    {
        skills = BinaryDataMgr.Instance.LoadData<Dictionary<string, BaseSkill>>("Skills");
    }

    /// <summary>
    /// 保存本地技能数据
    /// </summary>
    public void SaveSkills()
    {
        BinaryDataMgr.Instance.SaveData(skills, "Skills");
    }

    public override void Dispose()
    {
        if (IsDisposed) return;
        skills.Clear();
        base.Dispose();
    }
}