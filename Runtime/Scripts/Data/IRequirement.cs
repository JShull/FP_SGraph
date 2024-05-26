using System;

namespace FuzzPhyte.SGraph
{
    public interface IRequirement<OB> 
    {
        public abstract bool CheckRequirement(OB value);
    }
}
