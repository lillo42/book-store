using System;

namespace IdentityServer.Domain.Abstractions
{
    public abstract class Result : IEquatable<Result>
    {
        public virtual bool IsSuccess { get; }
        public string ErrorCode { get; }
        public string Description { get; }
        public object Value { get; }

        protected Result()
        {
            IsSuccess = true;
        }

        protected Result(object value)
        {
            IsSuccess = true;
            Value = value;
        }

        protected Result(string error, string description)
        {
            IsSuccess = false;
            ErrorCode = error ?? throw new ArgumentNullException(nameof(error));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        private static readonly OkResult s_ok = new OkResult();
        public static OkResult Ok()
            => s_ok;

        public static OkResult<T> Ok<T>(T value)
            => new OkResult<T>(value);

        public static ErrorResult Fail(string error, string description)
            => new ErrorResult(error, description);
        
        public static ErrorResult Fail(Exception exception)
            => new ErrorResult(exception.HResult.ToString(), exception.ToString());

        public bool Equals(Result other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return IsSuccess == other.IsSuccess && ErrorCode == other.ErrorCode
                && Description == other.Description && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Result result)
            {
                return Equals(result);
            }

            return false;
        }

        public override int GetHashCode() 
            => HashCode.Combine(IsSuccess, ErrorCode, Description, Value);
    }

    public class OkResult : Result
    {
        public override bool IsSuccess => true;
    }

    public class OkResult<T> : Result
    {
        public override bool IsSuccess => true;
        public new T Value { get; }

        public OkResult(T value)
            : base(value)
        {
            Value = value;
        }
    }

    public class ErrorResult : Result
    {
        public override bool IsSuccess => false;

        public ErrorResult(string error, string description) 
            : base(error, description)
        {
        }
    }
}