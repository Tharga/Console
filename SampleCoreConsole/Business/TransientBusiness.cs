namespace SampleCoreConsole.Business
{
    internal class TransientBusiness : ITransientBusiness
    {
        private int _i;

        public int GetValue()
        {
            return _i++;
        }
    }
}