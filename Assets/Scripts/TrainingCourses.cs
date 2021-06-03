using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrainingCourses
{
    public static string[] courseNameData = new string[] { "戦闘訓練・初級", "精神強化・初級", "座学・初級" };
    public static string[] courseDescData = new string[] { "模擬戦闘を行い生物の攻撃能力および俊敏性を高める", "生物の精神力を鍛え体力、強靭さを強化する", "講義を行い生物の知性を高める" };

    /// <summary>
    /// 各コースのステータス上昇
    /// statusChangeData = { course1{hp, atk, def...}, course2{hp, atk, def...}, course3... }
    /// </summary>
    public static int[,] statusChangeData = new int[,] { { 0, 3, 0, 2}, { 5, 0, 2, 0 }, { 0, 0, 0, 0 } };
    public static (string[] courseNames, string[] courseDescs, int[,] statusChange) GetCourseInfo()
    {
        return (courseNameData, courseDescData, statusChangeData);
    }
}
