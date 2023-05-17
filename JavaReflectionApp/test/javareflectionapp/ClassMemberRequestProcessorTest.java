package javareflectionapp;

import testpackage.TestEnum;
import testpackage.TestClassWithCtor;
import testpackage.TestClassWithDefaultCtor;
import java.io.IOException;
import org.junit.Assert;
import org.junit.Test;

/**
 *
 * @author std_string
 */
public class ClassMemberRequestProcessorTest {
    @Test public void processEnumRequest() throws IOException, ClassNotFoundException {
        Request request = new Request(RequestCommandDef.getMembersForClass, TestEnum.class.getCanonicalName());
        String actualResponse = requestProcessor.process(request);
        String expectedResponse = "F:testpackage.TestEnum.ENUM_VALUE1;F:testpackage.TestEnum.ENUM_VALUE2;M:testpackage.TestEnum.values();M:testpackage.TestEnum.valueOf(java.lang.String)";
        Assert.assertEquals(expectedResponse, actualResponse);
    }
    
    @Test public void processClassWithDefaultCtorRequest() throws IOException, ClassNotFoundException {
        Request request = new Request(RequestCommandDef.getMembersForClass, TestClassWithDefaultCtor.class.getCanonicalName());
        String actualResponse = requestProcessor.process(request);
        String expectedResponse = "C:testpackage.TestClassWithDefaultCtor.TestClassWithDefaultCtor();M:testpackage.TestClassWithDefaultCtor.doIt(int);M:testpackage.TestClassWithDefaultCtor.getData()";
        Assert.assertEquals(expectedResponse, actualResponse);
    }
    
    @Test public void processClassWithCtorRequest() throws IOException, ClassNotFoundException {
        Request request = new Request(RequestCommandDef.getMembersForClass, TestClassWithCtor.class.getCanonicalName());
        String actualResponse = requestProcessor.process(request);
        String expectedResponse = "C:testpackage.TestClassWithCtor.TestClassWithCtor(long);M:testpackage.TestClassWithCtor.doIt(boolean);M:testpackage.TestClassWithCtor.getData()";
        Assert.assertEquals(expectedResponse, actualResponse);
    }
    
    private final RequestProcessor requestProcessor = new ClassMemberRequestProcessor();
}