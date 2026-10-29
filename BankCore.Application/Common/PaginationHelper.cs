using BankCore.Application.DTOs.Requests;

namespace BankCore.Application.Common;

public class PaginationHelper
{
    public const int DefaultPageSize = 10;

    public const int MaxPageSize = 100;

    public static PaginationRequest Validate(PaginationRequest request)
    {
        var page = request.Page < 1 ? 1 : request.Page;

        var pageSize = request.PageSize < 1 ? DefaultPageSize : Math.Min(request.PageSize, MaxPageSize);

        return new PaginationRequest(page, pageSize);
    }

    public static int CalculatePages(int totaCount, int pageSize)
    {
        return (int)Math.Ceiling(totaCount / (double)pageSize);
    }
}
