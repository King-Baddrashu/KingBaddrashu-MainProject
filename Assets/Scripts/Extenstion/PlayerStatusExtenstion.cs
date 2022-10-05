using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatusExtenstion
{
    public static string PlayerStatusType2Info(PlayerStatusType type)
    {
        switch (type)
        {
            case PlayerStatusType.MONEY: return "��";
            case PlayerStatusType.MAX_HEALTH: return "�ִ� ü��";
            case PlayerStatusType.CUR_HEALTH: return "���� ü��";
            case PlayerStatusType.IQ: return "����";
            case PlayerStatusType.APPERARANCE: return "�ܸ�";
            case PlayerStatusType.MORALITY: return "������";
            case PlayerStatusType.SOCIALITY: return "��ȸ��";
            case PlayerStatusType.SYMPATHY: return "����";
            case PlayerStatusType.STRESS: return "��Ʈ����";
        }

        return "ERROR";
    }
}
