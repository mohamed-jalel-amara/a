using AutoMapper;
using Grpc.Core;
using View.grpc.Protos;
using View.grpc.Services.Interfaces;

namespace View.grpc.Services
{
    public class ViewServiceProvider : ViewProtoService.ViewProtoServiceBase
    {
        private readonly ILogger<ViewServiceProvider> _logger;
        private readonly IViewService _service;
        private readonly IMapper _mapper;

        public ViewServiceProvider(IViewService service, ILogger<ViewServiceProvider> logger, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<ViewObject> GetView(GetViewRequest request, ServerCallContext context)
        {
            var view = await _service.GetViewByIdAsync(request.Id);
            if (view.Id == 0)
            {
                _logger.LogInformation($"Failed to retrieve view by id {request.Id}");
                return new ViewObject() { Id = 0};
            }
            else
            {
                _logger.LogInformation($"View is retrieved by id {request.Id}");
                return _mapper.Map<ViewObject>(view);
            }

        }

        public override async Task<ViewAccess> GetUserView(GetUserViewRequest request, ServerCallContext context)
        {
            var access = await _service.GetUserViewAsync(request.Provider, request.ProviderId, request.ViewId) ;
            if (access.Id == 0)
            {
                _logger.LogInformation($"Failed to retrieve access to view by id {request.ViewId} and provider {request.Provider}");
                return new ViewAccess() { Id = 0 };
            }
            else
            {
                _logger.LogInformation($"View is retrieved by id {request.ViewId}");
                return _mapper.Map<ViewAccess>(access);
            }

        }

        public override async Task<ViewObjectList> GetViewsForAccount(GetViewsForAccountRequest request, ServerCallContext context)
        {
            var views = await _service.GetViewsForAccount(request.AccountId);
            if (views.Items.Count() == 0)
            {
                _logger.LogWarning($"No view available for account by id {request.AccountId}");
                return _mapper.Map<ViewObjectList>(views);
            }
            else
            {
                _logger.LogInformation($"Views is retrieved by account by id {request.AccountId}");
                return _mapper.Map<ViewObjectList>(views);
            }
        }
    }
}
