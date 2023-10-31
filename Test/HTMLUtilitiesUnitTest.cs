using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using CommunicationThreadTagExtraction.Utils;

namespace CommunicationThreadTagExtraction.Test;
[TestClass]
public class HTMLUtilitiesUnitTest
{
    [TestMethod]
    public void TagsRemovedFromContentWithoutLineFeedsTestMethod()
    {
        var testString = @"<html>" +
            "<head>" +
            "<title>Test</title>" +
            "</head>" +
            "<body>" +
            "<p>This is a test</p>" +
            "</body>" +
            "</html>";
        var result = HtmlUtilities.ConvertToPlainText(testString);
        Assert.IsFalse(result.Contains("<"), "The tags were not removed from the content.");        
    }

    //Test method to check if the tags are removed from the content
    [TestMethod]
    public void TagsRemovedFromContentWithLineFeedsTestMethod()
    {
        var result = HtmlUtilities.ConvertToPlainText(TestUtilities.GetMultiThreadedEmailAsHTML());
        
        Assert.IsFalse(result.Contains("<html>"), "The tags were not removed from the content.");
    } 
}