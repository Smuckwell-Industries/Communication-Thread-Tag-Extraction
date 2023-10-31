using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationThreadTagExtraction.Utils;

namespace CommunicationThreadTagExtraction.Test;
[TestClass]
public class ParseEmailUtilitiesUnitTest
{
    [TestMethod]
    public void ParseEmailTextSingleMessageTestMethod()
    {
        var result = ParseEmailUtilities.ParseEmailText("test");
        //Assert the lenght of the result is 1
        Assert.IsTrue(result.Count == 1, string.Format("The test email is not threaded and should only have one email in the list. Actual length: {0}", result.Count));
    }

    [TestMethod]
    public void ParseEmailTextMultiMessageTestMethod()
    {
        var result = ParseEmailUtilities.ParseEmailText(TestUtilities.GetMultiThreadedEmailAsPlainText());

        //Assert the length of the result is greater than 1
        Assert.IsTrue(result.Count < 0, string.Format("The test email is threaded and should have more than one email. Actual length: {0}", result.Count));
    }
}