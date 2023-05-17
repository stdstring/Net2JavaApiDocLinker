package javareflectionapp;

import org.junit.Assert;
import org.junit.Test;

/**
 *
 * @author std_string
 */
public class RequestParserTest {
    @Test public void parse() {
        final String commandPart = "command";
        final String argPart = "arg";
        Request requestWithArg = parser.parse(commandPart + RequestCommandDef.commandArgDelimiter + argPart);
        Assert.assertNotNull(requestWithArg);
        Assert.assertEquals(commandPart, requestWithArg.getCommand());
        Assert.assertEquals(argPart, requestWithArg.getArg());
        Request requestWithoutArg = parser.parse(commandPart);
        Assert.assertNotNull(requestWithoutArg);
        Assert.assertEquals(commandPart, requestWithoutArg.getCommand());
        Assert.assertNull(requestWithoutArg.getArg());
    }
    
    private final RequestParser parser = new RequestParser();
}
