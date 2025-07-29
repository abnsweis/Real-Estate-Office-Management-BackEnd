namespace RealEstate.Application.Common.Behaviours
{
    [Serializable]
    internal class ValidationExceptin : Exception
    {
        public ValidationExceptin()
        {
        }

        public ValidationExceptin(string? message) : base(message)
        {
        }

        public ValidationExceptin(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}