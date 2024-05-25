using System;

namespace FuzzPhyte.SGraph
{
    public interface IRequirement<T> 
    {
        public bool IsSatisfied(T value);
    }
}
