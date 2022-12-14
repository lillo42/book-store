using System;

namespace Users.Domain.Event
{
    public class CreateEvent : IEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string Email { get; }
        public string FirstName { get; }
        public string LastNames { get; }
        public DateTime BirthDay { get; }

        public CreateEvent(string email, string firstName, string lastNames, DateTime birthDay)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastNames = lastNames ?? throw new ArgumentNullException(nameof(lastNames));
            BirthDay = birthDay;
        }
    }
}
