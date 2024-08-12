using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

using NSubstitute;

namespace DancingGoat.Tests.Unit
{
    /// <summary>
    /// Provides mock of controller context for tests.
    /// </summary>
    public static class ControllerContextMock
    {
        /// <summary>
        /// Gets a mock of controller context.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="editMode">Indicates if edit mode should be enabled.</param>
        public static ControllerContext GetControllerContext(Controller controller, bool editMode = true)
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var pageBuilder = Substitute.For<IPageBuilderFeature>();
            pageBuilder.EditMode.Returns(editMode);
            httpContext.Kentico().SetFeature(pageBuilder);

            return new ControllerContext(httpContext, new RouteData(), controller);
        }
    }
}