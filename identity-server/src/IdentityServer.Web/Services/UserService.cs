using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityServer.Application.Operation.User;
using IdentityServer.Application.Request.User;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Mapper;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace IdentityServer.Web.Services
{
    public class UserService : Users.UsersBase
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<UserService> _logger;

        public UserService(IServiceProvider provider, 
            ILogger<UserService> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CreateUserReplay> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Going to execute {nameof(UserCreateOperation)}");
            var operation = _provider.GetRequiredService<UserCreateOperation>();
            Result result;
            using (MiniProfiler.Current.Step(nameof(CreateUser)))
            {
                result = await operation.ExecuteAsync(new UserCreate
                {
                    Mail = request.Mail,
                    Password = request.Password,
                    IsEnable = request.IsEnable,
                }).ConfigureAwait(false);
            }
            
            return _provider.GetService<IMapper<Result, CreateUserReplay>>()
                .Map(result);
        }

        public override async Task<UpdateUserReplay> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            Result result;
            if (Guid.TryParse(request.Id, out var id))
            {
                _logger.LogInformation($"Going to execute {nameof(UserUpdateOperation)}");
                    
                var operation = _provider.GetRequiredService<UserUpdateOperation>();
                using (MiniProfiler.Current.Step(nameof(UserUpdateOperation)))
                {
                    result = await operation.ExecuteAsync(new UserUpdate
                    {
                        Id = id,
                        Mail = request.Mail,
                        IsEnable = request.IsEnable
                    }).ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.UserError.InvalidId;
            }
                
            return _provider.GetService<IMapper<Result, UpdateUserReplay>>()
                .Map(result);
        }

        public override async Task<GetUserByIeReplay> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            Result result;
            if (Guid.TryParse(request.Id, out var id))
            {
                _logger.LogInformation($"Going to execute {nameof(UserGetOperation)}");
                    
                var operation = _provider.GetRequiredService<UserGetOperation>();
                using (MiniProfiler.Current.Step(nameof(UserGetOperation)))
                {
                    result = await operation.ExecuteAsync(new UserGetById
                        {
                            Id = id
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.UserError.InvalidId;
            }

            return _provider.GetService<IMapper<Result, GetUserByIeReplay>>()
                .Map(result);
        }

        public override async Task GetUsers(GetUsersRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
        {
            _logger.LogInformation($"Going to execute {nameof(UserGetAllOperation)}");
                    
            var operation = _provider.GetRequiredService<UserGetAllOperation>();
            var result = await operation.ExecuteAsync(new UserGetAll())
                .ConfigureAwait(false);

            var mapper = _provider.GetRequiredService<IMapper<Domain.Common.User, User>>();
            if (result is OkResult<IAsyncEnumerable<Domain.Common.User>> ok)
            {
                await foreach (var permission in ok.Value
                    .WithCancellation(context.CancellationToken)
                    .ConfigureAwait(false))
                {
                    await responseStream.WriteAsync(mapper.Map(permission))
                        .ConfigureAwait(false);
                }
            }
        }

        public override async Task<AddUserPermissionReplay> AddPermission(AddUserPermissionRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var userId) 
               && Guid.TryParse(request.PermissionId, out var permissionId))
            {
                _logger.LogInformation($"Going to execute {nameof(UserAddPermissionOperation)}");
                var operation = _provider.GetRequiredService<UserAddPermissionOperation>();
                using (MiniProfiler.Current.Step(nameof(UserAddPermissionOperation)))
                {
                    result = await operation.ExecuteAsync(new UserAddPermission
                        {
                            Id = userId,
                            PermissionId = permissionId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.UserError.InvalidId;
            }
            
            return _provider.GetService<IMapper<Result, AddUserPermissionReplay>>()
                .Map(result);
        }

        public override async Task<RemoveUserPermissionReplay> RemovePermission(RemoveUserPermissionRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var id) 
               && Guid.TryParse(request.PermissionId, out var permissionId))
            {
                _logger.LogInformation($"Going to execute {nameof(UserRemovePermissionOperation)}");
                var operation = _provider.GetRequiredService<UserRemovePermissionOperation>();
                using (MiniProfiler.Current.Step(nameof(UserRemovePermissionOperation)))
                {
                    result = await operation.ExecuteAsync(new UserRemovePermission
                        {
                            Id = id,
                            PermissionId = permissionId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.UserError.InvalidId;
            }
            
            return _provider.GetService<IMapper<Result, RemoveUserPermissionReplay>>()
                .Map(result);
        }


        public override async Task<AddUserRoleReplay> AddRole(AddUserRoleRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var id) 
               && Guid.TryParse(request.RoleId, out var roleId))
            {
                _logger.LogInformation($"Going to execute {nameof(UserAddRoleOperation)}");
                var operation = _provider.GetRequiredService<UserAddRoleOperation>();
                using (MiniProfiler.Current.Step(nameof(UserAddRoleOperation)))
                {
                    result = await operation.ExecuteAsync(new UserAddRole
                        {
                            Id = id,
                            RoleId = roleId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.UserError.InvalidId;
            }
            
            return _provider.GetService<IMapper<Result, AddUserRoleReplay>>()
                .Map(result);
        }

        public override async Task<RemoveUserRoleReplay> RemoveRole(RemoveUserRoleRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var id) 
               && Guid.TryParse(request.RoleId, out var roleId))
            {
                _logger.LogInformation($"Going to execute {nameof(UserRemoveRoleOperation)}");
                var operation = _provider.GetRequiredService<UserRemoveRoleOperation>();
                using (MiniProfiler.Current.Step(nameof(UserRemoveRoleOperation)))
                {
                    result = await operation.ExecuteAsync(new UserRemoveRole
                        {
                            Id = id,
                            RoleId = roleId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.UserError.InvalidId;
            }
            
            return _provider.GetService<IMapper<Result, RemoveUserRoleReplay>>()
                .Map(result);
        }
    }
}