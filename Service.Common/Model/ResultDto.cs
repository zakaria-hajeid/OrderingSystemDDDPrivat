namespace Service.Common.Model
{
    public class Result
    {
        public string Description { get; }
        public string Message { get; }
        public string Code { get; }
        public bool IsSpecialError { get; }
        internal static Result None => new Result(string.Empty, string.Empty, string.Empty);

        public Result(string description, string message, string shortCode)
        {
            Description = description;
            Message = message;
            Code = shortCode;
        }

        public Result(string description, string message, string shortCode, bool isSpecialError)
        {
            Description = description;
            Message = message;
            Code = shortCode;
            IsSpecialError = isSpecialError;
        }
    }
}
