using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    public interface IGoapActionLeafData
    {
        StateSet CreatePreconditions();
        StateSet CreateEffects();
    }
}
