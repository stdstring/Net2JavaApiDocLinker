package javareflectionapp;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;
import java.util.jar.JarEntry;
import java.util.jar.JarFile;

/**
 *
 * @author std_string
 */
public class ClassListRequestProcessor implements RequestProcessor {
    @Override
    public String process(Request request) throws IOException {
        List<String> responseStorage = new ArrayList<String>();
        String jarFileName = getFullPath2Jar(request.getArg());
        JarFile jarFile = new JarFile(jarFileName);
        Enumeration<JarEntry> entries = jarFile.entries();
        while(entries.hasMoreElements()) {
            JarEntry entry = entries.nextElement();
            String entryName = entry.getName();
            if(entryName.endsWith(classSuffix)) {
                int classSuffixIndex = entryName.indexOf(classSuffix);
                String className = entryName.substring(0, classSuffixIndex);
                responseStorage.add(className.replace('/', '.'));
            }
        }        
        return new ResponseBuilder().build(responseStorage);
    }
    
    private String getFullPath2Jar(String jarFileName) {
        String classPath = System.getProperty("java.class.path");
        String[] classPathElements = classPath.split(classPathElementDelim);
        for(String classPathElement : classPathElements) {
            if (classPathElement.endsWith(jarFileName))
                return classPathElement;
        }
        return jarFileName;
    }
    
    private static final String classSuffix = ".class";
    private static final String classPathElementDelim = ";";
}
