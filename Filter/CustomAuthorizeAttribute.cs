using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    public CustomAuthorizeAttribute() : base(typeof(CustomAuthorizeFilter))
    {
    }
}
