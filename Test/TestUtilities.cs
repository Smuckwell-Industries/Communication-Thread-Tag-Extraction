using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using CommunicationThreadTagExtraction.Utils;

namespace CommunicationThreadTagExtraction.Test;
public class TestUtilities
{
    public static string GetMultiThreadedEmailAsHTML()
    {
        return @"<html>Figure this out later</html>";
    }

    public static string GetMultiThreadedEmailAsPlainText()
    {
        var testString = GetMultiThreadedEmailAsHTML();
        var result = HtmlUtilities.ConvertToPlainText(testString);
        return result;
    }

    public class TestEmail
    {
        public string email_body { get; set; }
    }
}