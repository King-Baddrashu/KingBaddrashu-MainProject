using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatusExtenstion
{
    public static string PlayerStatusType2Info(PlayerStatusType type)
    {
        switch (type)
        {
            case PlayerStatusType.MONEY: return "돈";
            case PlayerStatusType.MAX_HEALTH: return "최대 체력";
            case PlayerStatusType.CUR_HEALTH: return "현재 체력";
            case PlayerStatusType.IQ: return "지능";
            case PlayerStatusType.APPERARANCE: return "외모";
            case PlayerStatusType.MORALITY: return "도덕성";
            case PlayerStatusType.SOCIALITY: return "사회성";
            case PlayerStatusType.SYMPATHY: return "공감";
            case PlayerStatusType.STRESS: return "스트레스";
        }

        return "ERROR";
    }
}
