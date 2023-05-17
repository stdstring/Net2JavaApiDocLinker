package javareflectionapp;

import org.junit.Assert;
import org.junit.Test;

/**
 *
 * @author std_string
 */
public class RequestProcessorFactoryTest {
    @Test public void get() {
        RequestProcessor classListRequestProcessor = factory.get(new Request(RequestCommandDef.getClassesFromJar, "Path2Jar"));
        Assert.assertNotNull(classListRequestProcessor);
        Assert.assertTrue(classListRequestProcessor instanceof ClassListRequestProcessor);
        RequestProcessor classMemberRequestProcessor = factory.get(new Request(RequestCommandDef.getMembersForClass, "Package.Class"));
        Assert.assertNotNull(classMemberRequestProcessor);
        Assert.assertTrue(classMemberRequestProcessor instanceof ClassMemberRequestProcessor);
        Assert.assertNull(factory.get(new Request("UnknownCommand", "SomeArgs")));
    }
    
    private final RequestProcessorFactory factory = new RequestProcessorFactory();
}
