using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Security.Principal;

namespace Mono.Tests.Auction
{
    class MockObject
    {
        public HttpContextBase isAjaxTrue()
        {
            return isAjax(true);
        }

        public HttpContextBase isAjaxFalse()
        {
            return isAjax(false);
        }

        private HttpContextBase isAjax(bool ajaxTrue)
        {
            var request = new Mock<HttpRequestBase>();
            if(ajaxTrue)
                request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
            else
                request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "HttpRequest" } });

            var context = mockCurrentUser();
            context.SetupGet(x => x.Request).Returns(request.Object);

            return context.Object;
        }

        public HttpContextBase currentUser()
        {
            return mockCurrentUser().Object;
        }

        private Mock<HttpContextBase> mockCurrentUser()
        {
            var identity = new Mock<IIdentity>();
            
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.User.Identity).Returns(identity.Object);

            return context;
        }
    }
}
