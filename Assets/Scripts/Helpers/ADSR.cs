public struct ADSR
{
    public float attackDuration;
    public float decayDuration;
    public float releaseDuration;

    public float attackValue;
    public float sustainValue;

    public ADSR(float _attackDuration, float _decayDuration, float _releaseDuration, float _attackValue, float _sustainValue)
    {
        attackDuration = _attackDuration;
        decayDuration = _decayDuration;
        releaseDuration = _releaseDuration;
        attackValue = _attackValue;
        sustainValue = _sustainValue;
    }
}
