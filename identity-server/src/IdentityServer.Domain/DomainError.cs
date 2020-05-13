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
            
            public static ErrorResult NotFound { get; } = Fail("ROL008", "Role not found");
            public static ErrorResult NameAlreadyExist { get; } = Fail("ROL009", "Name already exist");
            public static ErrorResult InvalidId { get; } = Fail("ROL010", "Invalid Id");

        }
        
        public static class PermissionError
        {
            public static ErrorResult MissingName { get; } = Fail("PRM000", "Missing name");
            public static ErrorResult InvalidName { get; } = Fail("PRM001", "Invalid name");
            
            public static ErrorResult MissingDisplayName { get; } = Fail("PRM002", "Missing display name");
            public static ErrorResult InvalidDisplayName { get; } = Fail("PRM003", "Invalid display name");
            
            public static ErrorResult InvalidDescription { get; } = Fail("PRM004", "Invalid description");
            public static ErrorResult NotFound { get; } = Fail("PRM005", "Permission not found");
            
            public static ErrorResult InvalidId { get; } = Fail("PRM006", "Invalid Id");
            public static ErrorResult NameAlreadyExist { get; } = Fail("PRM007", "Name already exist");
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
            
            
            public static ErrorResult NotFound { get; } = Fail("USR011", "User not found");
            public static ErrorResult MailAlreadyExist { get; } = Fail("USR012", "E-mail already exist");
            public static ErrorResult InvalidId { get; } = Fail("USR013", "Invalid Id");
        }
        
        public static class ResourceError
        {
            public static ErrorResult MissingName { get; } = Fail("RSC000", "Missing name");
            public static ErrorResult InvalidName { get; } = Fail("RSC001", "Invalid name");
            
            public static ErrorResult MissingDisplayName { get; } = Fail("RSC002", "Missing display name");
            public static ErrorResult InvalidDisplayName { get; } = Fail("RSC003", "Invalid display name");
            
            public static ErrorResult InvalidDescription { get; } = Fail("RSC004", "Invalid description");
            
            public static ErrorResult NotFound { get; } = Fail("RSC005", "Resource not found");
            
            public static ErrorResult InvalidId { get; } = Fail("RSC006", "Invalid Id");
            public static ErrorResult NameAlreadyExist { get; } = Fail("RSC007", "Name already exist");
        }
        
        public static class ClientError
        {
            public static ErrorResult MissingName { get; } = Fail("CLT000", "Missing name");
            public static ErrorResult InvalidName { get; } = Fail("CLT001", "Invalid name");
            
            public static ErrorResult MissingClientId { get; } = Fail("CLT002", "Missing client id");
            public static ErrorResult InvalidClientId { get; } = Fail("CLT003", "Invalid client id");
            
            public static ErrorResult MissingClientSecret { get; } = Fail("CLT004", "Missing client secret");
            public static ErrorResult InvalidClientSecret { get; } = Fail("CLT005", "Invalid client secret");
            
            public static ErrorResult InvalidPermission { get; } = Fail("CLT007", "Invalid permission");
            public static ErrorResult PermissionAlreadyExist { get; } = Fail("CLT008", "Client already have this permission");
            public static ErrorResult NotContainsPermission { get; } = Fail("CLT009", "Client does not have this permission");
            
            public static ErrorResult InvalidRole { get; } = Fail("CLT010", "Invalid role");
            public static ErrorResult RoleAlreadyExist { get; } = Fail("CLT011", "Client already have this role");
            public static ErrorResult NotContainsRole { get; } = Fail("CLT012", "Client does not have this role");
            
            public static ErrorResult InvalidResource { get; } = Fail("CLT013", "Invalid resource");
            public static ErrorResult ResourceAlreadyExist { get; } = Fail("CLT014", "Client already have this resource");
            public static ErrorResult NotContainsResource { get; } = Fail("CLT015", "Client does not have this resource");
            
            public static ErrorResult InvalidId { get; } = Fail("CLT016", "Invalid Id");
            public static ErrorResult NameAlreadyExist { get; } = Fail("CLT017", "Name already exist");
            
            public static ErrorResult ClientIdAlreadyExist { get; } = Fail("CLT017", "Name already exist");

        }
    }
}