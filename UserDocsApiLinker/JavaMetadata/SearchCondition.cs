using System;

namespace UserDocsApiLinker.JavaMetadata
{
    internal interface ISearchCondition
    {
        Boolean IsMatch(String parameter);
    }

    internal class SimpleValueSearchCondition : ISearchCondition
    {
        public SimpleValueSearchCondition(String value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            this.value = value;
        }

        public Boolean IsMatch(String parameter)
        {
            return Equals(value, parameter);
        }

        private readonly String value;
    }

    internal class ArraySearchCondition : ISearchCondition
    {
        public ArraySearchCondition(String[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            this.array = array;
        }

        public Boolean IsMatch(String parameter)
        {
            return Array.Find(array, value => Equals(value, parameter)) != null;
        }

        private readonly String[] array;
    }

    internal class FalseSearchCondition : ISearchCondition
    {
        public Boolean IsMatch(String parameter)
        {
            return false;
        }
    }
}
