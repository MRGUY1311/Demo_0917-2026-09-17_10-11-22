[System.Serializable]
public struct ActionTiming
{
    public float startupDuration;
    public float activeDuration;
    public float recoveryDuration;

    // 只描述阶段时长，阶段推进仍由动作脚本负责。
    public float TotalDuration =>
        startupDuration + activeDuration + recoveryDuration;

    public ActionTiming(
        float _startupDuration,
        float _activeDuration,
        float _recoveryDuration)
    {
        startupDuration = _startupDuration;
        activeDuration = _activeDuration;
        recoveryDuration = _recoveryDuration;
    }

    public static readonly ActionTiming Empty =  default;
}   