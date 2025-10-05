using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Tokens.Queries;

public class GetTokenQuery : IRequest<IResponseWrapper>
{
    public TokenRequest TokenRequest { get; set; }
}

public class GetTokenQueryHandler(ITokenService tokenService) : IRequestHandler<GetTokenQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await tokenService.LoginAsync(request.TokenRequest);

        return await ResponseWrapper<TokenResponse>.SuccessAsync(token);
    }
}
