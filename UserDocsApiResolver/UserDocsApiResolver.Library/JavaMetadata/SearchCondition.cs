using System;

namespace UserDocsApiResolver.Library.JavaMetadata
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
            _value = value;
        }

        public Boolean IsMatch(String parameter)
        {
            return Equals(_value, parameter);
        }

        private readonly String _value;
    }

    internal class ArraySearchCondition : ISearchCondition
    {
        public ArraySearchCondition(String[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            _array = array;
        }

        public Boolean IsMatch(String parameter)
        {
            return Array.Find(_array, value => Equals(value, parameter)) != null;
        }

        private readonly String[] _array;
    }

    internal class FalseSearchCondition : ISearchCondition
    {
        public Boolean IsMatch(String parameter)
        {
            return false;
        }
    }
}
