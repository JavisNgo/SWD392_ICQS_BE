using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IBlogsService
    {
        IEnumerable<BlogsView> GetBlogs();

        IEnumerable<BlogsView> getAllBlogsOfContractor(int contractorid);

        BlogsView GetBlogByCode(string? code);

        BlogsView AddBlog(BlogsView blogView);

        BlogsView UpdateBlog(BlogsView blogView);

        Blogs ChangeStatusBlog(string? code);

        Blogs DeleteBlog(string? code);
    }
}
