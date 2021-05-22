namespace SampleCoreConsole.Business
{
    internal class SingletonBusiness : ISingletonBusiness
    {
        private int _i;

        public int GetValue()
        {
            return _i++;
        }
    }
}