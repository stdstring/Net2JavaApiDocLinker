package javareflectionapp;

import java.io.IOException;
import org.junit.Assert;
import org.junit.Test;

/**
 *
 * @author std_string
 */
public class ClassListRequestProcessorTest {
    @Test public void processClassListRequest() throws IOException, ClassNotFoundException {
        Request request = new Request(RequestCommandDef.getClassesFromJar, "TestLibrary.jar");
        String actualResponse = requestProcessor.process(request);
        final String expectedResponse = "testpackage.TestClassWithCtor;testpackage.TestClassWithDefaultCtor;testpackage.TestEnum";
        Assert.assertEquals(expectedResponse, actualResponse);
    }
    
    private final RequestProcessor requestProcessor = new ClassListRequestProcessor();
}
