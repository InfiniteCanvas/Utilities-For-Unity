using System;

namespace InfiniteCanvas.Pooling
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PooledAttribute : Attribute
    {
        public string CreateFunc      { get; }
        public string GetAction       { get; }
        public string ReturnAction    { get; }
        public string DestroyAction   { get; }
        public int    DefaultCapacity { get; }
        public int    MaxSize         { get; }

        public PooledAttribute(string createFunc      = "",
                                 string getAction       = "",
                                 string returnAction    = "",
                                 string destroyAction   = "",
                                 int    defaultCapacity = 10,
                                 int    maxSize         = 100)
        {
            CreateFunc = createFunc;
            GetAction = getAction;
            ReturnAction = returnAction;
            DestroyAction = destroyAction;
            DefaultCapacity = defaultCapacity;
            MaxSize = maxSize;
        }
    }
}