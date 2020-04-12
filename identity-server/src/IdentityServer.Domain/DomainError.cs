using IdentityServer.Domain.Abstractions;
using static IdentityServer.Domain.Abstractions.Result;

namespace IdentityServer.Domain
{
    public static class DomainError
    {
        public static class RoleError
        {
            public static ErrorResult MissingName { get; } = Fail("ROL000", "Missing name");
            public static ErrorResult InvalidName { get; } = Fail("ROL001", "Invalid name");
            
            public static ErrorResult MissingDisplayName { get; } = Fail("ROL002", "Missing display name");
            public static ErrorResult InvalidDisplayName { get; } = Fail("ROL003", "Invalid display name");
            
            public static ErrorResult InvalidDescription { get; } = Fail("ROL004", "Invalid description");
            
            public static ErrorResult InvalidPermission { get; } = Fail("ROL005", "Invalid permission");
            
            public static ErrorResult PermissionAlreadyExist { get; } = Fail("ROL006", "Role already have this permission");
            
            public static ErrorResult NotContainsPermission { get; } = Fail("ROL007", "Role does not have this permission");
        }
        
        public static class PermissionError
        {
            public static ErrorResult MissingName { get; } = Fail("PRM000", "Missing name");
            public static ErrorResult InvalidName { get; } = Fail("PRM001", "Invalid name");
            
            public static ErrorResult MissingDisplayName { get; } = Fail("PRM002", "Missing display name");
            public static ErrorResult InvalidDisplayName { get; } = Fail("PRM003", "Invalid display name");
            
            public static ErrorResult InvalidDescription { get; } = Fail("PRM004", "Invalid description");
        }
        
        public static class UserError
        {
            public static ErrorResult MissingMail { get; } = Fail("USR000", "Missing mail");
            public static ErrorResult InvalidMail { get; } = Fail("USR001", "Invalid mail");
            
            public static ErrorResult MissingPassword { get; } = Fail("USR002", "Missing password");
            public static ErrorResult InvalidPassword { get; } = Fail("USR003", "Invalid password");
            
            
            public static ErrorResult InvalidPermission { get; } = Fail("USR005", "Invalid permission");
            public static ErrorResult PermissionAlreadyExist { get; } = Fail("USR006", "User already have this permission");
            public static ErrorResult NotContainsPermission { get; } = Fail("USR007", "User does not have this permission");
            
            public static ErrorResult InvalidRole { get; } = Fail("USR008", "Invalid role");
            
            public static ErrorResult RoleAlreadyExist { get; } = Fail("USR009", "User already have this role");
            public static ErrorResult NotContainsRole { get; } = Fail("USR010", "User does not have this role");
        }
    }
}