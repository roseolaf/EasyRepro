using System;

namespace Draeger.Testautomation.CredentialsManagerCore.Bases
{
    public abstract class Singleton<T> where T: class
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<T> _instance = new Lazy<T>( CreateSingletonInstance);

        public static T Instance => _instance.Value;

        private static T CreateSingletonInstance()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }
    }
}