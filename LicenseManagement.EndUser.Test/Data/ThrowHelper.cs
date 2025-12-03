namespace Hymma.Lm.EndUser.Test.Data
{
    public static class ThrowHelper
    {
        public static void ThrowNoIdException(string className) =>
            throw new Exception($"id of the {className} was 0, insert it into DB and get the new Id");

        internal static void ThrowUnSuccessfulRequest(string uri) =>
            throw new Exception($"A request to server was not successful. {uri}");

    }
}
