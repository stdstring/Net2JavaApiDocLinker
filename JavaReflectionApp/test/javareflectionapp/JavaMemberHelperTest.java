package javareflectionapp;

import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import org.junit.Assert;
import org.junit.Test;
import testpackage.TestClassWithCtor;
import testpackage.TestClassWithDefaultCtor;
import testpackage.TestEnum;

/**
 *
 * @author std_string
 */
public class JavaMemberHelperTest {
    @Test public void GetField() {
        Field field = findField(TestEnum.class.getDeclaredFields(), "ENUM_VALUE2");
        Assert.assertEquals("testpackage.TestEnum.ENUM_VALUE2", JavaMemberHelper.getField(TestEnum.class, field));
    }
    
    @Test public void getSignature4Constructor() {
        Constructor ctor = TestClassWithCtor.class.getDeclaredConstructors()[0];
        Assert.assertEquals("testpackage.TestClassWithCtor.TestClassWithCtor(long)", JavaMemberHelper.getSignature(TestClassWithCtor.class, ctor));
        Constructor defaultCtor = TestClassWithDefaultCtor.class.getDeclaredConstructors()[0];
        Assert.assertEquals("testpackage.TestClassWithDefaultCtor.TestClassWithDefaultCtor()", JavaMemberHelper.getSignature(TestClassWithDefaultCtor.class, defaultCtor));
    }
    
    @Test public void getSignature4Method() {
        Method method1 = findMethod(TestClassWithCtor.class.getMethods(), "doIt");
        Assert.assertEquals("testpackage.TestClassWithCtor.doIt(boolean)", JavaMemberHelper.getSignature(method1));
        Method method2 = findMethod(TestClassWithCtor.class.getMethods(), "getData");
        Assert.assertEquals("testpackage.TestClassWithCtor.getData()", JavaMemberHelper.getSignature(method2));
    }
    
    private static Field findField(Field[] fields, String name) {
        for(Field field : fields) {
            if(field.getName().equals(name))
                return field;
        }
        return null;
    }
    
    private static Method findMethod(Method[] methods, String name) {
        for(Method method : methods) {
            if(method.getName().equals(name))
                return method;
        }
        return null;
    }
}
