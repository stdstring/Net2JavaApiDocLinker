package javareflectionapp;

import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

/**
 *
 * @author std_string
 */
public class ClassMemberRequestProcessor implements RequestProcessor {

    @Override
    public String process(Request request) throws ClassNotFoundException {
        Class<?> requestedClass = Class.forName(request.getArg());
        List<String> responseStorage = new ArrayList<String>();
        // fields
        for(Field field : requestedClass.getDeclaredFields()) {
            if(isSuitable(field)) responseStorage.add(String.format(fieldTemplate, JavaMemberHelper.getField(requestedClass, field)));
        }
        // constructors
        for(Constructor ctor : requestedClass.getDeclaredConstructors()) {
            if(isSuitable(ctor)) responseStorage.add(String.format(ctorTemplate, JavaMemberHelper.getSignature(requestedClass, ctor)));
        }
        // methods
        for(Method method : requestedClass.getMethods()) {
            if(isSuitable(method)) responseStorage.add(String.format(methodTemplate, JavaMemberHelper.getSignature(method)));
        }
        return new ResponseBuilder().build(responseStorage);
    }
    
    private static boolean isSuitable(Field field) {
        return Modifier.isPublic(field.getModifiers());
    }
    
    private static boolean isSuitable(Constructor constructor) {
        return Modifier.isPublic(constructor.getModifiers()) || Modifier.isProtected(constructor.getModifiers());
    }
    
    private static boolean isSuitable(Method method) {
        for(String jdkClassPrefix : jdkClassPrefixList) {
            if (method.getDeclaringClass().getCanonicalName().startsWith(jdkClassPrefix)) return false;
        }
        return (Modifier.isPublic(method.getModifiers()) || Modifier.isProtected(method.getModifiers()));
    }
    
    private static final List<String> jdkClassPrefixList = Arrays.asList("java.", "com.sun.");
    private static final String fieldTemplate = "F:%1$s";
    private static final String ctorTemplate = "C:%1$s";
    private static final String methodTemplate = "M:%1$s";
}
