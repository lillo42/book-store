using BookStore.Domain.Abstractions;

namespace BookStore.Domain.Author;

public static class Errors
{
    public static Error NameIsMissing { get; } = new("AUT000", "Name is missing");
    public static Error NameIsTooLarge { get; } = new("AUT001", "Name is too large");
}