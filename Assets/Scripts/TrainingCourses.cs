using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrainingCourses
{
    public static string[] courseNameData = new string[] { "戦闘訓練", "精神強化", "座学" };
    public static string[] courseDescData = new string[] { "模擬戦闘を行い生物の攻撃能力および俊敏性を高める", "生物の精神力を鍛え体力、強靭さを強化する", "講義を行い生物の地性を高める" };

    public static (string[] courseNames, string[] courseDescs) GetCourseInfo()
    {
        return (courseNameData, courseDescData);
    }
}
