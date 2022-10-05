public enum ScriptType
{
    RESULT = 2, NORMAL = 0, OPTIONAL = 1
}

public enum ResultInfo
{
    ERROR = -1, OK = 0, FALSE = 1, NOT_FOUND = 2, NOT_IMPLEMENTED = 3
}

public enum RelationalOper
{
    GREATER,
    LESS,
    EQUALS,
    N_EQUALS
}

public enum NPCStatusType
{
    NORMAL,         // 보통
    HAPPINESS,      // 기쁨
    GLOOM,          // 우울
    ANGER,          // 분노
    SAD,            // 슬픔
    ANNOYANCE,      // 짜증
    SURPRISE,       // 놀람
    SHY             // 수줍
}

public enum NPCType
{
    NONE,           // 플레이어
    NPC1,
    NPC2,
    NPC3,
    NPC4,
    NPC5,
    NPC6,
    NPC7,
    NPC8,        
    NPC9,
    NPC10
}