using System;
using System.Collections.Generic;
using SecretSauce.Delegates.Data.Storage.ArgsDefinitions;
using Tests.TestDelegates.Data.ArgsDefinitions;

namespace Tests.TestModels.Dependencies
{
    public static class TestContextReplacements
    {
        public static readonly Dictionary<Type, Type> Map = new Dictionary<Type, Type>
        {
            {
                typeof(GetStoredArgsDefinitionsDataAsyncDelegate),
                typeof(TestArgsDefinitionsDataAsyncDelegate)
            }
        };
    }
}