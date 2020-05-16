using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityServer.Application.Operation.Client;
using IdentityServer.Application.Request.Client;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Mapper;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace IdentityServer.Web.Services
{
    public class ClientService : Clients.ClientsBase
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IServiceProvider provider, ILogger<ClientService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public override async Task<CreateClientReplay> CreateClient(CreateClientRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Going to execute {nameof(ClientCreateOperation)}");
            var operation = _provider.GetRequiredService<ClientCreateOperation>();
            Result result;
            using (MiniProfiler.Current.Step(nameof(CreateClient)))
            {
                result = await operation.ExecuteAsync(new ClientCreate
                {
                    Name =  request.Name,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    IsEnable = request.IsEnable
                }).ConfigureAwait(false);
            }
            
            return _provider.GetService<IMapper<Result, CreateClientReplay>>()
                .Map(result);
        }

        public override async Task<UpdateClientReplay> UpdateClient(UpdateClientRequest request, ServerCallContext context)
        {
            Result result;
            if (Guid.TryParse(request.Id, out var id))
            {
                _logger.LogInformation($"Going to execute {nameof(ClientUpdateOperation)}");
                    
                var operation = _provider.GetRequiredService<ClientUpdateOperation>();
                using (MiniProfiler.Current.Step(nameof(ClientUpdateOperation)))
                {
                    result = await operation.ExecuteAsync(new ClientUpdate
                    {
                        Id = id,
                        Name = request.Name,
                        ClientId = request.ClientId,
                        ClientSecret = request.ClientSecret,
                        IsEnable = request.IsEnable
                    }).ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.ClientError.InvalidId;
            }
                
            return _provider.GetService<IMapper<Result, UpdateClientReplay>>()
                .Map(result);
        }

        public override async Task<GetClientByIdReplay> GetClientById(GetClientByIdRequest request, ServerCallContext context)
        {
            Result result;
            if (Guid.TryParse(request.Id, out var id))
            {
                _logger.LogInformation($"Going to execute {nameof(ClientGetOperation)}");
                    
                var operation = _provider.GetRequiredService<ClientGetOperation>();
                using (MiniProfiler.Current.Step(nameof(ClientGetOperation)))
                {
                    result = await operation.ExecuteAsync(new ClientGetById
                        {
                            Id = id
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.ClientError.InvalidId;
            }

            return _provider.GetService<IMapper<Result, GetClientByIdReplay>>()
                .Map(result);
        }

        public override async Task GetClients(GetClientsRequest request, IServerStreamWriter<Client> responseStream, ServerCallContext context)
        {
            _logger.LogInformation($"Going to execute {nameof(ClientGetAllOperation)}");
                    
            var operation = _provider.GetRequiredService<ClientGetAllOperation>();
            var result = await operation.ExecuteAsync(new ClientGetAll())
                .ConfigureAwait(false);

            var mapper = _provider.GetRequiredService<IMapper<Domain.Common.Client, Client>>();
            if (result is OkResult<IAsyncEnumerable<Domain.Common.Client>> ok)
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

        public override async Task<AddClientPermissionReplay> AddPermission(AddClientPermissionRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var clientId) 
               && Guid.TryParse(request.PermissionId, out var permissionId))
            {
                _logger.LogInformation($"Going to execute {nameof(ClientAddPermissionOperation)}");
                var operation = _provider.GetRequiredService<ClientAddPermissionOperation>();
                using (MiniProfiler.Current.Step(nameof(ClientAddPermissionOperation)))
                {
                    result = await operation.ExecuteAsync(new ClientAddPermission
                        {
                            Id = clientId,
                            PermissionId = permissionId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.ClientError.InvalidId;
            }

            return _provider.GetService<IMapper<Result, AddClientPermissionReplay>>()
                .Map(result);
        }

        public override async Task<RemoveClientPermissionReplay> RemovePermission(RemoveClientPermissionRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var clientId) 
               && Guid.TryParse(request.PermissionId, out var permissionId))
            {
                _logger.LogInformation($"Going to execute {nameof(ClientRemovePermissionOperation)}");
                var operation = _provider.GetRequiredService<ClientRemovePermissionOperation>();
                using (MiniProfiler.Current.Step(nameof(ClientRemovePermissionOperation)))
                {
                    result = await operation.ExecuteAsync(new ClientRemovePermission
                        {
                            Id = clientId,
                            PermissionId = permissionId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.ClientError.InvalidId;
            }

            return _provider.GetService<IMapper<Result, RemoveClientPermissionReplay>>()
                .Map(result);
        }

        public override async Task<AddClientRoleReplay> AddRole(AddClientRoleRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var id) 
               && Guid.TryParse(request.RoleId, out var roleId))
            {
                _logger.LogInformation($"Going to execute {nameof(ClientAddRoleOperation)}");
                var operation = _provider.GetRequiredService<ClientAddRoleOperation>();
                using (MiniProfiler.Current.Step(nameof(ClientAddRoleOperation)))
                {
                    result = await operation.ExecuteAsync(new ClientAddRole
                        {
                            Id = id,
                            RoleId = roleId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.ClientError.InvalidId;
            }
            
            return _provider.GetService<IMapper<Result, AddClientRoleReplay>>()
                .Map(result);
        }

        public override async Task<RemoveClientRoleReplay> RemoveRole(RemoveClientRoleRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var id) 
               && Guid.TryParse(request.RoleId, out var roleId))
            {
                _logger.LogInformation($"Going to execute {nameof(ClientRemoveRoleOperation)}");
                var operation = _provider.GetRequiredService<ClientRemoveRoleOperation>();
                using (MiniProfiler.Current.Step(nameof(ClientRemoveRoleOperation)))
                {
                    result = await operation.ExecuteAsync(new ClientRemoveRole
                        {
                            Id = id,
                            RoleId = roleId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.ClientError.InvalidId;
            }
            
            return _provider.GetService<IMapper<Result, RemoveClientRoleReplay>>()
                .Map(result);
        }

        public override async Task<AddClientResourceReplay> AddResource(AddClientResourceRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var id) 
               && Guid.TryParse(request.ResourceId, out var resourceId))
            {
                _logger.LogInformation($"Going to execute {nameof(ClientAddResourceOperation)}");
                var operation = _provider.GetRequiredService<ClientAddResourceOperation>();
                using (MiniProfiler.Current.Step(nameof(ClientAddResourceOperation)))
                {
                    result = await operation.ExecuteAsync(new ClientAddResource
                        {
                            Id = id,
                            ResourceId = resourceId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.ClientError.InvalidId;
            }
            
            return _provider.GetService<IMapper<Result, AddClientResourceReplay>>()
                .Map(result);
        }

        public override async Task<RemoveClientResourceReplay> RemoveResource(RemoveClientResourceRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var id) 
               && Guid.TryParse(request.ResourceId, out var resourceId))
            {
                _logger.LogInformation($"Going to execute {nameof(ClientRemoveResourceOperation)}");
                var operation = _provider.GetRequiredService<ClientRemoveResourceOperation>();
                using (MiniProfiler.Current.Step(nameof(ClientRemoveResourceOperation)))
                {
                    result = await operation.ExecuteAsync(new ClientRemoveResource
                        {
                            Id = id,
                            ResourceId = resourceId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.ClientError.InvalidId;
            }
            
            return _provider.GetService<IMapper<Result, RemoveClientResourceReplay>>()
                .Map(result);
        }
    }
}