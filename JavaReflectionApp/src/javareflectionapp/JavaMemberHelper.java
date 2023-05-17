package javareflectionapp;

import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;

/**
 *
 * @author std_string
 */
public class JavaMemberHelper {
    public static String getField(Class<?> containedClass, Field field) {
        return containedClass.getCanonicalName() + classMemberDelimiter + field.getName();
    }
    
    public static String getSignature(Class<?> containedClass, Constructor<?> ctor) {
        String signature = clearSignature(ctor.toGenericString());
        String constructorName = containedClass.getSimpleName();
        int parametersBlockIndex = signature.indexOf(parametersBlockPrefix);
        return signature.substring(0, parametersBlockIndex) + classNameDelimiter + constructorName + signature.substring(parametersBlockIndex);
    }
    
    public static String getSignature(Method method) {
        return clearSignature(method.toGenericString());
    }
    
    private static String clearSignature(String source) {
        int throwsIndex = source.lastIndexOf(throwsPartPrefix);
        int paramStartIndex = source.indexOf(parametersBlockPrefix);
        int startIndex = source.lastIndexOf(" ", paramStartIndex);
        return throwsIndex == -1 ? source.substring(startIndex+1) : source.substring(startIndex+1, throwsIndex);
    }
    
    private JavaMemberHelper() {}
    
    private static final String classNameDelimiter = ".";
    private static final String classMemberDelimiter = ".";
    private static final String throwsPartPrefix = " throws ";
    private static final String parametersBlockPrefix = "(";
}
