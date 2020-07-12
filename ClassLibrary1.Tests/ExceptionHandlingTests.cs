using Microsoft.Practices.EnterpriseLibrary.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Web;

namespace ClassLibrary1.Tests
{
    [TestFixture(Category ="Unit")]
    public class ExceptionHandlingTests
    {
        private ExceptionHandling _exceptionHandling;

        private Mock<IEnterpriseLibrary> _mockEnterpriseLibrary;        

        [SetUp]
        public void Setup()
        {
            _mockEnterpriseLibrary = new Mock<IEnterpriseLibrary>();
            


            _exceptionHandling = new ExceptionHandling(_mockEnterpriseLibrary.Object, FakeHttpContext());
        }

        private static HttpContextBase FakeHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            request.Setup(req => req.QueryString).Returns(new NameValueCollection());
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            return context.Object;
        }

        [Test]
        public void LogException_WhenExceptionPolicyIsAudit_ShouldReturnFalse()
        {
            //Arrange
            _mockEnterpriseLibrary
                .Setup(x => x.HandleException(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(true);

            //Act
            var response = _exceptionHandling.LogException(new Exception(), ExceptionPolicy.Audit_Log);

            //Assert
            Assert.IsFalse(response);

            _mockEnterpriseLibrary.Verify(x => x.HandleException(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void LogException_WhenExceptionPolicyIsWebException_ShouldReturnTrue()
        {
            //Arrange
            var exception = new Exception("Some random web exception");

            var mockHttpContext = new Mock<HttpContextBase>();

            _mockEnterpriseLibrary
                .Setup(x => x.HandleException(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(true);

            //Act
            var response = _exceptionHandling.LogException(exception, ExceptionPolicy.Web_Exception);

            //Assert
            Assert.IsTrue(response);

            _mockEnterpriseLibrary.VerifyAll();
        }

        [Test]
        public void LogException_WhenExceptionPolicyIsSqlException_ShouldReturnTrue()
        {
            //Arrange
            var exception = FormatterServices.GetUninitializedObject(typeof(SqlException))
                as SqlException;

            _mockEnterpriseLibrary
                .Setup(x => x.HandleException(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(true);

            //Act
            var response = _exceptionHandling.LogException(exception, ExceptionPolicy.Database_Exception);

            //Assert
            Assert.IsTrue(response);

            _mockEnterpriseLibrary.VerifyAll();
        }

        [Test]
        public void OutToDatabase_WhenInvoked_ShouldWriteLogOnce()
        {
            //Arrange
            _mockEnterpriseLibrary
                .Setup(x => x.WriteLog(It.IsAny<LogEntry>()))
                .Verifiable();

            //Act
            _exceptionHandling.OutToDatabase("SomeUser", 90, "Some category", System.Diagnostics.TraceEventType.Information, null);

            //Assert
            _mockEnterpriseLibrary.Verify(x => x.WriteLog(It.IsAny<LogEntry>()), Times.Exactly(1));
        }
    }
}
